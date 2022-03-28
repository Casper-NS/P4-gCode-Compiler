using System;
using GOATCode.parser;
using GOATCode.analysis;
using GOATCode.lexer;
using GOATCode.node;

namespace PrettyPrintATestFile
{
    internal partial class Program
    {
        private static void Main(string[] args)
        {
            // Insert the name of the file from the CorrectFiles folder you wish to pretty-print
            PrettyPrintCorrectFile.Print();

        }
    }
}