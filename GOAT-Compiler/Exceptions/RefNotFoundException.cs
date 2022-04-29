using System;
using System.ComponentModel;

namespace GOAT_Compiler.Exceptions
{
    public class RefNotFoundException : Exception
    {
        public RefNotFoundException(string SymbolName) : base($"Symbol named: {SymbolName} not found")
        {

        }
    }
}