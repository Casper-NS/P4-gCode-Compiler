using System;
using Xunit;
using GOAT_Compiler;

namespace SymbolTableTest
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("a", Types.Integer)]
        [InlineData("b", Types.Boolean)]
        [InlineData("v", Types.Vector)]
        public void Check_If_The_Type_Is_Correct_In_Symbol_Table(string value, Types type)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope();
            symbolTable.AddSymbol(value, type);
            Assert.Equal(type, symbolTable.GetSymbol(value).type);
            symbolTable.CloseScope();
        }

        [Theory]
        [InlineData("a", 2, Types.Integer)]
        [InlineData("b", 3, Types.Boolean)]
        public void Test(string symbolName, int depth, Types type)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope();
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.OpenScope();
            }
            symbolTable.AddSymbol(symbolName, type);
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.CloseScope();
            }
            symbolTable.CloseScope();
            symbolTable.OpenScope();
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.OpenScope();
            }
            Assert.Contains(symbolName, symbolTable.GetSymbol(symbolName).name);
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.CloseScope();
            }
            symbolTable.CloseScope();
        }
    }
}
