using System;

namespace GOAT_Compiler
{
    internal interface ISymbolTable
    {
        /// <summary>
        /// Opens a new scope and extends the scope display if neccesary.
        /// </summary>
        public void OpenScope();

        /// <summary>
        /// Closes the current scope and removes the scopes symbol from the table.
        /// If the symbols contain references to outerscope symbols they're reinserted into the table.
        /// </summary>
        public void CloseScope();

        /// <summary>
        /// Gets the closest (scopewise) symbol from the symboltable
        /// </summary>
        /// <param name="Name">The name of the symbol you are trying to get</param>
        /// <returns>The symbol requested from the closest scope that contains the symbol.
        /// If the symbol is not found, null is returned</returns>
        public Symbol GetSymbol(string Name);

        /// <summary>
        /// Adds a symbol to the symbol table.
        /// If the same symbol exists in an outer scope it is temporarily removed from the table and stored in the new symbol.
        /// In case of a duplicate symbol an exception is thrown.
        /// </summary>
        /// <param name="Name">The name of the symbol</param>
        /// <param name="Type">The type of the symbol</param>
        /// <exception cref="ArgumentException">Exception that is thrown if there is a duplicate definition.</exception>
        public void AddSymbol(string Name, Types Type);

        public bool IsComplete();
    }
}