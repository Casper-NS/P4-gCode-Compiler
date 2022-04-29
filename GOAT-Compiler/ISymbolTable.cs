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
        /// Gets the closest (scopewise) variable symbol from the symboltable
        /// </summary>
        /// <param name="Name">The name of the variable symbol you are trying to get</param>
        /// <returns>The symbol requested from the closest scope that contains the variable symbol.
        /// If the symbol is not found, null is returned</returns>
        public Symbol GetVariableSymbol(string name);

        /// <summary>
        /// Gets the closest (scopewise) function symbol from the symboltable
        /// </summary>
        /// <param name="Name">The name of the function symbol you are trying to get</param>
        /// <returns>The symbol requested from the closest scope that contains the function symbol.
        /// If the symbol is not found, null is returned</returns>
        public Symbol GetFunctionSymbol(string name);


        /// <summary>
        /// Adds a variable symbol to the symbol table.
        /// If the same variable symbol exists in an outer scope it is temporarily removed from the table and stored in the new variable symbol.
        /// In case of a duplicate variable symbol an exception is thrown.
        /// </summary>
        /// <param name="Name">The name of the variable symbol</param>
        /// <param name="Type">The type of the variable symbol</param>
        /// <exception cref="ArgumentException">Exception that is thrown if there is a duplicate definition.</exception>
        public void AddVariableSymbol(string name, Types type);

        /// <summary>
        /// Adds a function symbol to the symbol table.
        /// If the same function symbol exists in an outer scope it is temporarily removed from the table and stored in the new function symbol.
        /// In case of a duplicate symbol an exception is thrown.
        /// </summary>
        /// <param name="Name">The name of the function symbol</param>
        /// <param name="Type">The type of the function symbol</param>
        /// <param name="paramTypes">The type of the formal parameters in case it is a function being added</param>
        /// <exception cref="ArgumentException">Exception that is thrown if there is a duplicate definition.</exception>
        public void AddFunctionSymbol(string name, Types type, params Types[] paramTypes);


        /// <summary>
        /// Checks whether the symbol table has been build.
        /// </summary>
        /// <returns>Returns a bool, true if the table has been build and false otherwise</returns>
        public bool IsComplete();
    }
}