using GOATCode.node;

namespace GOAT_Compiler.Exceptions
{
    public class VariableAlreadyDefinedException : CompilerException
    {
        public VariableAlreadyDefinedException(Node n, string Name) : base(n, $"The variable named {Name} is already defined.")
        {
        }
    }
}