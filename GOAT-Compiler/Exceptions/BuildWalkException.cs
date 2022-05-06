using GOATCode.node;

namespace GOAT_Compiler.Exceptions
{
    public class BuildWalkException : CompilerException
    {
        public BuildWalkException(Node node, string msg) : base(node, msg)
        {
        }
    }
}