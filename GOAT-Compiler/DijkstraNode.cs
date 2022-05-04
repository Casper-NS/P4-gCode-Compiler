using System.Collections.Generic;

namespace GOAT_Compiler
{
    /// <summary>
    /// This class represents a functioncall from a dijkstranode.
    /// </summary>
    internal struct FunctionCall
    {
        internal FunctionCall(DijkstraNode dn, Extrude e)
        {
            dijkstraNode = dn;
            extrudeType = e;
        }
        internal DijkstraNode dijkstraNode;
        internal Extrude extrudeType;
    }

    /// <summary>
    /// DijkstraNode represents a function with a name, a list of function calls, and what node it came from.
    /// </summary>
    internal class DijkstraNode
    {
        internal string Name { get; private set; }

        private List<FunctionCall> _functionCalls = new();

        private Extrude _extrudeType;

        private Extrude _callStackExtrudeType = Extrude.NotSet;

        private DijkstraNode _cameFrom;
        
        internal DijkstraNode(string n, Extrude extrude)
        {
            Name = n;
            _extrudeType = extrude;
        }

        internal void AddFunctionCall(DijkstraNode dn, Extrude e)
        {
            _functionCalls.Add(new FunctionCall(dn, e));
        }
        
        internal List<FunctionCall> GetFunctionCalls()
        {
            return _functionCalls;
        }

        internal void SetExtrudeType(Extrude e)
        {
            _extrudeType = e;
        }

        internal Extrude GetExtrudeType()
        {
            return _extrudeType;
        }

        internal Extrude GetCallStackType()
        {
            return _callStackExtrudeType;
        }

        internal void SetCallStackType(Extrude e)
        {
            _callStackExtrudeType = e;
        }

        internal void SetTheNodeItCameFrom(DijkstraNode node)
        {
            _cameFrom = node;
        }

        internal DijkstraNode GetWhereItWasCalled()
        {
            return _cameFrom;
        }
    }
}
