using GOAT_Compiler;
using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            string TestFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/Tests/InputFolder/" + file;
            string OutputFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/Tests/OutputFolder/" + outPutFileName;
            Start s = FileReadingTestUtilities.ParseFile(TestFilePath);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            TypeChecker typeChecker = new TypeChecker(symbolTable);
            s.Apply(typeChecker);
            

            using (Stream stream = new FileStream(OutputFilePath, FileMode.Open))
            {
                _ = new CodeGenerator(symbolTable, typeChecker.GetTypeDictionary(), stream);
            }

            Assert.True(File.Exists(OutputFilePath+".gcode"));
            if (File.Exists(OutputFilePath + ".gcode"))
            {
                File.Delete(OutputFilePath + ".gcode");
            }
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [InlineData("RelMoveTest.txt", "RelMove.gcode")]
        public void CheckIfCreatedFileIsntEmpty(string file, string outPutFileName)
        {
            string TestFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/Tests/InputFolder/" + file;
            string OutputFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/Tests/OutputFolder/" + outPutFileName;
            Start s = FileReadingTestUtilities.ParseFile(TestFilePath);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            TypeChecker typeChecker = new TypeChecker(symbolTable);
            s.Apply(typeChecker);
            
            using (Stream stream = new FileStream(OutputFilePath, FileMode.Open))
            {
                _ = new CodeGenerator(symbolTable, typeChecker.GetTypeDictionary(), stream);
            }
            Assert.True(File.Exists(OutputFilePath + ".gcode"));
            
            Assert.True(File.ReadAllText(OutputFilePath + ".gcode").Length > 0);
            if (File.Exists(OutputFilePath + ".gcode"))
            {
                File.Delete(OutputFilePath + ".gcode");
            }
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(FloatArithmeticWorksFilesEnumerator))]
        public void CheckFloatArithmeticsResult(string file)
        {
            float expectedFloat;
            // grab the expected float from the file name
            try
            {
                string fileName = file.Split("/").Last();
                string floatingPointString = fileName.Split('{', '}')[1];
                expectedFloat = float.Parse(floatingPointString, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new TestDependencyException("Failed to get the expected float", e);
            }

            Start s = FileReadingTestUtilities.ParseFile(file);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            TypeChecker typeChecker = FileReadingTestUtilities.DoTypeChecking(s, symbolTable);
            MemoryStream dummyStream = new MemoryStream();
            CodeGenerator generator = new CodeGenerator(symbolTable, typeChecker.GetTypeDictionary(), dummyStream);
            s.Apply(generator);
            
            //open the global scope and check the value of f
            symbolTable.OpenScope(s.GetPProgram());
            float f = generator.RT.Get(symbolTable.GetVariableSymbol("f"), Types.FloatingPoint);

            // assert equality within 5 decimal places
            Assert.Equal(expectedFloat, f, 5);

        }


    }
    class CodeGeneratorFilesEnumerator : BaseFilesEnumerator
    {
        public override string RelativeFolderPath() => "CodeGenerator/Tests/InputFolder";
    }
    class FloatArithmeticWorksFilesEnumerator : BaseFilesEnumerator
    {
        public override string RelativeFolderPath() => "CodeGenerator/Tests/FloatArithmetics";
    }
}
