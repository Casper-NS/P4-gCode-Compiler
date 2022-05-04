using GOATCode.node;
using System;

namespace GOAT_Compiler.Exceptions
{
    public class FunctionAlreadyDefinedException : CompilerException
    {
        public FunctionAlreadyDefinedException(Node n, string Name) : base(n, $"The function named {Name} is already defined.") { }
    }
}