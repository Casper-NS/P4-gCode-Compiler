using GOAT_Compiler;
using GOATCode.lexer;
using GOATCode.node;
using GOATCode.parser;
using System;
using System.IO;
using Xunit;

namespace ExtruderCheckerTest
{
    public class ExtruderCheckerTest
    {
        [Fact]
        public void Test1()
        {
            string filePath = "../../../ExtruderCheckerTests/PushErrorTest.txt";
            StreamReader reader = new StreamReader(filePath);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Start s = p.Parse();
            ISymbolTable symbolTable = new RecSymbolTable();
            ExtruderChecker checker = new ExtruderChecker(symbolTable);

            Assert.Throws<PushException>(()=> s.Apply(checker));
        }
    }
}
