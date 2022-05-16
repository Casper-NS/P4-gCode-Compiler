using GOATCode.node;

namespace GOAT_Compiler.Exceptions
{
    public class AssignConstException : CompilerException
    {
        public AssignConstException(Node node, string msg) : base(node, msg)
        {
        }
    }
}