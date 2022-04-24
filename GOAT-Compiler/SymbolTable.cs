using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Linq;

namespace GOAT_Compiler
{
    public enum Types
    {
        integer,
        floatingPoint,
        vector,
        boolean
    };


    class SymbolTable : ISymbolTable
    {
        private int depth = 0;
        private Dictionary<string, Symbol> symbolTable = new Dictionary<string, Symbol>();
        private List<Symbol> ScopeDisplay = new List<Symbol>();

        
        public SymbolTable()
        {
            ScopeDisplay.Add(null);
        }

        /// <summary>
        /// Opens a new scope and extends the scope display if neccesary.
        /// </summary>
        public void OpenScope()
        {
            depth++;
            if (ScopeDisplay.Count < depth)
            {
                ScopeDisplay.Add(null);
            }
        }

        /// <summary>
        /// Closes the current scope and removes the scopes symbol from the table.
        /// If the symbols contain references to outerscope symbols they're reinserted into the table.
        /// </summary>
        public void CloseScope()
        {
            Symbol symbol = ScopeDisplay[depth];
            Symbol previousSymbol;
            while (symbol.level != null)
            {
                previousSymbol = symbol.OuterSymbol;
                symbolTable.Remove(symbol.name);
                if (previousSymbol != null)
                {
                    symbolTable.Add(symbol.name, symbol);
                }
                symbol = symbol.level;
            }
            depth--;
        }

        /// <summary>
        /// Gets the closest (scopewise) symbol from the symboltable
        /// </summary>
        /// <param name="Name">The name of the symbol you are trying to get</param>
        /// <returns></returns>
        public Symbol GetSymbol(string Name)
        {
            if (symbolTable.TryGetValue(Name, out Symbol symbol))
            {
                return symbol;
            }
            else return null;
        }

        /// <summary>
        /// Adds a symbol to the symbol table.
        /// If the same symbol exists in an outer scope it is temporarily removed from the table and stored in the new symble.
        /// In case of a duplecate symbol an exception is thrown.
        /// </summary>
        /// <param name="Name">The name of the symbol</param>
        /// <param name="Type">The type of the symbol</param>
        /// <exception cref="Exception">Exception that is thrown if there is a duplicate definition.</exception>
        public void AddSymbol(string Name, Types Type)
        {
            Symbol oldSymbol = GetSymbol(Name);
            if (oldSymbol != null && oldSymbol.depth == depth)
            {
                throw new Exception($"Duplicate definition of {Name}");
            }

            Symbol newSymbol = new Symbol(Name, Type);

            newSymbol.level = ScopeDisplay[depth];
            newSymbol.depth = depth;
            ScopeDisplay[depth] = (newSymbol);

            if (oldSymbol == null)
            {
                symbolTable.Add(newSymbol.name, newSymbol);
            }
            else
            {
                symbolTable.Remove(oldSymbol.name);
                symbolTable.Add(newSymbol.name, newSymbol);
            }

            newSymbol.OuterSymbol = oldSymbol;
        }
    }

    /*
    class SymbolTableRecursive
    {
        private Table tables;
        private Table currentScope;

        public SymbolTableRecursive()
        {
            tables = new Table(null);
            currentScope = tables;
        }

        public void OpenScope()
        {
            currentScope = currentScope.ChildrenTables[tables.visitCounter];
        }

        public void CreateScope()
        {
            currentScope.ChildrenTables.Add(new Table(tables));
        }

        public void CloseScope()
        {
            currentScope = currentScope.ParentTable;
            currentScope.visitCounter = 0;
        }

        public Symbol GetSymbol(string Name)
        {
            return currentScope.GetSymbol(Name);
        }

        public void SetSymbol(string Name, Types type)
        {
            currentScope.SetSymbol(Name, type);
        }

    }

    class Table
    {
        public Table ParentTable { get; private set; }
        public List<Table> ChildrenTables { get; set; }
        private readonly Dictionary<string, Symbol> Symbols;

        public int visitCounter
        {
            get => visitCounter++;
            set { visitCounter = value; }
        }

        public Table(Table Parent)
        {
            Symbols = new Dictionary<string, Symbol>();
            ParentTable = Parent;
            visitCounter = 0;
        }

        public Symbol GetSymbol(string Name)
        {
            return Symbols[Name];
        }

        public void SetSymbol(string Name, Types type)
        {
            Symbols.Add(Name, new Symbol(Name, type));
        }

    }
    */

    internal class Symbol
    {
        public string name { get; private set; }
        public Types type { get; private set; }

        //level points to the previous symbol in the scope and is used for deleting symbols when exiting a scope.
        public Symbol level { get; set; }


        // depth is used for checking if a duplicate symbol exists in the scope
        public int depth { get; set; }

        // OuterSymbol refers to the symbol with the same name in the next outer scope. 
        // This is used to reinsert symbols into the dictionary when symbols are deleted.
        public Symbol OuterSymbol { get; set; }

        //Used to retrieve symbols in cases where multiple symbols map to the same hashvalue.
        //Seems unnecceary due to c# dictionaries handling collisions.
        //May need to be implemented in the future when we can do some testing.
        //public Symbol Hash { get;}

        public Symbol(string Name, Types Type)
        {
            name = Name;
            type = Type;
            level = null;
        }

    }
}
