using GOATCode.node;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    internal enum Extrude
    {
        NotSet,
        None,
        Build,
        Walk
    };

    internal class ExtruderChecker : SymbolTableVisitor
    {
        private Stack<Extrude> _stack = new Stack<Extrude>();

        private Symbol _currentSymbol;

        private Dictionary<Symbol, (Extrude ExtType, List<Symbol> SymList)> _functions = new();

        private Extrude _currentExtrude = Extrude.NotSet;

        internal ExtruderChecker(ISymbolTable symbolTable) : base(symbolTable)
        {

        }

        private void _updateExtrudeType(Symbol symbol)
        {
            _functions[_currentSymbol] = (
                                        _overruleExtrudeType(_functions[_currentSymbol].ExtType, _currentExtrude), 
                                        _functions[_currentSymbol].SymList);
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
            if (_stack.Peek() == Extrude.Walk)
            {
                throw new PushException(candidate);
            }
            else
            {
                _stack.Push(candidate);
            }
        }


        public override void InsideScopeInAFuncDecl(AFuncDecl node)
        {
            _currentSymbol = _symbolTable.GetSymbol(node.GetId().Text);
            _functions.Add(_currentSymbol, (Extrude.NotSet ,new List<Symbol>()));
        }

        public override void InsideScopeOutAFuncDecl(AFuncDecl node)
        {
            _updateExtrudeType(_currentSymbol);
        }

        public override void OutAFunctionExp(AFunctionExp node)
        {
            _functions[_currentSymbol].SymList.Add(_symbolTable.GetSymbol(node.GetName().Text));
        }

        

        private Extrude _overruleExtrudeType(Extrude currentType, Extrude candidateType)
        {
            if (candidateType > currentType)
            {
                return candidateType;
            }
            else return currentType;
        }

















        /*public override void OutsideScopeInADeclProgram(ADeclProgram node) 
        {
            _stack.Push(Extrude.Build);
        }

        public override void OutsideScopeOutADeclProgram(ADeclProgram node) 
        {
            _popVerificationHelper(Extrude.Build);
        }*/

        public override void InANoneBlock(ANoneBlock node)
        {
            _stack.Push(Extrude.None);
        }

        public override void OutANoneBlock(ANoneBlock node)
        {
            _popVerificationHelper(Extrude.None);
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
