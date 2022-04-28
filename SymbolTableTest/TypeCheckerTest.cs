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
            Start s;
            ISymbolTable symbolTable;
            try
            {
                StreamReader reader = new StreamReader(file);
                Lexer l = new Lexer(reader);
                Parser p = new Parser(l);
                s = p.Parse();

                symbolTable = new RecSymbolTable();
                SymbolTableBuilder symbolTableBuilder = new SymbolTableBuilder(symbolTable);

                s.Apply(symbolTableBuilder);
            }
            catch (Exception e)
            {

                throw new TestDependencyException(e);
            }
            TypeChecker typeChecker = new TypeChecker(symbolTable);
            s.Apply(typeChecker);
        }

        [Theory]
        [ClassData(typeof(WrongFilesEnumerator))]
        public void IsTypedIncorrectly(string file)
        {
            Start s;
            ISymbolTable symbolTable;
            try
            {
                StreamReader reader = new StreamReader(file);
                Lexer l = new Lexer(reader);
                Parser p = new Parser(l);
                s = p.Parse();

                symbolTable = new RecSymbolTable();
                SymbolTableBuilder symbolTableBuilder = new SymbolTableBuilder(symbolTable);

                s.Apply(symbolTableBuilder);
            }
            catch (Exception e)
            {

                throw new TestDependencyException(e);
            }
            TypeChecker typeChecker = new TypeChecker(symbolTable);
            Assert.Throws<TypeMismatchException>(() => s.Apply(typeChecker));
        }

        [Theory]
        [InlineData("int a = 3.0")]
        [InlineData("float a = true + 2.3")]
        [InlineData("int a\n a += 1.2")]
        [InlineData("int a\n a += (1.0, 1.0, 1.0)")]
        [InlineData("int a\n a += true")]
        [InlineData("float a\n a *= (1.0, 1.0, 1.0)")]
        [InlineData("vector a = true")]
        [InlineData("vector a = 1")]
        [InlineData("vector a = 1.0")]
        [InlineData("int a = (5, 1 * 3.4 * 2 / 21 + 5 % 2, 0)")]
        [InlineData("int a = ((0,0,0))")]
        [InlineData("float a = (0,0,0)")]
        [InlineData("int a = (0.0)")]
        [InlineData("int a = 2 > 3")]

        public void IsStatementTypedInCorrectly(string stmt)
        {
            string program = "void main() \n{ \n " + stmt + " \n}";
            Start s;
            ISymbolTable symbolTable;
            try
            {
                StringReader reader = new StringReader(program);
                Lexer l = new Lexer(reader);
                Parser p = new Parser(l);
                s = p.Parse();

                symbolTable = new RecSymbolTable();
                SymbolTableBuilder symbolTableBuilder = new SymbolTableBuilder(symbolTable);

                s.Apply(symbolTableBuilder);
            }
            catch (Exception e)
            {

                throw new TestDependencyException(e);
            }
            TypeChecker typeChecker = new TypeChecker(symbolTable);
            Assert.Throws<TypeMismatchException>(() => s.Apply(typeChecker));
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
        private class WrongFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var filePath in GetTestFilesRecursively("../../../TypesWrong"))
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
