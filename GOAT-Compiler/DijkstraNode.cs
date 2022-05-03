using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal struct Edge
    {
        internal Edge(DijkstraNode dn, Extrude e)
        {
            dijkstraNode = dn;
            extrudeType = e;
        }
        internal DijkstraNode dijkstraNode;
        internal Extrude extrudeType;
    }


    internal class DijkstraNode
    {
        private List<Edge> _functionCalls = new();

        private Extrude _callStackExtrudeType = Extrude.NotSet;

        private Extrude _extrudeType;

        
        private DijkstraNode _cameFrom;
        
        internal DijkstraNode(Extrude extrude)
        {
            _extrudeType = extrude;
        }

        internal void AddFunctionCall(DijkstraNode dn, Extrude e)
        {
            _functionCalls.Add(new Edge(dn, e));
        }
        
        internal List<Edge> GetFunctionCalls()
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
    }
}
