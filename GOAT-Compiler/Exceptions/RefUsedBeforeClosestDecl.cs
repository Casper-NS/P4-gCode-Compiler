using System;

namespace GOAT_Compiler.Exceptions
{
    public class RefUsedBeforeClosestDecl : Exception
    {
        public RefUsedBeforeClosestDecl(string name) : base($"The closest declaration of variable: {name} is made after it is referenced")
        {
            
        }
    }
}