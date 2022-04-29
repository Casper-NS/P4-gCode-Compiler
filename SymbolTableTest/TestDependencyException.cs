using System;

namespace SymbolTableTest
{
    public class TestDependencyException : Exception
    {
        public TestDependencyException(Exception innerException) : base("Test dependency failed", innerException)
        {

        }
    }
}
