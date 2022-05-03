using GOAT_Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static GOAT_Compiler.RuntimeTable;

namespace VisitorTests
{
    public class RuntimeTableTests
    {
        [Theory]
        [InlineData("a", Types.Integer, 2)]
        public void GetIntegerValuesFromRuntimeTable(string name, Types types, int value)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.AddVariableSymbol(name, types);
            Symbol symbol = symbolTable.GetVariableSymbol(name);
            RuntimeTable table = new RuntimeTable(symbolTable);
            table.Put(symbol, value);
            Assert.Equal(value, table.Get(symbol));
        }
        
        [Theory]
        [InlineData("bool", Types.Boolean, true)]
        public void GetBooleanValuesFromRuntimeTable(string name, Types types, bool value)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.AddVariableSymbol(name, types);
            Symbol symbol = symbolTable.GetVariableSymbol(name);
            RuntimeTable table = new RuntimeTable(symbolTable);
            table.Put(symbol, value);
            Assert.Equal(value, table.Get(symbol));
        }
        
        [Theory]
        [InlineData("vector", Types.Vector, 2.2f, 3.3f, 4.4f)]
        public void GetVectorValuesFromRuntimeTable(string name, Types types, float x, float y, float z)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.AddVariableSymbol(name, types);
            Symbol symbol = symbolTable.GetVariableSymbol(name);
            RuntimeTable table = new RuntimeTable(symbolTable);
            Vector value = new Vector(x, y, z);
            table.Put(symbol, value);
            Assert.Equal(value, table.Get(symbol));
        }

        [Theory]
        [InlineData("float", Types.FloatingPoint, 2.2f)]
        public void GetFloatValuesFromRuntimeTable(string name, Types types, float value)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.AddVariableSymbol(name, types);
            Symbol symbol = symbolTable.GetVariableSymbol(name);
            RuntimeTable table = new RuntimeTable(symbolTable);
            table.Put(symbol, value);
            Assert.Equal(value, table.Get(symbol));
        }
    }
}
