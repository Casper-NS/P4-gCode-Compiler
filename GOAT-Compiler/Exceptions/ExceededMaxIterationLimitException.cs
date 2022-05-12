using GOATCode.node;
using System;

namespace GOAT_Compiler.Exceptions
{
    public class ExceededMaxIterationLimitException : CompilerException
    {
        public ExceededMaxIterationLimitException(Node n) : base(n, "Exceeded the max amount of iterations.")
        {
        }
    }
}