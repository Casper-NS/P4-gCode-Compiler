using GOAT_Compiler;
using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static GOAT_Compiler.CodeGenerator;

namespace VisitorTests
{
    public class RuntimeTableTests
    {
        [Theory]
        [InlineData("a", Types.Integer, 2)]
        public void GetIntegerValuesFromRuntimeTable(string name, Types types, int value)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddVariableSymbol(name, types);
            Symbol symbol = symbolTable.GetVariableSymbol(name);
            RuntimeTable<Symbol> table = new RuntimeTable<Symbol>();
            table.Put(symbol, value);
            Assert.Equal(value, table.Get(symbol, symbol.type));
        }
        
        [Theory]
        [InlineData("bool", Types.Boolean, true)]
        public void GetBooleanValuesFromRuntimeTable(string name, Types types, bool value)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddVariableSymbol(name, types);
            Symbol symbol = symbolTable.GetVariableSymbol(name);
            RuntimeTable<Symbol> table = new RuntimeTable<Symbol>();
            table.Put(symbol, value);
            Assert.Equal(value, table.Get(symbol, symbol.type));
        }
        
        [Theory]
        [InlineData("vector", Types.Vector, 2.2f, 3.3f, 4.4f)]
        public void GetVectorValuesFromRuntimeTable(string name, Types types, float x, float y, float z)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddVariableSymbol(name, types);
            Symbol symbol = symbolTable.GetVariableSymbol(name);
            RuntimeTable<Symbol> table = new RuntimeTable<Symbol>();
            Vector value = new Vector(x, y, z);
            table.Put(symbol, value);
            Assert.Equal(value, table.Get(symbol, symbol.type));
        }

        [Theory]
        [InlineData("float", Types.FloatingPoint, 2.2f)]
        public void GetFloatValuesFromRuntimeTable(string name, Types types, float value)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddVariableSymbol(name, types);
            Symbol symbol = symbolTable.GetVariableSymbol(name);
            RuntimeTable<Symbol> table = new RuntimeTable<Symbol>();
            table.Put(symbol, value);
            Assert.Equal(value, table.Get(symbol, symbol.type));
        }
    }
}
