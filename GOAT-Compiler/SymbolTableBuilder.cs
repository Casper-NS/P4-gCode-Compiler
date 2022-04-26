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
        /// Dictionary 
        /// </summary>
        internal Dictionary<Node, string> _typeTable = new Dictionary<Node, string>();

        internal SymbolTableBuilder(ISymbolTable IST)
        {
            _symboltable = IST;
        }

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
            _symboltable.AddSymbol(node.GetId().Text, _processTypeOfNode(node.GetTypes()));
        }

        public override void OutAFuncDecl(AFuncDecl node)
        {
            _symboltable.AddSymbol(node.GetId().Text, _processTypeOfNode(node));
        }

        public override void OutAProcDecl(AProcDecl node)
        {
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

        public override void OutAIntTypes(AIntTypes node) 
        {
            _typeTable.Add(node.Parent(), "int");
        }

        public override void OutAFloatTypes(AFloatTypes node)
        {
            _typeTable.Add(node.Parent(), "float");
        }

        public override void OutABoolTypes(ABoolTypes node) 
        {
            _typeTable.Add(node.Parent(), "bool");
        }

        public override void OutAVectorTypes(AVectorTypes node)
        {
            _typeTable.Add(node.Parent(), "vector");
        }
    }
}