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

        private void _popVerificationHelper(Extrude checker)
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

        private void _pushVerificationHelper(Extrude candidate)
        {
            if (_stack.Count == 0)
            {
                _stack.Push(candidate);
            }
            else if (_stack.Peek() == Extrude.Build)
            {
                _stack.Push(candidate);
            }
            else
            {
                throw new PushException(candidate);
            }
        }

        public override void InABuildBlock(ABuildBlock node)
        {
            _pushVerificationHelper(Extrude.Build);
        }

        public override void OutABuildBlock(ABuildBlock node)
        {
            _popVerificationHelper(Extrude.Build);
        }

        public override void InABuildExp(ABuildExp node)
        {
            _pushVerificationHelper(Extrude.Build);
        }

        public override void OutABuildExp(ABuildExp node)
        {
            _popVerificationHelper(Extrude.Build);
        }

        public override void InABuildStmt(ABuildStmt node)
        {
            _pushVerificationHelper(Extrude.Build);
        }

        public override void OutABuildStmt(ABuildStmt node)
        {
            _popVerificationHelper(Extrude.Build);
        }

        public override void InAWalkBlock(AWalkBlock node)
        {
            _stack.Push(Extrude.Walk);
        }

        public override void OutAWalkBlock(AWalkBlock node)
        {
            _popVerificationHelper(Extrude.Walk);
        }

        public override void InAWalkExp(AWalkExp node)
        {
            _stack.Push(Extrude.Walk);
        }

        public override void OutAWalkExp(AWalkExp node)
        {
            _popVerificationHelper(Extrude.Walk);

        }

        public override void InAWalkStmt(AWalkStmt node)
        {
            _stack.Push(Extrude.Walk);
        }

        public override void OutAWalkStmt(AWalkStmt node)
        {
            _popVerificationHelper(Extrude.Walk);
        }
    }

}
