using GOATCode.node;

namespace GOAT_Compiler.Exceptions
{
    public class NotAllPathsReturnException : CompilerException
    {
        public NotAllPathsReturnException(Node n, string functionName) : base(n, $"Not all paths return in function {functionName}")
        {
        }
    }
}