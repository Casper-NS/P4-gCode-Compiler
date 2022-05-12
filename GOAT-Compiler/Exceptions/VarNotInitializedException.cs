using GOATCode.node;
using System;

namespace GOAT_Compiler.Exceptions
{
    public class VarNotInitializedException : CompilerException
    {
        public VarNotInitializedException(Node n, string Name) : base(n, $"The variable named {Name} has not been initialized.")
        {
        }
    }
}