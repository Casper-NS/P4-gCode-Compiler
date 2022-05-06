using System;
using System.Collections.Generic;

namespace GOAT_Compiler.Code_Generation
{
    public class BuildInFunctionImplementations
    {
        CNCMachine machine = new CNCMachine();

        /// <summary>
        /// Calls the specified built in function with the right params.
        /// </summary>
        /// <param name="functionName">The name of the built in function.</param>
        /// <param name="actuelParams">The params for the built in function.</param>
        /// <returns></returns>
        public dynamic CallBuildInFunctions(string functionName, List<dynamic> actuelParams)
        {
            switch (functionName)
            {
                case "RelMove":
                    RelMove(actuelParams[0]);
                    break;
                case "AbsMove":
                    AbsMove(actuelParams[0]);
                    break;
                case "RelArc":
                    RelArc(actuelParams[0], actuelParams[1]);
                    break;
                case "AbsArc":
                    AbsArc(actuelParams[0], actuelParams[1]);
                    break;
                case "SetExtruderRate":
                    SetExtruderRate(actuelParams[0]);
                    break;
                case "SetBedTemp":
                    SetBedTemp(actuelParams[0]);
                    break;
                case "SetExtruderTemp":
                    SetExtruderTemp(actuelParams[0]);
                    break;
                case "SetFanPower":
                    SetFanPower(actuelParams[0]);
                    break;
                case "Steps":
                    Steps(actuelParams[0]);
                    break;
                case "Position":
                    return Position();
                case "Lift":
                    Lift(actuelParams[0]);
                    break;
                case "Right":
                    Right(actuelParams[0]);
                    break;
                case "Left":
                    Left(actuelParams[0]);
                    break;
                case "Direction":
                    return Direction();
                case "TurnTo":
                    TurnTo(actuelParams[0]);
                    break;
                case "WaitForBedTemp":
                    WaitForBedTemp();
                    break;
                case "WaitForExtruderTemp":
                    WaitForExtruderTemp();
                    break;
                case "WaitForCurrentMove":
                    WaitForCurrentMove();
                    break;
                case "WaitForMillis":
                    WaitForMillis(actuelParams[0]);
                    break;
            }
            return null;
        }
        public void RelMove(Vector v)
        {
            string gLine = "";
            Vector oldPosition = machine.Position;
            machine.Position = new Vector(oldPosition.X + v.X, oldPosition.Y + v.Y, oldPosition.Z + v.Z);
            if (machine.Build)
            {
                machine.CurrentExtrusion += (machine.ExtruderRate*VectorDistance(oldPosition, machine.Position));
                gLine += "G1 X" + machine.Position.X + " Y" + machine.Position.Y + " Z" + machine.Position.Z + " E" + machine.CurrentExtrusion;
            }
            else
            {
                gLine += "G0 X" + machine.Position.X + " Y" + machine.Position.Y + " Z" + machine.Position.Z;
                
            }
        }
        public void AbsMove(Vector v)
        {
            string gLine = "";
            Vector oldPosition = machine.Position;
            machine.Position = v;
            if (machine.Build)
            {
                machine.CurrentExtrusion += (machine.ExtruderRate * VectorDistance(oldPosition, machine.Position));
                gLine += "G1 X" + machine.Position.X + " Y" + machine.Position.Y + " Z" + machine.Position.Z + " E" + machine.CurrentExtrusion;
            }
            else
            {
                
                gLine += "G0 X" + machine.Position.X + " Y" + machine.Position.Y + " Z" + machine.Position.Z;
            }
        }
        private float CircelLenght(Vector v1, Vector v2, float r)
        {
            float top = v1.X * v2.X + v1.Y * v2.Y;
            float bot = (float)Math.Sqrt(Math.Pow(v1.X, 2) + Math.Pow(v1.Y, 2)) * (float)Math.Sqrt(Math.Pow(v2.X, 2) + Math.Pow(v2.Y, 2));
            float deg = (float)Math.Acos(top/bot);
            return deg * r
;        }
        public void RelArc(Vector v, float r)
        {
            string gLine = "";
            Vector oldPosition = machine.Position;
            if (VectorDistance(oldPosition, v) > r*2)
            {
                throw new Exception("RelArc radius is too small.");
            }
            machine.Position = new Vector(oldPosition.X + v.X, oldPosition.Y + v.Y, oldPosition.Z + v.Z);
            Vector v2 = machine.Position;
            if (machine.Build)
            {
                if(r < 0) { 
                    machine.CurrentExtrusion += (machine.ExtruderRate * CircelLenght(oldPosition, machine.Position, -r));
                    gLine = "G3 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + machine.ExtruderRate + " R" + -r;
                }
                else
                {
                    machine.CurrentExtrusion += (machine.ExtruderRate * CircelLenght(oldPosition, machine.Position, r));
                    gLine = "G2 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + machine.ExtruderRate + " R" + r;
                }
            }
            else
            {
                if (r < 0)
                {
                    gLine = "G3 X" + "X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " R" + -r;
                }
                else
                {
                    gLine = "G2 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " R" + r;
                }
            }
        }
        private float VectorDistance(Vector v1, Vector v2) 
        {
            if(v1.Z == v2.Z)
            {
                return (float)Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
            }
            throw new Exception("VectorDistance: Z values are not the same.");
        }
        public void AbsArc(Vector v, float r)
        {
            string gLine = "";
            Vector oldPosition = machine.Position;
            if (VectorDistance(oldPosition, v) > r * 2)
            {
                throw new Exception("AbsArc radius is too small.");
            }
            machine.Position = v;
            Vector v2 = machine.Position;
            if (machine.Build)
            {
                if (r < 0)
                {
                    machine.CurrentExtrusion += (machine.ExtruderRate * CircelLenght(oldPosition, machine.Position, -r));
                    gLine = "G3 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + machine.ExtruderRate + " R" + -r;
                }
                else
                {
                    machine.CurrentExtrusion += (machine.ExtruderRate * CircelLenght(oldPosition, machine.Position, r));
                    gLine = "G2 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + machine.ExtruderRate + " R" + r;
                }
            }
            else
            {
                if (r < 0)
                {
                    gLine = "G3 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " R" + -r;
                }
                else
                {
                    gLine = "G2 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " R" + r;
                }
            }
        }
        public void SetExtruderTemp(float temp)
        {
            string gLine = "";
            machine.ExtruderTemp = temp;
            gLine = "M104 S" + temp.ToString();
        }
        public void SetFanPower(float power)
        {
            
            string gLine = "";
            machine.FanPower = power;
            gLine = "M106 S" + power.ToString();
        }
        public void SetExtruderRate(float rate)
        {
            string gLine = "";
            machine.ExtruderRate = rate;
            gLine = "M220 S" + rate.ToString();
        }
        public void SetBedTemp(float temp)
        {
            string gLine = "";
            machine.BedTemp = temp;
            gLine = "M140 S" + temp.ToString();
        }
        public Vector Position(){
            return machine.Position;
        }
        public void Steps(float step)
        {
            Vector oldPosition = machine.Position;
            Vector newPoint = new ((float)Math.Cos(DegreesToRadians(machine.Rotation))*step, 
                                   (float)Math.Sin(DegreesToRadians(machine.Rotation))*step,
                                   0);
            Vector newPosition = new (oldPosition.X + newPoint.X, oldPosition.Y + newPoint.Y, oldPosition.Z + newPoint.Z);
            RelMove(newPosition);
        }
        private float DegreesToRadians(float degrees)
        {
           return degrees * (float)Math.PI / 180.0f;
        }
        public void Lift(float step)
        {
            string gLine = "";
            machine.Position.Z += step;
            if (machine.Build)
            {
                machine.CurrentExtrusion += machine.ExtruderRate * step;
                gLine += "G1 X" + machine.Position.X + " Y" + machine.Position.Y + " Z" + machine.Position.Z + " E" + machine.CurrentExtrusion;
            }
            else
            {
                gLine += "G0 X" + machine.Position.X + " Y" + machine.Position.Y + " Z" + machine.Position.Z;

            }
        }
        public void Right(float deg)
        {
            machine.Rotation -= deg;
        }
        public void Left(float deg)
        {
            machine.Rotation += deg;
        }
        public float Direction()
        {
            return machine.Rotation;
        }
        public void TurnTo(float deg)
        {
            machine.Rotation = deg;
        }
        public void WaitForBedTemp()
        {
            string gLine = "M190 R" + machine.BedTemp.ToString();
        }
        public void WaitForExtruderTemp()
        {
            string gLine = "M109 R" + machine.ExtruderTemp.ToString();
        }
        public void WaitForCurrentMove()
        {
            string gLine = "M400";
        }
        public void WaitForMillis(int millis)
        {
            string gLine = "G4 P" + millis.ToString();
        }
    }
}
