using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAT_Compiler;
using GOAT_Compiler.Exceptions;
using GOATCode.lexer;
using GOATCode.node;
using GOATCode.parser;
using Xunit;

namespace SymbolTableTest
{
    public class ScopeCheckTest
    {

        [Fact]
        public void Test_Refs_In_Testfile()
        {
            ISymbolTable symTab = new RecSymbolTable();
            StreamReader reader = new StreamReader("../../../../SymbolTableTest/SymbolBuildTest.txt");
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            SymbolTableBuilder builder = new SymbolTableBuilder(symTab);
            ScopeChecker scopeChecker = new ScopeChecker(symTab);
            Start s = p.Parse();
            s.Apply(builder);
            s.Apply(scopeChecker);
        }

        [Theory]
        [ClassData(typeof(RefBeforeDeclFilesEnumerator))]
        public void Test_RefBeforeDecl_Exception(string filePath)
        {
            ISymbolTable symTab = new RecSymbolTable();
            StreamReader reader = new StreamReader(filePath);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            SymbolTableBuilder builder = new SymbolTableBuilder(symTab);
            ScopeChecker scopeChecker = new ScopeChecker(symTab);
            Start s = p.Parse();
            s.Apply(builder);
            Assert.Throws<RefUsedBeforeClosestDeclException>(() => s.Apply(scopeChecker));
        }

        [Theory]
        [ClassData(typeof(RefNotFoundFilesEnumerator))] 
        public void Test_RefNotFound_Exception(string filepath)
        {
            ISymbolTable symTab = new RecSymbolTable();
            StreamReader reader = new StreamReader(filepath);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            SymbolTableBuilder builder = new SymbolTableBuilder(symTab);
            ScopeChecker scopeChecker = new ScopeChecker(symTab);
            Start s = p.Parse();
            s.Apply(builder);
            Assert.Throws<RefNotFoundException>(() => s.Apply(scopeChecker));
        }

        [Theory]
        [ClassData(typeof(NotInitializedFilesEnumerator))]
        public void Test_VarNotInitialized_Exception(string filepath)
        {
            ISymbolTable symTab = new RecSymbolTable();
            StreamReader reader = new StreamReader(filepath);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            SymbolTableBuilder builder = new SymbolTableBuilder(symTab);
            ScopeChecker scopeChecker = new ScopeChecker(symTab);
            Start s = p.Parse();
            s.Apply(builder);
            Assert.Throws<VarNotInitializedException>(() => s.Apply(scopeChecker));
        }


        // ------------------------------------------------------------------------------------------------------------
        // Classes for getting test files
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

        private class RefBeforeDeclFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var filePath in GetTestFilesRecursively("../../../../SymbolTableTest/ScopeTestFiles/RefBeforeDecl"))
                {
                    yield return filePath;
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class RefNotFoundFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var filePath in GetTestFilesRecursively("../../../../SymbolTableTest/ScopeTestFiles/RefNotFound"))
                {
                    yield return filePath;
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class NotInitializedFilesEnumerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var filePath in GetTestFilesRecursively("../../../../SymbolTableTest/ScopeTestFiles/VarNotInitialized"))
                {
                    yield return filePath;
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

    }
}
