using System;
using System.Collections.Generic;
using GOAT_Compiler.Exceptions;
using GOATCode.node;

namespace GOAT_Compiler
{
    class ScopeChecker : SymbolTableVisitor
    {
        private HashSet<Symbol> isDeclared = new HashSet<Symbol>();
        private HashSet<Symbol> isInitialized = new HashSet<Symbol>();
        private HashSet<Symbol> isConst = new HashSet<Symbol>();

        public ScopeChecker(ISymbolTable symbolTable) : base(symbolTable)
        {
        }

        public override void OutAVarDecl(AVarDecl node)
        {
            Symbol symbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            isDeclared.Add(symbol);
            if (node.GetExp() != null)
            {
                isInitialized.Add(symbol);
            }

            if (node.GetConst() != null)
            {
                isConst.Add(symbol);
            }
        }


        public override void OutAParamDecl(AParamDecl node)
        {
            Symbol symbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            isDeclared.Add(symbol);
            isInitialized.Add(symbol);
        }


        public override void OutAIdExp(AIdExp node)
        {
            Symbol symbol = _symbolTable.GetVariableSymbol(node.GetId().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node, node.GetId().Text);
            }

            if (isDeclared.Contains(symbol) == false)
            {
                throw new RefUsedBeforeClosestDeclException(node, symbol.Name);
            }

            if (isInitialized.Contains(symbol) == false)
            {
                throw new VarNotInitializedException(node, symbol.Name);
            }
        }

        public override void OutAFunctionExp(AFunctionExp node)
        {
            Symbol symbol = _symbolTable.GetFunctionSymbol(node.GetName().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node, node.GetName().Text);
            }
        }

        public override void OutAAssignStmt(AAssignStmt node)
        {
            Symbol symbol = _symbolTable.GetVariableSymbol(node.GetId().Text);

            if (symbol == null)
            {
                throw new RefNotFoundException(node, node.GetId().Text);
            }

            if (isDeclared.Contains(symbol) == false)
            {
                throw new RefUsedBeforeClosestDeclException(node, symbol.Name);
            }

            if (isConst.Contains(symbol) == true)
            {
                throw new AssignConstException(node, $"Const variable named {symbol.Name} cant be changed");
            }

            if (isInitialized.Contains(symbol) == false)
            {
                isInitialized.Add(symbol);
            }

        }

        public override void OutAAssignPlusStmt(AAssignPlusStmt node)
        {
            GetAndCheckSymbol(node, node.GetId().Text);
        }



        public override void OutAAssignMinusStmt(AAssignMinusStmt node)
        {
            GetAndCheckSymbol(node, node.GetId().Text);
        }

        public override void OutAAssignMultStmt(AAssignMultStmt node)
        {
            GetAndCheckSymbol(node, node.GetId().Text);
        }

        public override void OutAAssignModStmt(AAssignModStmt node)
        {
            GetAndCheckSymbol(node, node.GetId().Text);
        }

        public override void OutAAssignDivisionStmt(AAssignDivisionStmt node)
        {
            GetAndCheckSymbol(node, node.GetId().Text);
        }

        private void GetAndCheckSymbol(Node node, string SymName)
        {
            Symbol symbol = _symbolTable.GetVariableSymbol(SymName);

            if (symbol == null)
            {
                throw new RefNotFoundException(node, SymName);
            }

            if (isDeclared.Contains(symbol) == false)
            {
                throw new RefUsedBeforeClosestDeclException(node, symbol.Name);
            }

            if (isInitialized.Contains(symbol) == false)
            {
                throw new VarNotInitializedException(node, SymName);
            }

            if (isConst.Contains(symbol) == true)
            {
                throw new AssignConstException(node, $"Const variable named {symbol.Name} cant be changed");
            }

        }

    }
}