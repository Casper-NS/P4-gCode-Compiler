using GOATCode.analysis;
using GOATCode.node;
using System;
using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal enum Extrude
    {
        NotSet,
        None,
        Build,
        Walk
    };

    internal class ExtruderChecker : DepthFirstAdapter
    {
        /// <summary>
        /// The stack which maintains the current Extrude scope
        /// </summary>
        private readonly Stack<Extrude> _stack = new Stack<Extrude>();
        /// <summary>
        /// The hashmap which holds all the functions
        /// </summary>
        private readonly Dictionary<Symbol, BFSNode> _functions = new();
        /// <summary>
        /// The Symbol which is used for keeping track of which function's scope we are currently in
        /// </summary>
        private Symbol _currentSymbol;
        private readonly ISymbolTable _symbolTable;

        internal ExtruderChecker(ISymbolTable symbolTable)
        {
            _symbolTable = symbolTable;
        }

        /// <summary>
        /// This function is a small verifier that checks that it pops the expected extrude type.
        /// </summary>
        /// <param name="checker"></param>
        /// <exception cref="PopException"></exception>
        private void PopVerification(Extrude checker)
        {
            if (_stack.Peek() != checker)
            {
                throw new PopException(_stack.Peek());
            }
            _stack.Pop();
        }

        /// <summary>
        /// Checks if we call a build from a walk and if so throw exception.
        /// </summary>
        /// <param name="candidate"></param>
        /// <exception cref="CallBuildInWalkException"></exception>
        private void PushVerification(Node node, Extrude candidate)
        {
            if (_stack.Peek() == Extrude.Walk)
            {
                throw new StaticCallBuildInWalkException(node, "Tried to build in a walk scope");
            }
            _stack.Push(candidate);
        }

        /// <summary>
        /// Functions to check if a function already is in the functions-dictionary
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsFunctionDeclared(AFuncDecl node) => _functions.ContainsKey(_symbolTable.GetFunctionSymbol(node.GetId().Text));
        private bool IsFunctionDeclared(AFunctionExp node) => _functions.ContainsKey(_symbolTable.GetFunctionSymbol(node.GetName().Text));
        private bool IsFunctionDeclared(AProcDecl node) => _functions.ContainsKey(_symbolTable.GetFunctionSymbol(node.GetId().Text));

        /// <summary>
        /// Sets the "global-scope"'s extrude type to notSet. 
        /// </summary>
        /// <param name="node"></param>
        public override void InADeclProgram(ADeclProgram node) => _stack.Push(Extrude.NotSet);

        /// <summary>
        /// Check if the function is aldready in the function Dictionary,
        /// if not, create a new BFSNode and add it to the dictionary
        /// </summary>
        /// <param name="node">AFuncDecl</param>
        public override void InAFuncDecl(AFuncDecl node)
        {
            _currentSymbol = _symbolTable.GetFunctionSymbol(node.GetId().Text);

            if (!IsFunctionDeclared(node))
            {
                _functions.Add(_currentSymbol, new BFSNode(_currentSymbol.name, Extrude.NotSet));
            }
        }
        public override void InAProcDecl(AProcDecl node)
        {
            _currentSymbol = _symbolTable.GetFunctionSymbol(node.GetId().Text);

            if (!IsFunctionDeclared(node))
            {
                _functions.Add(_currentSymbol, new BFSNode(_currentSymbol.name, Extrude.NotSet));
            }
        }

        /// <summary>
        /// This is when a function is called. Because we can't be sure if the function has been declared,
        /// we add it to the list of function, without all the decleration data.
        /// </summary>
        /// <param name="node"></param>
        public override void OutAFunctionExp(AFunctionExp node)
        {
            if (!IsFunctionDeclared(node)) 
            {
                _functions.Add(_symbolTable.GetFunctionSymbol(node.GetName().Text), new BFSNode(node.GetName().Text, Extrude.NotSet));
            }
            _functions[_currentSymbol].AddFunctionCall(_functions[_symbolTable.GetFunctionSymbol(node.GetName().Text)], _stack.Peek());
        }

        //The In and Out of Blocks sets the extrude type to the function that is declared is
        //and updates the scope extrude type.
        public override void InANoneBlock(ANoneBlock node)
        {
            _functions[_currentSymbol].ExtrudeType = Extrude.None;

            _stack.Push(Extrude.None);
        }
        public override void OutANoneBlock(ANoneBlock node) => PopVerification(Extrude.None);

        public override void InABuildBlock(ABuildBlock node)
        {
            _functions[_currentSymbol].ExtrudeType = Extrude.Build;

            PushVerification(node, Extrude.Build);
        }
        public override void OutABuildBlock(ABuildBlock node) => PopVerification(Extrude.Build);

        public override void InAWalkBlock(AWalkBlock node)
        {
            _functions[_currentSymbol].ExtrudeType = Extrude.Walk;

            _stack.Push(Extrude.Walk);
        }
        public override void OutAWalkBlock(AWalkBlock node) => PopVerification(Extrude.Walk);

        //All of the In and Out below pushes and pops the right extrusion type.
        public override void InABuildExp(ABuildExp node) => PushVerification(node, Extrude.Build);
        public override void OutABuildExp(ABuildExp node) => PopVerification(Extrude.Build);
        public override void InAWalkExp(AWalkExp node) => _stack.Push(Extrude.Walk);
        public override void OutAWalkExp(AWalkExp node) => PopVerification(Extrude.Walk);
        public override void InABuildStmt(ABuildStmt node) => PushVerification(node, Extrude.Build);
        public override void OutABuildStmt(ABuildStmt node) => PopVerification(Extrude.Build);
        public override void InAWalkStmt(AWalkStmt node) => _stack.Push(Extrude.Walk);
        public override void OutAWalkStmt(AWalkStmt node) => PopVerification(Extrude.Walk);

        /// <summary>
        /// Runs the BFSalgorithm funtion as the very last thing when the whole program has been visited.
        /// The function is run on main
        /// </summary>
        /// <param name="node"></param>
        public override void OutADeclProgram(ADeclProgram node) => BFSAlgorithm(_functions[_symbolTable.GetFunctionSymbol("main")]);

        /// <summary>
        /// Breadth first search, which goes through all function calls, and updates Extrude type in the stack call.
        /// </summary>
        /// <param name="node"></param>
        private static void BFSAlgorithm(BFSNode node)
        {
            //Makes frontier list and adds the first node to be checked, and sets the extrude type to none
            //The first node will be main!
            List<BFSNode> frontier = new List<BFSNode>();
            frontier.Add(node);
            node.TheExtrudeTypeFromCallStack = Extrude.None;

            //while the list of function nodes to be checked is not empty...
            while(frontier.Count > 0)
            {
                BFSNode curNode = frontier[0];

                //Go through that function's function calls...
                foreach (var functionCall in curNode.FunctionCalls)
                {
                    Extrude currentExtrude = UpdateExtrudeType(functionCall, curNode);
                    //and check if the call stack extrude type can be updated and if so...
                    if (currentExtrude > functionCall.BFSNode.TheExtrudeTypeFromCallStack)
                    {
                        //set the new call stack type to the function being called
                        //and set where it was called from
                        //and lastly add all the function calls to be checked.
                        functionCall.BFSNode.TheExtrudeTypeFromCallStack = currentExtrude;
                        functionCall.BFSNode.CallerNode = curNode;
                        frontier.Add(functionCall.BFSNode);
                    }
                }
                //Remove the checked function from being checked.
                frontier.Remove(curNode);
            }
        }

        /// <summary>
        /// Checks if a walk calls a build, if so throw exception, else return the highest ranked extrude type.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="node"></param>
        /// <returns>The highest extrude type</returns>
        /// <exception cref="CallBuildInWalkException">Called if walk calls a build</exception>
        private static Extrude UpdateExtrudeType(FunctionCall edge, BFSNode node)
        {
            if (node.TheExtrudeTypeFromCallStack == Extrude.Walk && edge.extrusionType == Extrude.Build)
            {
                edge.BFSNode.CallerNode = node;
                throw new CallBuildInWalkException(edge.BFSNode);
            }
            //Returns the nodes extrudetype if its value is higher, else return the edges extrudetype.
            return node.TheExtrudeTypeFromCallStack > edge.extrusionType ? node.TheExtrudeTypeFromCallStack : edge.extrusionType;
        }
    }
}
