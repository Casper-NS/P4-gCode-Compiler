using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal class DijkstraNode
    {
        private List<DijkstraNode> _functionCalls = new();

        private Extrude _extrudeType;
        
        private DijkstraNode _cameFrom;
        
        internal DijkstraNode(Extrude extrude)
        {
            _extrudeType = extrude;
        }

        internal void AddFunctionCall(DijkstraNode dn)
        {
            _functionCalls.Add(dn);
        }
        
        internal List<DijkstraNode> GetFunctionCalls()
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
    }
}
