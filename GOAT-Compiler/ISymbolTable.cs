namespace GOAT_Compiler
{
    internal interface ISymbolTable
    {
        public void OpenScope();

        public void CloseScope();

        public Symbol GetSymbol(string Name);

        public void AddSymbol(string Name, Types Type);
    }
}