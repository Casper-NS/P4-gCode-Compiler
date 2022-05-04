using GOATCode.node;

namespace GOAT_Compiler
{
    internal class StaticCallBuildInWalkException : CompilerException
    {
        public StaticCallBuildInWalkException(Node node, string message) : base(node, message)
        {
        }
    }
}
