using GOATCode.lexer;
using GOATCode.node;
using GOATCode.parser;
using System.IO;

namespace PrettyPrintATestFile
{
    internal static class PrettyPrintCorrectFile
    {
        private static string GetFilePath()
        {
            foreach (string filePath in Directory.GetFiles("../../../FileToPrettyPrint"))
            {
                return filePath;
            }
            return null;
        }

        public static void Print()
        {
            string filePath = PrettyPrintCorrectFile.GetFilePath();
            StreamReader reader = new StreamReader("../../../../SymbolTableTest/ScopeTestFiles/RefNotFoundTest-var.txt");
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Start s = p.Parse();
            TextPrinter printer = new TextPrinter();
            //printer.SetColor(true);
            s.Apply(printer);
        }
    }
}