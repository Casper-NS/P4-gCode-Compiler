using System;

namespace GOAT_Compiler
{
    internal class PushException : Exception
    {
        internal PushException(Extrude e)
        {
            Console.WriteLine("Cannot push " + e);
        }
    }

}
