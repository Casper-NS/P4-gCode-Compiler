using System;
using System.IO;
using GOAT_Compiler;
using GOATCode.lexer;
using GOATCode.node;
using GOATCode.parser;

namespace P4_GCode_Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            ISymbolTable symTab = new RecSymbolTable();
            StreamReader reader = new StreamReader("../../../../visitorTests/Test.txt");
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            Start S = p.Parse();
            SymbolTableBuilder builder = new SymbolTableBuilder(symTab);
            S.Apply(builder);
            TypeChecker Tchecker = new TypeChecker(symTab);
            S.Apply(Tchecker);
            ScopeChecker scopeChecker = new ScopeChecker(symTab);
            S.Apply(scopeChecker);
            CodeGenerator codeGenerator;
            using (File.Create("test.gcode")) { }

            using (Stream stream = new FileStream("test.gcode", FileMode.Open))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    codeGenerator = new CodeGenerator(symTab, Tchecker.GetTypeDictionary(), writer);
                    S.Apply(codeGenerator);
                }

            }
        }
    }
}
