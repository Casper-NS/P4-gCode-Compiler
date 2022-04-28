using System;

namespace GOAT_Compiler.Exceptions
{
    public class VarNotInitializedException : Exception
    {
        public VarNotInitializedException(string Name) : base($"The variable named {Name} has not been initialized.")
        {
            
        }
    }
}