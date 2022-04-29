using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    class RecSymbolTable : ISymbolTable
    {
        private Table tables;
        private Table globalScope;
        private Table currentScope;

        //Flag to determine whether we are building or going through the symbol table.
        private bool buildComplete = false;

        public RecSymbolTable() 
        {
            tables = new Table(null);
            globalScope = new Table(tables);
            tables.ChildrenTables.Add(globalScope);
            currentScope = tables;
        }

        public void OpenScope()
        {
            //Handles the edge case of always opening the globalScope on the first open.
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
            currentScope.VisitCounter = 0;
            currentScope = currentScope.ParentTable;

            if (currentScope.ParentTable == null)
            {
                buildComplete = true;
            }

        }

        public Symbol GetSymbol(string Name)
        {
            return currentScope.GetSymbol(Name);
        }

        public void AddSymbol(string Name, Types type, params Types[] paramTypes)
        {
            currentScope.SetSymbol(Name, type, paramTypes);
        }

        public bool IsComplete()
        {
            return buildComplete;
        }

    }

    class Table
    {
        public Table ParentTable { get; private set; }
        public List<Table> ChildrenTables { get; set; }
        private readonly Dictionary<string, Symbol> Symbols;

        /// <summary>
        /// Keeps track of which tables have been visited, to ensure they are visited in order.
        /// </summary>
        public int VisitCounter { get; set; }

        /// <summary>
        /// Constructs a table and sets its parent to the given table
        /// </summary>
        /// <param name="Parent">Parent table</param>
        public Table(Table Parent)
        {
            Symbols = new Dictionary<string, Symbol>();
            ChildrenTables = new List<Table>();
            ParentTable = Parent;
            VisitCounter = 0;
        }

        /// <summary>
        /// Gets the given symbol from the current table if its not found it checks the parent tables.
        /// </summary>
        /// <param name="Name">Name of the symbol you want to get</param>
        /// <returns>Returns the requested symbol</returns>
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

        public void SetSymbol(string Name, Types type, params Types[] paramTypeArray)
        {
            Symbols.Add(Name, new Symbol(Name, type, paramTypeArray));
        }

    }
}
