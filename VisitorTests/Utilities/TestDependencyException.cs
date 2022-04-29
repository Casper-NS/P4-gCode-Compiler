using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
