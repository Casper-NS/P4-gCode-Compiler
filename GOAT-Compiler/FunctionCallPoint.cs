using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal class FunctionCallPoint
    {

        internal FunctionCallPoint() { }
        internal FunctionCallPoint(List<FunctionCallPoint> fList, Extrude extrude)
        {
            _extrudeType = extrude;
            foreach (FunctionCallPoint f in fList)
            {
                points.Add(f);
            }
        }


        private List<FunctionCallPoint> points = new List<FunctionCallPoint>();
        
        private Extrude _extrudeType;

        internal void AddFunctionCall(FunctionCallPoint fcp)
        {
            points.Add(fcp);
        }

        internal void SetExtrudeType(Extrude e)
        {
            _extrudeType = e;
        }
    }

}
