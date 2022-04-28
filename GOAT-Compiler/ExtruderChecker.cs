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

    internal class PopException : Exception
    {
        internal PopException(Extrude e)
        {
            Console.WriteLine("Cannot pop " + e);
        }
    }

    internal class PushException : Exception
    {
        internal PushException(Extrude e)
        {
            Console.WriteLine("Cannot push " + e);
        }
    }

    internal class ExtruderChecker : SymbolTableVisitor
    {
        private Stack<Extrude> _stack = new Stack<Extrude>();

        internal ExtruderChecker(ISymbolTable symbolTable) : base(symbolTable)
        {

        }

        private void _popVerificationHelper(Extrude topOfStack, Extrude checker)
        {
            if (topOfStack != checker)
            {
                throw new PopException(topOfStack);
            }
            else
            {
                _stack.Pop();
            }
        }

        private void _pushVerificationHelper(Extrude candidate)
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

        public override void OutsideScopeInADeclProgram(ADeclProgram node) 
        {
            _stack.Push(Extrude.Build);
        }

        public override void OutsideScopeOutADeclProgram(ADeclProgram node) 
        {
            _popVerificationHelper(_stack.Peek(), Extrude.Build);
        }



        public override void InABuildBlock(ABuildBlock node)
        {
            _pushVerificationHelper(Extrude.Build);
        }

        public override void OutABuildBlock(ABuildBlock node)
        {
            _popVerificationHelper(_stack.Peek(), Extrude.Build);
        }

        public override void InABuildExp(ABuildExp node)
        {
            _pushVerificationHelper(Extrude.Build);
        }

        public override void OutABuildExp(ABuildExp node)
        {
            _popVerificationHelper(_stack.Peek(), Extrude.Build);
        }

        public override void InABuildStmt(ABuildStmt node)
        {
            _pushVerificationHelper(Extrude.Build);
        }

        public override void OutABuildStmt(ABuildStmt node)
        {
            _popVerificationHelper(_stack.Peek(), Extrude.Build);
        }

        public override void InAWalkBlock(AWalkBlock node)
        {
            _stack.Push(Extrude.Walk);
        }

        public override void OutAWalkBlock(AWalkBlock node)
        {
            _popVerificationHelper(_stack.Peek(), Extrude.Walk);
        }

        public override void InAWalkExp(AWalkExp node)
        {
            _stack.Push(Extrude.Walk);
        }

        public override void OutAWalkExp(AWalkExp node)
        {
            _popVerificationHelper(_stack.Peek(), Extrude.Walk);

        }

        public override void InAWalkStmt(AWalkStmt node)
        {
            _stack.Push(Extrude.Walk);
        }

        public override void OutAWalkStmt(AWalkStmt node)
        {
            _popVerificationHelper(_stack.Peek(), Extrude.Walk);
        }
    }

}
