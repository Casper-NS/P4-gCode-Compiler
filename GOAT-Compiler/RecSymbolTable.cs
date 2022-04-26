using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    class RecSymbolTable : ISymbolTable
    {
        private Table tables;
        private Table globalScope;
        private bool buildComplete = false;
        private Table currentScope;

        public RecSymbolTable() 
        {
            tables = new Table(null);
            globalScope = new Table(tables);
            tables.ChildrenTables.Add(globalScope);
            currentScope = tables;
        }

        public void OpenScope()
        {
            if (currentScope.ParentTable == null)
            {
                currentScope = currentScope.ChildrenTables[0];
            }
            else
            {
                int visitcount = currentScope.VisitCounter;
                currentScope.VisitCounter++;

                if (buildComplete)
                {
                    currentScope = currentScope.ChildrenTables[visitcount];
                }
                else
                {
                    currentScope.ChildrenTables.Add(new Table(currentScope));
                    currentScope = currentScope.ChildrenTables[visitcount];

                }
            }
        }

        public void CloseScope()
        {
            if (currentScope.ParentTable == null)
            {
                buildComplete = true;
            }
            currentScope.VisitCounter = 0;
            currentScope = currentScope.ParentTable;
        }

        public Symbol GetSymbol(string Name)
        {
            return currentScope.GetSymbol(Name);
        }

        public void AddSymbol(string Name, Types type)
        {
            currentScope.SetSymbol(Name, type);
        }

    }

    class Table
    {
        public Table ParentTable { get; private set; }
        public List<Table> ChildrenTables { get; set; }
        private readonly Dictionary<string, Symbol> Symbols;

        public int VisitCounter { get; set; }

        public Table(Table Parent)
        {
            Symbols = new Dictionary<string, Symbol>();
            ChildrenTables = new List<Table>();
            ParentTable = Parent;
            VisitCounter = 0;
        }

        public Symbol GetSymbol(string Name)
        {
            if (Symbols.TryGetValue(Name, out Symbol symbol))
            {
                return symbol;
            }
            else if(ParentTable != null)
            {
                return ParentTable.GetSymbol(Name);
            }

            return null;
        }

        public void SetSymbol(string Name, Types type)
        {
            Symbols.Add(Name, new Symbol(Name, type));
        }

    }
}
