using System;
using System.Collections.Generic;
using System.Globalization;
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
        private TextWriter _stream;
        public BuildInFunctionImplementations(CNCMachine newMachine, TextWriter stream)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
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
                case "RelArcCW":
                    RelArc(actuelParams[0], actuelParams[1], false);
                    break;
                case "RelArcCCW":
                    RelArc(actuelParams[0], actuelParams[1], true);
                    break;
                case "AbsArcCW":
                    AbsArc(actuelParams[0], actuelParams[1], false);
                    break;
                case "AbsArcCCW":
                    AbsArc(actuelParams[0], actuelParams[1], true);
                    break;
                case "SetExtrusionRate":
                    SetExtrusionRate(actuelParams[0]);
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
                case "Home":
                    Home();
                    break;
            }
            return null;
        }

        private void Home()
        {
            string gLine = "G28";
            _machine.Position = new Vector(0, 0, 0);
            _stream.WriteLine(gLine);
        }

        private void RelMove(Vector v)
        {
            string gLine = "";
            Vector oldPosition = _machine.Position;
            _machine.Position = new Vector(oldPosition.X + v.X, oldPosition.Y + v.Y, oldPosition.Z + v.Z);
            if (_machine.Build)
            {
                _machine.CurrentExtrusion += (_machine.ExtrusionRate*VectorDistance(oldPosition, _machine.Position));
                gLine += "G1 X" + (decimal)_machine.Position.X + " Y" + (decimal)_machine.Position.Y + " Z" + (decimal)_machine.Position.Z + " E" + (decimal)_machine.CurrentExtrusion;
            }
            else
            {
                gLine += "G0 X" + (decimal)_machine.Position.X + " Y" + (decimal)_machine.Position.Y + " Z" + (decimal)_machine.Position.Z;
            }
            _stream.WriteLine(gLine);
        }
        private void AbsMove(Vector v)
        {
            string gLine = "";
            Vector oldPosition = _machine.Position;
            _machine.Position = v;
            if (_machine.Build)
            {
                _machine.CurrentExtrusion += (_machine.ExtrusionRate * VectorDistance(oldPosition, _machine.Position));
                gLine += "G1 X" + (decimal)_machine.Position.X + " Y" + (decimal)_machine.Position.Y + " Z" + (decimal)_machine.Position.Z + " E" + (decimal)_machine.CurrentExtrusion;
            }
            else
            {
                gLine += "G0 X" + (decimal)_machine.Position.X + " Y" + (decimal)_machine.Position.Y + " Z" + (decimal)_machine.Position.Z;
            }
            _stream.WriteLine(gLine);
        }
        private double CircelLenght(Vector v1, Vector v2, double r)
        {
            double chord = VectorDistance(v1, v2);
            double shortArcLength = 2 * r * Math.Asin(chord / (2 * r));
            if (r < 0) // if its the long arc
            {
                // subtract from total circle length
                return (2 * Math.Abs(r) * Math.PI) - shortArcLength;
            }
            else
            {
                return shortArcLength;
            }
        }
        private void RelArc(Vector v, double r, bool CCW)
        {
            string gLine = "";
            Vector oldPosition = _machine.Position;
            _machine.Position = new Vector(oldPosition.X + v.X, oldPosition.Y + v.Y, oldPosition.Z + v.Z);
            Vector v2 = _machine.Position;
            if (_machine.Build)
            {
                if(CCW) { 
                    _machine.CurrentExtrusion += (_machine.ExtrusionRate * CircelLenght(oldPosition, _machine.Position, r));
                    gLine = "G3 X" + (decimal)v2.X + " Y" + (decimal)v2.Y + " Z" + (decimal)v2.Z + " E" + (decimal)_machine.CurrentExtrusion + " R" + (decimal)r;
                }
                else
                {
                    _machine.CurrentExtrusion += (_machine.ExtrusionRate * CircelLenght(oldPosition, _machine.Position, r));
                    gLine = "G2 X" + (decimal)v2.X + " Y" + (decimal)v2.Y + " Z" + (decimal)v2.Z + " E" + (decimal)_machine.CurrentExtrusion + " R" + (decimal)r;
                }
            }
            else
            {
                if (CCW)
                {
                    gLine = "G3 X" + (decimal)v2.X + " Y" + (decimal)v2.Y + " Z" + (decimal)v2.Z + " R" + (decimal)r;
                }
                else
                {
                    gLine = "G2 X" + (decimal)v2.X + " Y" + (decimal)v2.Y + " Z" + (decimal)v2.Z + " R" + (decimal)r;
                }
            }
            if (VectorDistance(oldPosition, v2) > Math.Abs(r)*2)
            {
                throw new Exception("RelArc radius is too small.");
            }
            _stream.WriteLine(gLine);
        }
        private double VectorDistance(Vector v1, Vector v2) 
        {
            return Math.Sqrt(Math.Pow((v1.X - v2.X), 2) + Math.Pow(v1.Y - v2.Y, 2) + Math.Pow(v1.Z - v2.Z, 2));
        }
        private void AbsArc(Vector v, double r, bool CCW)
        {
            string gLine = "";
            Vector oldPosition = _machine.Position;
            _machine.Position = v;
            Vector v2 = _machine.Position;
            if (_machine.Build)
            {
                if (CCW)
                {
                    _machine.CurrentExtrusion += (_machine.ExtrusionRate * CircelLenght(oldPosition, _machine.Position, r));
                    gLine = "G3 X" + (decimal)v2.X + " Y" + (decimal)v2.Y + " Z" + (decimal)v2.Z + " E" + (decimal)_machine.CurrentExtrusion + " R" + (decimal)r;
                }
                else
                {
                    _machine.CurrentExtrusion += (_machine.ExtrusionRate * CircelLenght(oldPosition, _machine.Position, r));
                    gLine = "G2 X" + (decimal)v2.X + " Y" + (decimal)v2.Y + " Z" + (decimal)v2.Z + " E" + (decimal)_machine.CurrentExtrusion + " R" + (decimal)r;
                }
            }
            else
            {
                if (CCW)
                {
                    gLine = "G3 X" + (decimal)v2.X + " Y" + (decimal)v2.Y + " Z" + (decimal)v2.Z + " R" + (decimal)r;
                }
                else
                {
                    gLine = "G2 X" + (decimal)v2.X + " Y" + (decimal)v2.Y + " Z" + (decimal)v2.Z + " R" + (decimal)r;
                }
            }
            if (VectorDistance(oldPosition, v2) > Math.Abs(r) * 2)
            {
                throw new Exception("AbsArc radius is too small.");
            }
            _stream.WriteLine(gLine);
        }
        private void SetExtruderTemp(double temp)
        {
            string gLine = "";
            _machine.ExtruderTemp = temp;
            gLine = "M104 S" + (decimal)temp;
            _stream.WriteLine(gLine);
        }
        private void SetFanPower(double power)
        {
            string gLine = "";
            _machine.FanPower = power;
            gLine = "M106 S" + Math.Floor(_machine.FanPower * 255);
            _stream.WriteLine(gLine);

        }
        private void SetExtrusionRate(double rate)
        {
            _machine.ExtrusionRate = rate;
        }
        private void SetBedTemp(double temp)
        {
            string gLine = "";
            _machine.BedTemp = temp;
            gLine = "M140 S" + (decimal)temp;
            _stream.WriteLine(gLine);
        }
        private Vector Position(){
            return _machine.Position;
        }
        private void Steps(double step)
        {
            Vector movement = new (Math.Cos(DegreesToRadians(_machine.Rotation))*step, 
                                   Math.Sin(DegreesToRadians(_machine.Rotation))*step,
                                   0);
            RelMove(movement);
        }
        private double DegreesToRadians(double degrees)
        {
           return degrees * Math.PI / 180.0f;
        }
        private void Lift(double step)
        {
            string gLine = "";
            _machine.Position.Z += step;
            if (_machine.Build)
            {
                _machine.CurrentExtrusion += _machine.ExtrusionRate * step;
                gLine += "G1 X" + (decimal)_machine.Position.X + " Y" + (decimal)_machine.Position.Y + " Z" + (decimal)_machine.Position.Z + " E" + (decimal)_machine.CurrentExtrusion;
            }
            else
            {
                gLine += "G0 X" + (decimal)_machine.Position.X + " Y" + (decimal)_machine.Position.Y + " Z" + (decimal)_machine.Position.Z;
            }
            _stream.WriteLine(gLine);
        }
        private void Right(double deg)
        {
            _machine.Rotation -= deg;
        }
        private void Left(double deg)
        {
            _machine.Rotation += deg;
        }
        private double Direction()
        {
            return _machine.Rotation;
        }
        private void TurnTo(double deg)
        {
            _machine.Rotation = deg;
        }
        private void WaitForBedTemp()
        {
            string gLine = "M190 R" + (decimal)(_machine.BedTemp);
            _stream.WriteLine(gLine);
        }
        private void WaitForExtruderTemp()
        {
            string gLine = "M109 R" + (decimal)(_machine.ExtruderTemp);
            _stream.WriteLine(gLine);
        }
        private void WaitForCurrentMove()
        {
            string gLine = "M400";
            _stream.WriteLine(gLine);
        }
        private void WaitForMillis(int millis)
        {
            string gLine = "G4 P" + millis;
            _stream.WriteLine(gLine);
        }
    }
}
