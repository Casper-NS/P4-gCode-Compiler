using GOATCode.node;
using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal class RecSymbolTable : ISymbolTable
    {
        private Table _globalScope = null;
        private Table currentScope = null;

        /// <summary>
        /// The stack that keeps track of which scope we are in, the top of the stack is the current scope.
        /// </summary>
        private readonly Stack<Table> scopeStack = new();

        /// <summary>
        /// Maps from a node to a symboltable. Nodes that open a scope create a symbol-table.
        /// </summary>
        private readonly Dictionary<Node, Table> scopeMap = new();

        /// <summary>
        /// Maps from function id to the function symbol.
        /// </summary>
        private readonly Dictionary<string, Symbol> functionSymbols;

        /// <summary>
        /// Maps from function symbols to the node that declares the functions.
        /// Used to execute functions-calls in code-generator.
        /// </summary>
        private readonly Dictionary<Symbol, Node> funcDeclMap = new();

        /// <summary>
        /// Flag to determine whether we are building the symboltable or going through the symbol table.
        /// </summary>
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
                // On the first iteration this builds the symbol-table
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

        public Symbol GetVariableSymbol(string name) => currentScope.GetSymbol(name);

        public void AddVariableSymbol(string name, Types type) => currentScope.SetSymbol(name, type);

        public bool IsComplete() => buildComplete;

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

        public Node GetFunctionNode(Symbol symbol) => funcDeclMap.TryGetValue(symbol, out Node node) ? node : null;

        public bool IsGlobal(Symbol symbol) => (_globalScope.GetSymbol(symbol.Name) != null);
    }

    internal class Table
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
            else if (ParentTable != null)
            {
                return ParentTable.GetSymbol(Name);
            }
            return null;
        }

        /// <summary>
        /// Sets the given symbol in the current table.
        /// </summary>
        /// <param name="Name">Name of the symbol.</param>
        /// <param name="type">The type of the symbol.</param>
        /// <param name="paramTypeArray">X amount of formel parameters.</param>
        public void SetSymbol(string Name, Types type, params Types[] paramTypeArray) => Symbols.Add(Name, new Symbol(Name, type, paramTypeArray));
    }
}