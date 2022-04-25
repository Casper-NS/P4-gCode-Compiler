using GOATCode.lexer;
using GOATCode.node;
using GOATCode.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    class Program
    {
        static void main(string[] args)
        { 
            string filePath = "../../../SymbolTableBuilderTest/Test.txt";
            StreamReader reader = new StreamReader(filePath);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Start s = p.Parse();
            SymbolTableBuilder builder = new SymbolTableBuilder(new RecSymbolTable());
            s.Apply(builder);
            Console.WriteLine();
        }
    }
}
