using GOATCode.node;

namespace GOAT_Compiler.Exceptions
{
    public class RefNotFoundException : CompilerException
    {
        public RefNotFoundException(Node n, string SymbolName) : base(n, $"Symbol named: {SymbolName} not found")
        {
        }
    }
}