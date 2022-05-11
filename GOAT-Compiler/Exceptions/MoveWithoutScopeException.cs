using System;

namespace GOAT_Compiler.Exceptions
{
    public class MoveWithoutScopeException : Exception
    {
        public MoveWithoutScopeException(string msg) : base(msg)
        {
            
        }
    }
}