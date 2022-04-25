using System;
using Xunit;
using GOAT_Compiler;

namespace SymbolTableTest
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("a", Types.integer)]
        [InlineData("b", Types.boolean)]
        [InlineData("v", Types.vector)]
        public void Check_If_The_Type_Is_Correct_In_Symbol_Table(string value, Types type)
        {
            ISymbolTable symbolTable = new SymbolTable();
            symbolTable.OpenScope();
            symbolTable.AddSymbol(value, type);
            Assert.Equal(type, symbolTable.GetSymbol(value).type);
            symbolTable.CloseScope();
        }

        [Theory]
        [InlineData("a", 2)]
        public void test(string symbolName, int depth)
        {
            ISymbolTable symbolTable = new SymbolTable();
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.OpenScope();
            }
            symbolTable.AddSymbol(symbolName, Types.integer);
            Assert.Equal(depth, symbolTable.GetSymbol(symbolName).depth);
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.CloseScope();
            }
        }
    }
}
