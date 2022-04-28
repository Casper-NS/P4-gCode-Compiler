using System;

namespace GOAT_Compiler.Exceptions
{
    public class RefUsedBeforeClosestDeclException : Exception
    {
        public RefUsedBeforeClosestDeclException(string name) : base($"The closest declaration of variable: {name} is made after it is referenced")
        {
            
        }
    }
}