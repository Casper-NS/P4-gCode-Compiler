using GOAT_Compiler;
using GOAT_Compiler.Code_Generation;
using GOATCode.node;
using System.IO;
using System.Linq;
using System.Text;
using VisitorTests.Utilities;
using Xunit;

namespace VisitorTests
{
    public class BuiltInFunctionsTests
    {
        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(GeneratesExpectedCodeEnumerator))]
        public void BuiltInFunctionTest(string file)
        {
            // get the file and split it into the two sections
            string TestFilePath = FileReadingTestUtilities.ProjectBaseDirectory + "CodeGenerator/Tests/InputFolder/" + file;
            string fileContent = File.ReadAllText(TestFilePath);
            string[] split = fileContent.Split('@');
            string GOATCode = split.First();
            string GCode = split.Last().Trim();

            // build expected file with start and end code included
            StringBuilder expectedFile = new StringBuilder();

            using (StringWriter writer = new StringWriter(expectedFile))
            {
                BuildInFunctionImplementations b = new BuildInFunctionImplementations(new CNCMachine(), writer);

                b.StartUp();

                writer.WriteLine(GCode);

                b.EndFile();
            }


            // do the parsing and checking needed
            Start s = FileReadingTestUtilities.ParseString(GOATCode);
            ISymbolTable symbolTable = FileReadingTestUtilities.BuildSymbolTable(s);
            TypeChecker typeChecker = FileReadingTestUtilities.DoTypeChecking(s, symbolTable);

            // run the code generator
            StringBuilder actualFile = new StringBuilder();
            CodeGenerator codeGenerator;


            using (StringWriter writer = new StringWriter(actualFile))
            {
                codeGenerator = new CodeGenerator(symbolTable, typeChecker.TypeDictionary, writer);
                s.Apply(codeGenerator);
            }

            // compare files
            Assert.Equal(expectedFile.ToString().Trim(), actualFile.ToString().Trim());
        }

        private class GeneratesExpectedCodeEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "CodeGenerator/Tests/Generated";
        }
    }
}