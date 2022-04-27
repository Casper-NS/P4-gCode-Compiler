using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    internal enum Extrude
    {
        Build,
        Walk
    };

    internal class ExtruderChecker : SymbolTableVisitor
    {
        private Stack<Extrude> _stack = new Stack<Extrude>();

        internal ExtruderChecker(ISymbolTable symbolTable) : base(symbolTable)
        {

        }

        public override void OutsideScopeInADeclProgram(ADeclProgram node) { }
        public override void InsideScopeInADeclProgram(ADeclProgram node) { }


        public override void InABuildBlock(ABuildBlock node)
        {
            DefaultIn(node);
        }

        public override void OutABuildBlock(ABuildBlock node)
        {
            DefaultOut(node);
        }

        public override void InABuildExp(ABuildExp node)
        {
            DefaultIn(node);
        }

        public override void OutABuildExp(ABuildExp node)
        {
            DefaultOut(node);
        }

        public override void InABuildStmt(ABuildStmt node)
        {
            DefaultIn(node);
        }

        public override void OutABuildStmt(ABuildStmt node)
        {
            DefaultOut(node);
        }

        public override void InAWalkBlock(AWalkBlock node)
        {
            DefaultIn(node);
        }

        public override void OutAWalkBlock(AWalkBlock node)
        {
            DefaultOut(node);
        }

        public override void InAWalkExp(AWalkExp node)
        {
            DefaultIn(node);
        }

        public override void OutAWalkExp(AWalkExp node)
        {
            DefaultOut(node);
        }

        public override void InAWalkStmt(AWalkStmt node)
        {
            DefaultIn(node);
        }

        public override void OutAWalkStmt(AWalkStmt node)
        {
            DefaultOut(node);
        }
    }

}
