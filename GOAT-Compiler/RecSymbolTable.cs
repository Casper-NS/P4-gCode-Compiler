using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    class RecSymbolTable
    {

        private Table tables;
        private Table currentScope;

        public RecSymbolTable()
        {
            tables = new Table(null);
            currentScope = tables;
        }

        public void OpenScope()
        {
            int visitcount = currentScope.VisitCounter;
            currentScope.VisitCounter++;

            if (currentScope.ChildrenTables.Count <= visitcount)
            {
                currentScope = currentScope.ChildrenTables[tables.VisitCounter];
            }
            else
            {
                currentScope.ChildrenTables.Add(new Table(tables));
            }
        }

        public void CloseScope()
        {
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

            if (Symbols.TryGetValue(Name, out Symbol symbol))
            {
                throw new Exception($"Duplicate definition of {Name}");
            }

            Symbols.Add(Name, symbol);
        }

    }
}
