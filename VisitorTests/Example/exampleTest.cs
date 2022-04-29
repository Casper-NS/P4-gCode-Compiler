using GOAT_Compiler;
using GOATCode.node;
using System;
using VisitorTests.Utilities;
using Xunit;

namespace VisitorTests.Example
{
    public class ExampleTest
    {
        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(CorrectTypeFilesEnumerator))]
        public void IsTypedCorrectly(string file)
        {
            /*Start s = FileReadingTestUtilities.ParseFile(file);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);

            TypeChecker typeChecker = new TypeChecker(symbolTable);
            s.Apply(typeChecker);
            */
        }
    }
    class CorrectTypeFilesEnumerator : BaseFilesEnumerator
    {
        public override string RelativeFolderPath() => "Example/iscool";
    }
}
