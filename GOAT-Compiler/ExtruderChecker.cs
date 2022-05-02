using GOATCode.node;
using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal enum Extrude
    {
        NotSet,
        None,
        Walk,
        Build
    };

    internal class ExtruderChecker : SymbolTableVisitor
    {
        private Stack<Extrude> _stack = new Stack<Extrude>();
        private Dictionary<Symbol, DijkstraNode> _functions = new();
        private Symbol _currentSymbol;
        private Extrude _currentExtrude = Extrude.NotSet;

        internal ExtruderChecker(ISymbolTable symbolTable) : base(symbolTable)
        {
        }

        private void PopVerification(Extrude checker)
        {
            if (_stack.Peek() != checker)
            {
                throw new PopException(_stack.Peek());
            }
            else
            {
                _stack.Pop();
            }
        }

        private void PushVerification(Extrude candidate)
        {
            if (_stack.Peek() == Extrude.Walk)
            {
                throw new PushException(candidate);
            }
            else
            {
                _stack.Push(candidate);
            }
        }
        
        private bool DoesntContainKey(AFuncDecl node)
        {
            return _functions.ContainsKey(_symbolTable.GetSymbol(node.GetId().Text));
        }
        private bool DoesntContainKey(AFunctionExp node)
        {
            return _functions.ContainsKey(_symbolTable.GetSymbol(node.GetName().Text));
        }
        private bool DoesntContainKey(AProcDecl node)
        {
            return _functions.ContainsKey(_symbolTable.GetSymbol(node.GetId().Text));
        }

        /*
            Check if the function is aldready in the function Dictionary,
            if not, create a new DijkstraNode and add it to the dictionary
        */
        public override void InsideScopeInAFuncDecl(AFuncDecl node)
        {
            _currentSymbol = _symbolTable.GetSymbol(node.GetId().Text);

            if (DoesntContainKey(node))
            {
                _functions.Add(_currentSymbol, new DijkstraNode(Extrude.NotSet));
            }
        }

        public override void InsideScopeOutAFuncDecl(AFuncDecl node)
        {
            _functions[_currentSymbol].SetExtrudeType(_currentExtrude);
        }

        public override void InsideScopeInAProcDecl(AProcDecl node)
        {
            _currentSymbol = _symbolTable.GetSymbol(node.GetId().Text);

            if (DoesntContainKey(node))
            {
                _functions.Add(_currentSymbol, new DijkstraNode(Extrude.NotSet));
            }
        }

        public override void InsideScopeOutAProcDecl(AProcDecl node)
        {
            _functions[_currentSymbol].SetExtrudeType(_currentExtrude);
        }

        public override void OutAFunctionExp(AFunctionExp node)
        {
            if (DoesntContainKey(node)) 
            {
                _functions.Add(_symbolTable.GetSymbol(node.GetName().Text), new DijkstraNode(Extrude.NotSet));
            }
            else
            {
                _functions[_currentSymbol].AddFunctionCall(_functions[_symbolTable.GetSymbol(node.GetName().Text)]);
            }
        }

        public override void InANoneBlock(ANoneBlock node)
        {
            _functions[_currentSymbol].SetExtrudeType(Extrude.None);

            _stack.Push(Extrude.None);
            _currentExtrude = Extrude.None;
        }

        public override void OutANoneBlock(ANoneBlock node)
        {
            PopVerification(Extrude.None);
            _currentExtrude = _stack.Peek();
        }

        public override void InABuildBlock(ABuildBlock node)
        {
            _functions[_currentSymbol].SetExtrudeType(Extrude.Build);

            PushVerification(Extrude.Build);
            _currentExtrude = Extrude.Build;
        }
         
        public override void OutABuildBlock(ABuildBlock node)
        {
            PopVerification(Extrude.Build);
            _currentExtrude = _stack.Peek();
        }

        public override void InAWalkBlock(AWalkBlock node)
        {
            _functions[_currentSymbol].SetExtrudeType(Extrude.Walk);

            _stack.Push(Extrude.Walk);
            _currentExtrude = Extrude.Walk;
        }

        public override void OutAWalkBlock(AWalkBlock node)
        {
            PopVerification(Extrude.Walk);
            _currentExtrude = _stack.Peek();
        }

        public override void InABuildExp(ABuildExp node)
        {
            PushVerification(Extrude.Build);
            _currentExtrude = Extrude.Build;
        }

        public override void OutABuildExp(ABuildExp node)
        {
            PopVerification(Extrude.Build);
            _currentExtrude = _stack.Peek();
        }

        public override void InAWalkExp(AWalkExp node)
        {
            _stack.Push(Extrude.Walk);
            _currentExtrude = Extrude.Walk;
        }

        public override void OutAWalkExp(AWalkExp node)
        {
            PopVerification(Extrude.Walk);
            _currentExtrude = _stack.Peek();
        }

        public override void InABuildStmt(ABuildStmt node)
        {
            PushVerification(Extrude.Build);
            _currentExtrude = Extrude.Build;
        }

        public override void OutABuildStmt(ABuildStmt node)
        {
            PopVerification(Extrude.Build);
            _currentExtrude = _stack.Peek();
        }

        public override void InAWalkStmt(AWalkStmt node)
        {
            _stack.Push(Extrude.Walk);
            _currentExtrude = Extrude.Walk;
        }

        public override void OutAWalkStmt(AWalkStmt node)
        {
            PopVerification(Extrude.Walk);
            _currentExtrude = _stack.Peek();
        }

        
    }

    internal static class BTSExtrude
    {
        static void Search(DijkstraNode node, List<DijkstraNode> nodes)
        {
            foreach (var dn in node.GetFunctionCalls())
            {

            }
        }
    }
}
