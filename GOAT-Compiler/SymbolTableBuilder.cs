using GOATCode.analysis;
using GOATCode.node;
using System;
using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal class SymbolTableBuilder : DepthFirstAdapter
    {
        
        internal ISymbolTable _symboltable;

        /// <summary>
        /// The dictionary that stores the name of the type, that a variable or function declaration, is stored.
        /// </summary>
        internal Dictionary<Node, string> _typeTable = new Dictionary<Node, string>();

        /// <summary>
        /// The constructor for the SymbolTableBuilder
        /// </summary>
        /// <param name="IST">The instance of the symboltable</param>
        internal SymbolTableBuilder(ISymbolTable IST)
        {
            _symboltable = IST;
        }

        /// <summary>
        /// Method used to find the right Type for decleration nodes, using strings with the same name.
        /// </summary>
        /// <param name="node">The Node used as key in typetable</param>
        /// <returns>Returns the right enum Type for the given node</returns>
        /// <exception cref="TypeAccessException">The exception that is thrown if an invalid string is inputted</exception>
        private Types _processTypeOfNode(Node node)
        {
            switch (_typeTable[node])
            {
                case "int":
                    return Types.Integer;
                case "float":
                    return Types.FloatingPoint;
                case "vector":
                    return Types.Vector;
                case "bool":
                    return Types.Boolean;
                case "void":
                    return Types.Void;
                default:
                    throw new TypeAccessException();
            }
        }

        public override void OutAVarDecl(AVarDecl node)
        {
            _symboltable.AddSymbol(node.GetId().Text, _processTypeOfNode(node));
        }

        public override void OutAParamDecl(AParamDecl node)
        {
            _symboltable.AddSymbol(node.GetId().Text, _processTypeOfNode(node));
        }

        public override void InAFuncDecl(AFuncDecl node)
        {
            _symboltable.OpenScope();
        }
        public override void OutAFuncDecl(AFuncDecl node)
        {
            _symboltable.CloseScope();
            _symboltable.AddSymbol(node.GetId().Text, _processTypeOfNode(node));
        }

        public override void InAProcDecl(AProcDecl node)
        {
            _symboltable.OpenScope();
        }

        public override void OutAProcDecl(AProcDecl node)
        {
            _symboltable.CloseScope();
            _symboltable.AddSymbol(node.GetId().Text, Types.Void);
        }

        public override void InAStmtlistBlock(AStmtlistBlock node)
        {
            _symboltable.OpenScope();
        }

        public override void OutAStmtlistBlock(AStmtlistBlock node)
        {
            _symboltable.CloseScope();
        }

        public override void InABuildBlock(ABuildBlock node)
        {
            _symboltable.OpenScope();
        }

        public override void OutABuildBlock(ABuildBlock node)
        {
            _symboltable.CloseScope();
        }

        public override void InAWalkBlock(AWalkBlock node)
        {
            _symboltable.OpenScope();
        }

        public override void OutAWalkBlock(AWalkBlock node)
        {
            _symboltable.CloseScope();
        }

        /// <summary>
        /// Adds a string with the name of the right type to typeTable.
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutAIntTypes(AIntTypes node) 
        {
            _typeTable.Add(node.Parent(), "int");
        }

        /// <summary>
        /// Adds a string with the name of the right type to typeTable.
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutAFloatTypes(AFloatTypes node)
        {
            _typeTable.Add(node.Parent(), "float");
        }

        /// <summary>
        /// Adds a string with the name of the right type to typeTable.
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutABoolTypes(ABoolTypes node) 
        {
            _typeTable.Add(node.Parent(), "bool");
        }

        /// <summary>
        /// Adds a string with the name of the right type to typeTable. 
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutAVectorTypes(AVectorTypes node)
        {
            _typeTable.Add(node.Parent(), "vector");
        }
    }
}