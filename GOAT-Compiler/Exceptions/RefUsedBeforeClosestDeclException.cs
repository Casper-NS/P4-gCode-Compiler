using GOATCode.node;
using System;

namespace GOAT_Compiler.Exceptions
{
    public class RefUsedBeforeClosestDeclException : CompilerException
    {
        public RefUsedBeforeClosestDeclException(Node n, string name) : base(n, $"The closest declaration of variable: {name} is made after it is referenced")
        {
            
        }
    }
}