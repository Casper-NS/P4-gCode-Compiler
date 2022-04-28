using System.Collections.Generic;
using GOAT_Compiler.Exceptions;
using GOATCode.node;

namespace GOAT_Compiler
{
    class ScopeChecker : SymbolTableVisitor
    {
        private HashSet<Symbol> isDeclared = new HashSet<Symbol>();
        private HashSet<Symbol> isInitialized = new HashSet<Symbol>();

        public ScopeChecker(ISymbolTable symbolTable) : base(symbolTable)
        {
        }

        public override void OutAVarDecl(AVarDecl node)
        {
            Symbol symbol = _symbolTable.GetSymbol(node.GetId().Text);
            isDeclared.Add(symbol);
            if (node.GetExp() != null)
            {
                isInitialized.Add(symbol);
            }
        }

        public override void OutAIdExp(AIdExp node)
        {
            Symbol symbol = _symbolTable.GetSymbol(node.GetId().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node.GetId().Text);
            }

            if (isDeclared.Contains(symbol) == false)
            {
                throw new RefUsedBeforeClosestDeclException(symbol.name);
            }

            if (isInitialized.Contains(symbol) == false)
            {
                throw new VarNotInitializedException(node.GetId().Text);
            }

        }

        public override void OutAFunctionExp(AFunctionExp node)
        {
            Symbol symbol = _symbolTable.GetSymbol(node.GetName().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node.GetName().Text);
            }
        }

        public override void OutAAssignStmt(AAssignStmt node)
        {
            Symbol symbol = _symbolTable.GetSymbol(node.GetId().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node.GetId().Text);
            }

            if (isDeclared.Contains(symbol) == false)
            {
                throw new RefUsedBeforeClosestDeclException(symbol.name);
            }

            if (isInitialized.Contains(symbol) == false)
            {
                isInitialized.Add(symbol);
            }
        }

        public override void OutAAssignPlusStmt(AAssignPlusStmt node)
        {
            Symbol symbol = _symbolTable.GetSymbol(node.GetId().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node.GetId().Text);
            }

            if (isDeclared.Contains(symbol) == false)
            {
                throw new RefUsedBeforeClosestDeclException(symbol.name);
            }

            if (isInitialized.Contains(symbol) == false)
            {
                throw new VarNotInitializedException(node.GetId().Text);
            }
        }


        public override void OutAAssignMinusStmt(AAssignMinusStmt node)
        {
            Symbol symbol = _symbolTable.GetSymbol(node.GetId().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node.GetId().Text);
            }

            if (isDeclared.Contains(symbol) == false)
            {
                throw new RefUsedBeforeClosestDeclException(symbol.name);
            }

            if (isInitialized.Contains(symbol) == false)
            {
                throw new VarNotInitializedException(node.GetId().Text);
            }
        }

        public override void OutAAssignMultStmt(AAssignMultStmt node)
        {
            Symbol symbol = _symbolTable.GetSymbol(node.GetId().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node.GetId().Text);
            }

            if (isDeclared.Contains(symbol) == false)
            {
                throw new RefUsedBeforeClosestDeclException(symbol.name);
            }

            if (isInitialized.Contains(symbol) == false)
            {
                throw new VarNotInitializedException(node.GetId().Text);
            }
        }

        public override void OutAAssignModStmt(AAssignModStmt node)
        {
            Symbol symbol = _symbolTable.GetSymbol(node.GetId().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node.GetId().Text);
            }

            if (isDeclared.Contains(symbol) == false)
            {
                throw new RefUsedBeforeClosestDeclException(symbol.name);
            }

            if (isInitialized.Contains(symbol) == false)
            {
                throw new VarNotInitializedException(node.GetId().Text);
            }
        }

        public override void OutAAssignDivisionStmt(AAssignDivisionStmt node)
        {
            Symbol symbol = _symbolTable.GetSymbol(node.GetId().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node.GetId().Text);
            }

            if (isDeclared.Contains(symbol) == false)
            {
                throw new RefUsedBeforeClosestDeclException(symbol.name);
            }

            if (isInitialized.Contains(symbol) == false)
            {
                throw new VarNotInitializedException(node.GetId().Text);
            }
        }

    }
}