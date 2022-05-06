using GOATCode.node;
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
        private Table _globalScope = null;
        private Table currentScope = null;

        private Stack<Table> scopeStack = new();
        
        private readonly Dictionary<Node, Table> scopeMap = new ();

        private readonly Dictionary<string, Symbol> functionSymbols;

        private readonly Dictionary<Symbol, Node> funcDeclMap = new();

        //Flag to determine whether we are building or going through the symbol table.
        private bool buildComplete = false;

        public RecSymbolTable() 
        {
            functionSymbols = new Dictionary<string, Symbol>(BuiltInFunctions.FunctionsList);
        }

        public void OpenScope(Node node)
        {
            scopeStack.Push(currentScope);
            if (buildComplete)
            {
                currentScope = scopeMap[node];
            }
            else
            {
                Table scope = new Table(currentScope);
                if (currentScope == null)
                {
                    _globalScope = scope;
                }
                scopeMap.Add(node, scope);
                currentScope = scope;
            }
        }

        public void CloseScope()
        {
            currentScope = scopeStack.Pop();
            if (currentScope == null)
            {
                buildComplete = true;
            }

        }

        public Symbol GetVariableSymbol(string name)
        {
            return currentScope.GetSymbol(name);
        }

        public void AddVariableSymbol(string name, Types type)
        {
            currentScope.SetSymbol(name, type);
        }

        public bool IsComplete()
        {
            return buildComplete;
        }

        public Symbol GetFunctionSymbol(string name)
        {
            if (functionSymbols.TryGetValue(name, out Symbol symbol))
            {
                return symbol;
            }
            return null;
        }

        public void AddFunctionSymbol(Node node, string name, Types returnType, params Types[] paramTypes)
        {
            Symbol sym = new Symbol(name, returnType, paramTypes);
            functionSymbols.Add(name, sym);
            funcDeclMap.Add(sym, node);
        }

        public Node GetFunctionNode(Symbol symbol)
        {
            return funcDeclMap.TryGetValue(symbol, out Node node) ? node : null;
        }

        public bool IsGlobal(Symbol symbol)
        {
            return (_globalScope.GetSymbol(symbol.name) != null);
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
