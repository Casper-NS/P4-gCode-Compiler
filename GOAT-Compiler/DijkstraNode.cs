using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal class DijkstraNode
    {
        private List<DijkstraNode> _functionCalls = new();

        private Extrude _extrudeType;

        internal DijkstraNode(Extrude extrude)
        {
            _extrudeType = extrude;
        }

        internal void AddFunctionCall(DijkstraNode dn)
        {
            _functionCalls.Add(dn);
        }

        internal void SetExtrudeType(Extrude e)
        {
            _extrudeType = e;
        }

        internal Extrude GetExtrudeType()
        {
            return _extrudeType;
        }

        internal void SearchChild() 
        {
            
        }
    }

}
