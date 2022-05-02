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
using VisitorTests;
using VisitorTests.Utilities;
using Xunit;

namespace SymbolTableTest
{
    public class ScopeCheckTest
    {
        [SkippableFact(typeof(TestDependencyException))]
        public void Test_Refs_In_Testfile()
        {
            Start s = FileReadingTestUtilities.ParseFile(
                FileReadingTestUtilities.ProjectBaseDirectory + "ScopeChecker/SymbolBuildTest.txt");
            ISymbolTable symTab = FileReadingTestUtilities.BuildSymbolTable(s);

            ScopeChecker scopeChecker = new ScopeChecker(symTab);
            s.Apply(scopeChecker);
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(RefBeforeDeclFilesEnumerator))]
        public void Test_RefBeforeDecl_Exception(string filePath)
        {
            Start s = FileReadingTestUtilities.ParseFile(filePath);
            ISymbolTable symTab = FileReadingTestUtilities.BuildSymbolTable(s);

            ScopeChecker scopeChecker = new ScopeChecker(symTab);
            Assert.Throws<RefUsedBeforeClosestDeclException>(() => s.Apply(scopeChecker));
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(RefNotFoundFilesEnumerator))] 
        public void Test_RefNotFound_Exception(string filePath)
        {
            Start s = FileReadingTestUtilities.ParseFile(filePath);
            ISymbolTable symTab = FileReadingTestUtilities.BuildSymbolTable(s);
            
            ScopeChecker scopeChecker = new ScopeChecker(symTab);
            Assert.Throws<RefNotFoundException>(() => s.Apply(scopeChecker));
        }

        [SkippableTheory(typeof(TestDependencyException))]
        [ClassData(typeof(NotInitializedFilesEnumerator))]
        public void Test_VarNotInitialized_Exception(string filePath)
        {
            Start s = FileReadingTestUtilities.ParseFile(filePath);
            ISymbolTable symTab = FileReadingTestUtilities.BuildSymbolTable(s);
            
            ScopeChecker scopeChecker = new ScopeChecker(symTab);
            Assert.Throws<VarNotInitializedException>(() => s.Apply(scopeChecker));
        }

        private class RefBeforeDeclFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "ScopeChecker/ScopeTestFiles/RefBeforeDecl";
        }
        private class RefNotFoundFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "ScopeChecker/ScopeTestFiles/RefNotFound";
        }
        private class NotInitializedFilesEnumerator : BaseFilesEnumerator
        {
            public override string RelativeFolderPath() => "ScopeChecker/ScopeTestFiles/VarNotInitialized";
        }
    }
}
