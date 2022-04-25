using GOATCode.analysis;
using GOATCode.node;
using System;

namespace GOAT_Compiler
{
    internal class SymbolTableBuilder : DepthFirstAdapter
    {

        private ISymbolTable _table;

        SymbolTableBuilder(ISymbolTable IST)
        {
            _table = IST;
        }

        private Types _processNode(Node node)
        {
            switch (node.ToString())
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
            _table.AddSymbol(node.GetId().ToString(), _processNode(node));
        }

        public override void OutAParamDecl(AParamDecl node)
        {
            _table.AddSymbol(node.GetId().ToString(), _processNode(node));
        }

        public override void OutAFuncDecl(AFuncDecl node)
        {
            _table.AddSymbol(node.GetId().ToString(), _processNode(node)); ;
        }

        public override void OutAProcDecl(AProcDecl node)
        {
            _table.AddSymbol(node.GetId().ToString(), Types.Void);
        }

        public override void InAStmtlistBlock(AStmtlistBlock node)
        {
            _table.OpenScope();
        }

        public override void OutAStmtlistBlock(AStmtlistBlock node)
        {
            _table.CloseScope();
        }

        public override void InABuildBlock(ABuildBlock node)
        {
            _table.OpenScope();
        }

        public override void OutABuildBlock(ABuildBlock node)
        {
            _table.CloseScope();
        }

        public override void InAWalkBlock(AWalkBlock node)
        {
            _table.OpenScope();
        }

        public override void OutAWalkBlock(AWalkBlock node)
        {
            _table.CloseScope();
        }
    }
}