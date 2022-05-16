using System;

namespace VisitorTests.Utilities
{
    public class TestDependencyException : Exception
    {
        public TestDependencyException(Exception innerException) : this("", innerException)
        {
        }

        public TestDependencyException(string phase, Exception innerException) : base("Test dependency " + phase + " failed: \n" + innerException, innerException)
        {
        }
    }
}