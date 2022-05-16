using System;

namespace GOAT_Compiler
{
    internal class PopException : Exception
    {
        internal PopException(Extrude e)
        {
            Console.WriteLine("Cannot pop " + e);
        }
    }
}