using GOAT_Compiler;
using GOATCode.node;
using VisitorTests;
using VisitorTests.Utilities;
using Xunit;

namespace ExtruderCheckerTest
{
    public class ExtruderCheckerTest
    {
        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(PushErrorThrownFilesEnumerator))]
        public void PushErrorThrown(string filePath)
        {
            Start s = FileReadingTestUtilities.ParseFile(filePath);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            ExtruderChecker checker = new ExtruderChecker(symbolTable);

            Assert.Throws<StaticCallBuildInWalkException>(() => s.Apply(checker));
        }

        private class PushErrorThrownFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "ExtruderChecker/PushFilesError";
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(ExtruderCheckerRunsFilesEnumerator))]
        public void ExtruderCheckerRunsCorrectly(string filePath)
        {
            Start s = FileReadingTestUtilities.ParseFile(filePath);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            ExtruderChecker checker = new ExtruderChecker(symbolTable);

            s.Apply(checker);
        }

        private class ExtruderCheckerRunsFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "ExtruderChecker/RunsCorrectly";
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(ExtruderCheckerThrowsExtruderExceptionFilesEnumerator))]
        public void ExtruderCheckerThrowsBuildInWalk(string filePath)
        {
            Start s = FileReadingTestUtilities.ParseFile(filePath);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            ExtruderChecker checker = new ExtruderChecker(symbolTable);
            Assert.Throws<CallBuildInWalkException>(() => s.Apply(checker));
        }

        private class ExtruderCheckerThrowsExtruderExceptionFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "ExtruderChecker/ThrowsBuildInWalk";
        }
    }
}