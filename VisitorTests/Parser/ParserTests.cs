using GOATCode.lexer;
using GOATCode.parser;
using System.IO;
using VisitorTests;
using Xunit;

namespace ParserTester
{
    /// <summary>
    /// The class runs the tests that checks sableCC.
    /// </summary>
    public class ParserTests
    {
        // Tests if all files in the CorrectFiles folder can be parsed
        [SkippableTheory]
        [ClassData(typeof(CorrectFilesEnumerator))]
        public void ParseOK(string file)
        {
            StreamReader reader = new StreamReader(file);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            p.Parse();
        }

        // Tests if all files in the LexerError folder throws lexer exceptions
        [SkippableTheory]
        [ClassData(typeof(LexerFilesEnumerator))]
        public void LexErr(string file)
        {
            StreamReader reader = new StreamReader(file);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Assert.Throws<LexerException>(() => p.Parse());
        }

        // Tests if all files in the ParserError folder throws lexer exceptions
        [SkippableTheory]
        [ClassData(typeof(ParserFilesEnumerator))]
        public void ParseErr(string file)
        {
            StreamReader reader = new StreamReader(file);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Assert.Throws<ParserException>(() => p.Parse());
        }

        private class CorrectFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "Parser/ParseOK";
        }

        private class LexerFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "Parser/LexErr";
        }

        private class ParserFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "Parser/ParseErr";
        }
    }
}