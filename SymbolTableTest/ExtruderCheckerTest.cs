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

        public Start MakeStartNode()
        {
            string filePath = "../../../ExtruderCheckerTests/PushErrorTest.txt";
            StreamReader reader = new StreamReader(filePath);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Start s = p.Parse();
            return s;
            
        }
        internal static ExtruderChecker MakeExtruderChecker() => new(new RecSymbolTable());

        [Fact]
        public void Test1()
        {
            Start s = MakeStartNode();
            ExtruderChecker checker = MakeExtruderChecker();

            Assert.Throws<PushException>(()=> s.Apply(checker));
        }
    }
}
