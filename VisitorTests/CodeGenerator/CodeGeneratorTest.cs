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
        [InlineData("RelMoveTest.txt", "RelMoveTest.gcode")]
        public void CreatedFileChecker(string file, string outPutFileName)
        {
            string TestFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/Tests/InputFolder/"+ file;
            Start s = FileReadingTestUtilities.ParseFile(TestFilePath);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            TypeChecker typeChecker = FileReadingTestUtilities.DoTypeChecking(s, symbolTable);
            
            CodeGenerator codeGenerator;
            using (File.Create(outPutFileName)) { }
            
            using (Stream stream = new FileStream(outPutFileName, FileMode.Open))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    codeGenerator = new CodeGenerator(symbolTable, typeChecker.GetTypeDictionary(), writer);
                    s.Apply(codeGenerator);
                }
            }

            Assert.True(File.Exists(outPutFileName));
            if (File.Exists(outPutFileName))
            {
                File.Delete(outPutFileName);
            }
        }
        [SkippableTheory(typeof(TestDependencyException))]
        [InlineData("RelMoveTest.txt", "RelMove.gcode")]
        public void CheckIfCreatedFileIsntEmpty(string file, string outPutFileName)
        {
            string TestFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/Tests/InputFolder/" + file;
            Start s = FileReadingTestUtilities.ParseFile(TestFilePath);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            TypeChecker typeChecker = FileReadingTestUtilities.DoTypeChecking(s, symbolTable);
            
            CodeGenerator codeGenerator;
            using (File.Create(outPutFileName)) { }

            using (Stream stream = new FileStream(outPutFileName, FileMode.Open))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    codeGenerator = new CodeGenerator(symbolTable, typeChecker.GetTypeDictionary(), writer);
                    s.Apply(codeGenerator);
                }
            }
            Assert.True(File.Exists(outPutFileName));
            
            Assert.True(File.ReadAllText(outPutFileName).Length > 0);
            if (File.Exists(outPutFileName))
            {
                File.Delete(outPutFileName);
            }
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(FloatArithmeticWorksFilesEnumerator))]
        public void CheckFloatArithmeticsResult(string file)
        {
            double expectedFloat;
            // grab the expected float from the file name
            try
            {
                string fileName = file.Split("/").Last();
                string floatingPointString = fileName.Split('{', '}')[1];
                expectedFloat = double.Parse(floatingPointString, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new TestDependencyException("Failed to get the expected float", e);
            }

            Start s = FileReadingTestUtilities.ParseFile(file);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            TypeChecker typeChecker = FileReadingTestUtilities.DoTypeChecking(s, symbolTable);
            CodeGenerator generator;

            using (MemoryStream dummyStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(dummyStream))
                {
                    generator = new CodeGenerator(symbolTable, typeChecker.GetTypeDictionary(), writer);
                    s.Apply(generator);
                }
            }

            
            
            //open the global scope and check the value of f
            symbolTable.OpenScope(s.GetPProgram());
            double f = generator.RT.Get(symbolTable.GetVariableSymbol("f"), Types.FloatingPoint);

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
