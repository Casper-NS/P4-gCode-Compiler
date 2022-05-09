using System.Collections.Generic;

namespace GOAT_Compiler
{
    /// <summary>
    /// This class represents a functioncall from a BFSnode.
    /// </summary>
    internal struct FunctionCall
    {
        internal FunctionCall(BFSNode bfsnode, Extrude e)
        {
            BFSNode = bfsnode;
            extrudeType = e;
        }
        internal BFSNode BFSNode;
        internal Extrude extrudeType;
    }

    /// <summary>
    /// BFSNode represents a function with a name, a list of function calls, and what node it came from.
    /// </summary>
    internal class BFSNode
    {
        internal string Name { get; private set; }

        private List<FunctionCall> _functionCalls = new();

        private Extrude _extrudeType;

        private Extrude _callStackExtrudeType = Extrude.NotSet;

        private BFSNode _cameFrom;
        
        internal BFSNode(string n, Extrude extrude)
        {
            Name = n;
            _extrudeType = extrude;
        }

        internal void AddFunctionCall(BFSNode dn, Extrude e)
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

        internal void SetTheNodeItCameFrom(BFSNode node)
        {
            _cameFrom = node;
        }

        internal BFSNode GetWhereItWasCalled()
        {
            return _cameFrom;
        }
    }
}
