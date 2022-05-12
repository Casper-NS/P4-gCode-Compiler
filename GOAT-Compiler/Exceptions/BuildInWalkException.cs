using GOATCode.node;

namespace GOAT_Compiler.Exceptions
{
    public class BuildInWalkException : CompilerException
    {
        public BuildInWalkException(Node node, string msg) : base(node, msg)
        {
        }
    }
}