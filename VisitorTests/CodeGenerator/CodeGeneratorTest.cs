using GOAT_Compiler;
using GOATCode.node;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorTests.Utilities;
using Xunit;

namespace VisitorTests
{
    public class CodeGeneratorTest
    {
        [SkippableTheory(typeof(TestDependencyException))]
        [InlineData("RelMoveTest.txt", "RelMoveTest")]
        public void CreatedFileChecker(string file, string outPutFileName)
        {
            string TestFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/TestFiles/InputFolder/" + file;
            string OutputFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/TestFiles/OutputFolder/" + outPutFileName;
            Start s = FileReadingTestUtilities.ParseFile(TestFilePath);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            TypeChecker typeChecker = new TypeChecker(symbolTable);
            s.Apply(typeChecker);

            _ = new CodeGenerator(symbolTable, typeChecker.GetTypeDictionary(), OutputFilePath);

            Assert.True(File.Exists(OutputFilePath+".gcode"));
            if (File.Exists(OutputFilePath + ".gcode"))
            {
                File.Delete(OutputFilePath + ".gcode");
            }
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [InlineData("RelMove.txt", "RelMove.gcode")]
        public void CheckIfCreatedFileIsntEmpty(string file, string outPutFileName)
        {
            string TestFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/TestFiles/InputFolder/" + file;
            string OutputFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/TestFiles/OutputFolder/" + outPutFileName;
            Start s = FileReadingTestUtilities.ParseFile(TestFilePath);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            TypeChecker typeChecker = new TypeChecker(symbolTable);
            s.Apply(typeChecker);

            _ = new CodeGenerator(symbolTable, typeChecker.GetTypeDictionary(), OutputFilePath);

            Assert.True(File.Exists(OutputFilePath + ".gcode"));
            
            Assert.True(File.ReadAllText(OutputFilePath + ".gcode").Length > 0);
            if (File.Exists(OutputFilePath + ".gcode"))
            {
                File.Delete(OutputFilePath + ".gcode");
            }
        }

    }
    class CodeGeneratorFilesEnumerator : BaseFilesEnumerator
    {
        public override string RelativeFolderPath() => "CodeGenerator/TestFiles/InputFolder";
    }
}
