using GOAT_Compiler;
using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorTests.Utilities;
using Xunit;
using static GOAT_Compiler.CodeGenerator;

namespace VisitorTests
{
    public class CodeGeneratorTest
    {
        [SkippableTheory(typeof(TestDependencyException))]
        [InlineData("RelMove.txt", "RelMove.gcode")]
        public void CreatedFileChecker(string file, string outPutFileName)
        {
            Start s = FileReadingTestUtilities.ParseFile(file);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);

            _ = new CodeGenerator(symbolTable, outPutFileName);
            //Create a assert that checks if the output file exists
            Assert.True(System.IO.File.Exists(outPutFileName));
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [InlineData("RelMove.txt", "RelMove.gcode")]
        public void CheckIfCreatedFileIsntEmpty(string file, string outPutFileName)
        {
            Start s = FileReadingTestUtilities.ParseFile(file);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);

            _ = new CodeGenerator(symbolTable, outPutFileName);
            
            //Create a assert that checks if the output file is not empty
            Assert.True(System.IO.File.ReadAllText(outPutFileName).Length > 0);
        }

    }
    class CodeGeneratorFilesEnumerator : BaseFilesEnumerator
    {
        public override string RelativeFolderPath() => "CodeGenerator/TestFiles";
    }
}
