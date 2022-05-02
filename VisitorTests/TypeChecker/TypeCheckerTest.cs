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
using VisitorTests;
using VisitorTests.Utilities;
using Xunit;

namespace SymbolTableTest
{
    public class TypeCheckerTest
    {
        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(CorrectFilesEnumerator))]
        public void IsTypedCorrectly(string file)
        {
            Start s = FileReadingTestUtilities.ParseFile(file);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);

            TypeChecker typeChecker = new TypeChecker(symbolTable);
            s.Apply(typeChecker);
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(WrongFilesEnumerator))]
        public void IsTypedIncorrectly(string file)
        {
            Start s = FileReadingTestUtilities.ParseFile(file);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);

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
        [InlineData("if (1) { int a }")]
        [InlineData("if (233 + 2) { int a }")]
        [InlineData("while (1) { int a }")]
        [InlineData("repeat (true) { int a }")]
        [InlineData("repeat (32 > 2) { int a }")]
        public void IsStatementTypedInCorrectly(string stmt)
        {
            string program = "void main() \n{ \n " + stmt + " \n}";
            Start s = FileReadingTestUtilities.ParseString(program);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);

            TypeChecker typeChecker = new TypeChecker(symbolTable);
            Assert.Throws<TypeMismatchException>(() => s.Apply(typeChecker));
        }

        private class CorrectFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "TypeChecker/TypesOK";
        }
        private class WrongFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "TypeChecker/TypesWrong";
        }
    }
}
