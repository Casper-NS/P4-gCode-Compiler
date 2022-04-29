using GOATCode.lexer;
using GOATCode.node;
using GOATCode.parser;
using System;
using System.IO;
using Xunit;
using GOAT_Compiler;

namespace VisitorTests
{
    public class FileReadingTestUtilities
    {
        internal static string ProjectBaseDirectory => "../../../";
        internal static Start ParseFile(string filePath)
        {
            try
            {
                ISymbolTable symTab = new RecSymbolTable();
                StreamReader reader = new StreamReader(filePath);
                Lexer l = new Lexer(reader);
                Parser p = new Parser(l);
                return p.Parse();
            }
            catch (Exception e)
            {
                throw new SkipException("Test dependency parsing failed: \n" + e, e);
            }
        }
        internal static ISymbolTable BuildSymbolTable(Start s)
        {
            try
            {
                ISymbolTable symbolTable = new RecSymbolTable();
                SymbolTableBuilder symbolTableBuilder = new SymbolTableBuilder(symbolTable);
                s.Apply(symbolTableBuilder);
                return symbolTable;
            }
            catch (Exception e)
            {
                throw new SkipException("Test dependency symbol table building failed: \n" + e, e);
            }
        }
    }
    //public class TestDependencyException : compiler
}
