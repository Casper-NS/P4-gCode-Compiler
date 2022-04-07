using GOATCode.lexer;
using GOATCode.parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Xunit.Sdk;

namespace ParserTester
{
    public class Tests
    {
        // Tests if all files in the CorrectFiles folder can be parsed
        [Theory]
        [ClassData(typeof(CorrectFilesEnumerator))]
        public void ParseOK(string file)
        {
            StreamReader reader = new StreamReader(file);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            p.Parse();
        }

        // Tests if all files in the LexerError folder throws lexer exceptions
        [Theory]
        [ClassData(typeof(LexerFilesEnumerator))]
        public void LexErr(string file)
        {
            StreamReader reader = new StreamReader(file);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Assert.Throws<LexerException>(() => p.Parse());
        }
        // Tests if all files in the ParserError folder throws lexer exceptions
        [Theory]
        [ClassData(typeof(ParserFilesEnumerator))]
        public void ParseErr(string file)
        {
            StreamReader reader = new StreamReader(file);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Assert.Throws<ParserException>(() => p.Parse());
        }

        // -------- All the classes below are used to give enumerators of the correct folders for the tests --------------

        private static IEnumerable<object[]> GetTestFilesRecursively(string directoryPath)
        {
            // Get all files in root directory
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                yield return new object[] { filePath };
            }
            // get all subdirectories
            foreach (string subdirectoryPath in Directory.GetDirectories(directoryPath))
            {
                // get everything from current subdirectory
                foreach (var file in GetTestFilesRecursively(subdirectoryPath))
                {
                    yield return file;
                }
            }

        }


        private class CorrectFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var filePath in GetTestFilesRecursively("../../../ParseOK"))
                {
                    yield return filePath;
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        private class LexerFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var filePath in GetTestFilesRecursively("../../../LexErr"))
                {
                    yield return filePath;
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        private class ParserFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var filePath in GetTestFilesRecursively("../../../ParseErr"))
                {
                    yield return filePath;
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
