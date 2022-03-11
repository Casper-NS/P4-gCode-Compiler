using GGCodeParser.lexer;
using GGCodeParser.parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ParserTester
{
    public class Tests
    {
        private class CorrectFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (string filePath in Directory.GetFiles("../../../CorrectFiles"))
                {
                    yield return new object[] { filePath }; 
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        private class LexerFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (string filePath in Directory.GetFiles("../../../LexerError"))
                {
                    yield return new object[] { filePath };
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        private class ParserFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (string filePath in Directory.GetFiles("../../../ParserError"))
                {
                    yield return new object[] { filePath };
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        [Theory]
        [ClassData(typeof(CorrectFilesEnumerator))]
        public void FileParsesSuccesfully(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            p.Parse();
        }

        [Theory]
        [ClassData(typeof(LexerFilesEnumerator))]
        public void FileLexerError(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Assert.Throws<LexerException>(() => p.Parse());
        }
        [Theory]
        [ClassData(typeof(ParserFilesEnumerator))]
        public void FileParserError(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Assert.Throws<ParserException>(() => p.Parse());
        }
    }
}
