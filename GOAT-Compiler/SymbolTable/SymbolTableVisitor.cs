using GOATCode.analysis;
using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    /// <summary>
    /// A class that handles scopes for a symbol table
    /// If you inherit from this, scopes are handled automatically.
    /// For safety, this class will seal cases that include modifying scope.
    /// Instead cases OutsideScope______ and InsideScope____ are included, 
    /// that are run inside and outside the scope.
    /// </summary>
    abstract class SymbolTableVisitor : DepthFirstAdapter
    {
        protected ISymbolTable _symbolTable;
        public SymbolTableVisitor(ISymbolTable symbolTable)
        {
            _symbolTable = symbolTable;
        }

        public sealed override void InADeclProgram(ADeclProgram node)
        {
            OutsideScopeInADeclProgram(node);
            _symbolTable.OpenScope(node);
            InsideScopeInADeclProgram(node);
        }
        public virtual void OutsideScopeInADeclProgram(ADeclProgram node) { }
        public virtual void InsideScopeInADeclProgram(ADeclProgram node) { }

        public sealed override void OutADeclProgram(ADeclProgram node)
        {
            InsideScopeOutADeclProgram(node);
            _symbolTable.CloseScope();
            OutsideScopeOutADeclProgram(node);

        }
        public virtual void InsideScopeOutADeclProgram(ADeclProgram node) { }
        public virtual void OutsideScopeOutADeclProgram(ADeclProgram node) { }

        public sealed override void InAFuncDecl(AFuncDecl node)
        {
            OutsideScopeInAFuncDecl(node);
            _symbolTable.OpenScope(node);
            InsideScopeInAFuncDecl(node);
        }
        public virtual void OutsideScopeInAFuncDecl(AFuncDecl node) { }
        public virtual void InsideScopeInAFuncDecl(AFuncDecl node) { }

        public sealed override void OutAFuncDecl(AFuncDecl node)
        {
            InsideScopeOutAFuncDecl(node);
            _symbolTable.CloseScope();
            OutsideScopeOutAFuncDecl(node);
        }
        public virtual void InsideScopeOutAFuncDecl(AFuncDecl node) { }
        public virtual void OutsideScopeOutAFuncDecl(AFuncDecl node) { }

        public sealed override void InAProcDecl(AProcDecl node)
        {
            OutsideScopeInAProcDecl(node);
            _symbolTable.OpenScope(node);
            InsideScopeInAProcDecl(node);
        }
        public virtual void OutsideScopeInAProcDecl(AProcDecl node) { }
        public virtual void InsideScopeInAProcDecl(AProcDecl node) { }

        public sealed override void OutAProcDecl(AProcDecl node)
        {
            InsideScopeOutAProcDecl(node);
            _symbolTable.CloseScope();
            OutsideScopeOutAProcDecl(node);
        }
        public virtual void InsideScopeOutAProcDecl(AProcDecl node) { }
        public virtual void OutsideScopeOutAProcDecl(AProcDecl node) { }

        public sealed override void InAStmtlistBlock(AStmtlistBlock node)
        {
            OutsideScopeInAStmtlistBlock(node);
            if (IsGrandparentNotFuncOrProc(node))
            {
                _symbolTable.OpenScope(node);
            }
            InsideScopeInAStmtlistBlock(node);
        }
        public virtual void OutsideScopeInAStmtlistBlock(AStmtlistBlock node) { }
        public virtual void InsideScopeInAStmtlistBlock(AStmtlistBlock node) { }

        public sealed override void OutAStmtlistBlock(AStmtlistBlock node)
        {
            InsideScopeOutAStmtlistBlock(node);
            if (IsGrandparentNotFuncOrProc(node))
            {
                _symbolTable.CloseScope();
            }
            OutsideScopeOutAStmtlistBlock(node);
        }
        public virtual void InsideScopeOutAStmtlistBlock(AStmtlistBlock node) { }
        public virtual void OutsideScopeOutAStmtlistBlock(AStmtlistBlock node) { }

        /// <summary>
        /// Function to check whether the grandparent of the node is a function or a procedure.
        /// </summary>
        /// <param name="node">The node whose grandparent is checked</param>
        /// <returns>Returns true or false depending on if is a function or a procedure</returns>
        private static bool IsGrandparentNotFuncOrProc(Node node)
        {
            Node GrandParent = node.Parent().Parent();
            return !(GrandParent is AProcDecl || GrandParent is AFuncDecl);
        }
    }
}
