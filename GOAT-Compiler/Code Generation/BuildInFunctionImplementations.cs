using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GOAT_Compiler.Code_Generation
{
    /// <summary>
    /// The class with the implentations of the built in functions
    /// </summary>
    public class BuildInFunctionImplementations
    {
        private CNCMachine _machine;
        private Stream _stream;
        public BuildInFunctionImplementations(CNCMachine newMachine, Stream stream)
        {
            _machine = newMachine;
            _stream = stream;
        }

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
            Vector oldPosition = _machine.Position;
            _machine.Position = new Vector(oldPosition.X + v.X, oldPosition.Y + v.Y, oldPosition.Z + v.Z);
            if (_machine.Build)
            {
                _machine.CurrentExtrusion += (_machine.ExtruderRate*VectorDistance(oldPosition, _machine.Position));
                gLine += "G1 X" + _machine.Position.X + " Y" + _machine.Position.Y + " Z" + _machine.Position.Z + " E" + _machine.CurrentExtrusion;
            }
            else
            {
                gLine += "G0 X" + _machine.Position.X + " Y" + _machine.Position.Y + " Z" + _machine.Position.Z;
            }
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
        }
        public void AbsMove(Vector v)
        {
            string gLine = "";
            Vector oldPosition = _machine.Position;
            _machine.Position = v;
            if (_machine.Build)
            {
                _machine.CurrentExtrusion += (_machine.ExtruderRate * VectorDistance(oldPosition, _machine.Position));
                gLine += "G1 X" + _machine.Position.X + " Y" + _machine.Position.Y + " Z" + _machine.Position.Z + " E" + _machine.CurrentExtrusion;
            }
            else
            {
                gLine += "G0 X" + _machine.Position.X + " Y" + _machine.Position.Y + " Z" + _machine.Position.Z;
            }
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
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
            Vector oldPosition = _machine.Position;
            if (VectorDistance(oldPosition, v) > r*2)
            {
                throw new Exception("RelArc radius is too small.");
            }
            _machine.Position = new Vector(oldPosition.X + v.X, oldPosition.Y + v.Y, oldPosition.Z + v.Z);
            Vector v2 = _machine.Position;
            if (_machine.Build)
            {
                if(r < 0) { 
                    _machine.CurrentExtrusion += (_machine.ExtruderRate * CircelLenght(oldPosition, _machine.Position, -r));
                    gLine = "G3 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + _machine.ExtruderRate + " R" + -r;
                }
                else
                {
                    _machine.CurrentExtrusion += (_machine.ExtruderRate * CircelLenght(oldPosition, _machine.Position, r));
                    gLine = "G2 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + _machine.ExtruderRate + " R" + r;
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
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
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
            Vector oldPosition = _machine.Position;
            if (VectorDistance(oldPosition, v) > r * 2)
            {
                throw new Exception("AbsArc radius is too small.");
            }
            _machine.Position = v;
            Vector v2 = _machine.Position;
            if (_machine.Build)
            {
                if (r < 0)
                {
                    _machine.CurrentExtrusion += (_machine.ExtruderRate * CircelLenght(oldPosition, _machine.Position, -r));
                    gLine = "G3 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + _machine.ExtruderRate + " R" + -r;
                }
                else
                {
                    _machine.CurrentExtrusion += (_machine.ExtruderRate * CircelLenght(oldPosition, _machine.Position, r));
                    gLine = "G2 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + _machine.ExtruderRate + " R" + r;
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
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
        }
        public void SetExtruderTemp(float temp)
        {
            string gLine = "";
            _machine.ExtruderTemp = temp;
            gLine = "M104 S" + temp.ToString();
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
        }
        public void SetFanPower(float power)
        {
            string gLine = "";
            _machine.FanPower = power;
            gLine = "M106 S" + power.ToString();
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
            
        }
        public void SetExtruderRate(float rate)
        {
            string gLine = "";
            _machine.ExtruderRate = rate;
            gLine = "M220 S" + rate.ToString();
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
        }
        public void SetBedTemp(float temp)
        {
            string gLine = "";
            _machine.BedTemp = temp;
            gLine = "M140 S" + temp.ToString();
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
        }
        public Vector Position(){
            return _machine.Position;
        }
        public void Steps(float step)
        {
            Vector oldPosition = _machine.Position;
            Vector newPoint = new ((float)Math.Cos(DegreesToRadians(_machine.Rotation))*step, 
                                   (float)Math.Sin(DegreesToRadians(_machine.Rotation))*step,
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
            _machine.Position.Z += step;
            if (_machine.Build)
            {
                _machine.CurrentExtrusion += _machine.ExtruderRate * step;
                gLine += "G1 X" + _machine.Position.X + " Y" + _machine.Position.Y + " Z" + _machine.Position.Z + " E" + _machine.CurrentExtrusion;
            }
            else
            {
                gLine += "G0 X" + _machine.Position.X + " Y" + _machine.Position.Y + " Z" + _machine.Position.Z;
            }
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
        }
        public void Right(float deg)
        {
            _machine.Rotation -= deg;
        }
        public void Left(float deg)
        {
            _machine.Rotation += deg;
        }
        public float Direction()
        {
            return _machine.Rotation;
        }
        public void TurnTo(float deg)
        {
            _machine.Rotation = deg;
        }
        public void WaitForBedTemp()
        {
            string gLine = "M190 R" + _machine.BedTemp.ToString();
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
        }
        public void WaitForExtruderTemp()
        {
            string gLine = "M109 R" + _machine.ExtruderTemp.ToString();
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
        }
        public void WaitForCurrentMove()
        {
            string gLine = "M400";
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
        }
        public void WaitForMillis(int millis)
        {
            string gLine = "G4 P" + millis.ToString();
            _stream.Write(Encoding.UTF8.GetBytes(gLine), 0, gLine.Length);
        }
    }
}
