using GOAT_Compiler;
using GOATCode.lexer;
using GOATCode.node;
using GOATCode.parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SymbolTableTest
{
    public class TypeCheckerTest
    {
        [Theory]
        [ClassData(typeof(CorrectFilesEnumerator))]
        public void IsTypedCorrectly(string file)
        {
            StreamReader reader = new StreamReader(file);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Start s = p.Parse();

            ISymbolTable symbolTable = new RecSymbolTable();
            SymbolTableBuilder symbolTableBuilder = new SymbolTableBuilder(symbolTable);

            s.Apply(symbolTableBuilder);

            TypeChecker typeChecker = new TypeChecker(symbolTable);
            s.Apply(typeChecker);
        }
        private class CorrectFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var filePath in GetTestFilesRecursively("../../../TypesOK"))
                {
                    yield return filePath;
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

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
    }
}
