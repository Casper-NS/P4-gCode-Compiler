using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Linq;

namespace GOAT_Compiler
{
    enum Types
    {
        integer,
        floatingPoint,
        vector,
        boolean
    };

    /*
    class SymbolTable
    {
        private int depth = 0;
        private Dictionary<int, SymbolData> symbolTable = new Dictionary<int, SymbolData>();
        private List<List<SymbolData>> ScopeDisplay = new List<List<SymbolData>>();

        
        public SymbolTable()
        {
            ScopeDisplay.Add(null);
        }

        void OpenScope()
        {
            depth = depth++;
            ScopeDisplay.Add(null);
        }

        void CloseScope()
        {
            SymbolData previousSymbol;
            foreach (var symbol in ScopeDisplay[depth])
            {
                previousSymbol = symbol.previousSymbol;
            }
        }

        SymbolData GetSymbol(string Name)
        {
            SymbolData symbol = ScopeDisplay[depth].Find(sym => sym.name == Name);
            symbolTable.TryGetValue(symbol.GetHashCode(), out symbol);
            return symbol;
        }


        void AddSymbol(string Name, Types Type)
        {
            SymbolData oldSymbol = GetSymbol(Name);
            if (oldSymbol != null && oldSymbol.depth == depth)
            {
                throw new Exception("Duplicate definition");
            }

            SymbolData newSymbol = new SymbolData(Name, Type);

            ScopeDisplay[depth].Add(newSymbol);
            //newSymbol.level = ScopeDisplay[depth];
            newSymbol.depth = depth;

            if (oldSymbol == null)
            {
                symbolTable.Add(newSymbol.GetHashCode(), newSymbol);
            }
            else
            {
                symbolTable.Remove(oldSymbol.GetHashCode());
                symbolTable.Add(newSymbol.GetHashCode(), newSymbol);
            }

            newSymbol.previousSymbol = oldSymbol;
        }
    }
        */

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

        public SymbolData GetSymbol(string Name)
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
        private readonly Dictionary<string, SymbolData> Symbols;

        public int visitCounter
        {
            get => visitCounter++;
            set { visitCounter = value; }
        }

        public Table(Table Parent)
        {
            Symbols = new Dictionary<string, SymbolData>();
            ParentTable = Parent;
            visitCounter = 0;
        }

        public SymbolData GetSymbol(string Name)
        {
            return Symbols[Name];
        }

        public void SetSymbol(string Name, Types type)
        {
            Symbols.Add(Name, new SymbolData(Name, type));
        }

    }


    class SymbolData
    {
        public string name { get; private set; }
        public Types type { get; private set; }
        //public List<SymbolData> level { get; set; }
        //public int depth { get; set; }
        //public SymbolData previousSymbol { get; set; }

        public SymbolData(string Name, Types Type)
        {
            name = Name;
            type = Type;
        }

    }
}
