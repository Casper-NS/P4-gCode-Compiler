using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GOAT_Compiler.Exceptions;

namespace GOAT_Compiler.Code_Generation
{
    /// <summary>
    /// The class with the implentations of the built in functions
    /// </summary>
    public class BuildInFunctionImplementations
    {
        private readonly CNCMachine _machine;
        private readonly TextWriter _stream;
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
        /// <param name="actualParams">The params for the built in function.</param>
        /// <returns></returns>
        public dynamic CallBuildInFunctions(string functionName, List<dynamic> actualParams)
        {
            switch (functionName)
            {
                case "RelMove":
                    RelMove(actualParams[0]);
                    break;
                case "AbsMove":
                    AbsMove(actualParams[0]);
                    break;
                case "RelArcCW":
                    RelArc(actualParams[0], actualParams[1], false);
                    break;
                case "RelArcCCW":
                    RelArc(actualParams[0], actualParams[1], true);
                    break;
                case "AbsArcCW":
                    AbsArc(actualParams[0], actualParams[1], false);
                    break;
                case "AbsArcCCW":
                    AbsArc(actualParams[0], actualParams[1], true);
                    break;
                case "SetExtrusionRate":
                    SetExtrusionRate(actualParams[0]);
                    break;
                case "SetBedTemp":
                    SetBedTemp(actualParams[0]);
                    break;
                case "SetExtruderTemp":
                    SetExtruderTemp(actualParams[0]);
                    break;
                case "SetFanPower":
                    SetFanPower(actualParams[0]);
                    break;
                case "Steps":
                    Steps(actualParams[0]);
                    break;
                case "Position":
                    return Position();
                case "Lift":
                    Lift(actualParams[0]);
                    break;
                case "Right":
                    Right(actualParams[0]);
                    break;
                case "Left":
                    Left(actualParams[0]);
                    break;
                case "Direction":
                    return Direction();
                case "TurnTo":
                    TurnTo(actualParams[0]);
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
                    WaitForMillis(actualParams[0]);
                    break;
                case "Home":
                    Home();
                    break;
            }
            return null;
        }
        private string VectorToGCodeStringCoordinate(Vector v) => $"X{(decimal)v.X} Y{(decimal)v.Y} Z{(decimal)v.Z}";
        private void Home()
        {
            _machine.Position = new Vector(0, 0, 0);
            _stream.WriteLine("G28");
        }

        private void RelMove(Vector v)
        {
            string gLine;
            Vector oldPosition = _machine.Position;
            _machine.Position = oldPosition + v;
            ThrowExceptionIfInNoneScope("Relmove");
            if (_machine.ExtrusionMode == ExtrusionMode.build)
            {
                _machine.CurrentExtrusion += (_machine.ExtrusionRate*VectorDistance(oldPosition, _machine.Position));
                gLine = "G1 " + VectorToGCodeStringCoordinate(_machine.Position) + " E" + (decimal)_machine.CurrentExtrusion;
            }
            else
            {
                gLine = "G0 " + VectorToGCodeStringCoordinate(_machine.Position);
            }
            _stream.WriteLine(gLine);
        }

        void ThrowExceptionIfInNoneScope(string MovementFunctionName)
        {
            if (_machine.ExtrusionMode == ExtrusionMode.none)
            {
                throw new MoveWithoutScopeException($"{MovementFunctionName} cant be called without being in a build or walk scope.");
            }
        }

        private void AbsMove(Vector v)
        {
            string gLine;
            Vector oldPosition = _machine.Position;
            _machine.Position = v;
            ThrowExceptionIfInNoneScope("Absmove");
            if (_machine.ExtrusionMode == ExtrusionMode.build)
            {
                _machine.CurrentExtrusion += (_machine.ExtrusionRate * VectorDistance(oldPosition, _machine.Position));
                gLine = "G1 " + VectorToGCodeStringCoordinate(_machine.Position) + " E" + (decimal)_machine.CurrentExtrusion;
            }
            else
            {
                gLine = "G0 " + VectorToGCodeStringCoordinate(_machine.Position);
            }
            _stream.WriteLine(gLine);
        }
        private double CircleLength(Vector v1, Vector v2, double r)
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
            string gLine;
            Vector oldPosition = _machine.Position;
            _machine.Position = oldPosition + v;
            Vector v2 = _machine.Position;
            ThrowExceptionIfInNoneScope("RelArc");
            if (_machine.ExtrusionMode == ExtrusionMode.build)
            {
                if(CCW) { 
                    _machine.CurrentExtrusion += (_machine.ExtrusionRate * CircleLength(oldPosition, _machine.Position, r));
                    gLine = "G3 " + VectorToGCodeStringCoordinate(_machine.Position) + " E" + (decimal)_machine.CurrentExtrusion + " R" + (decimal)r;
                }
                else
                {
                    _machine.CurrentExtrusion += (_machine.ExtrusionRate * CircleLength(oldPosition, _machine.Position, r));
                    gLine = "G2 " + VectorToGCodeStringCoordinate(_machine.Position) + " E" + (decimal)_machine.CurrentExtrusion + " R" + (decimal)r;
                }
            }
            else
            {
                if (CCW)
                {
                    gLine = "G3 " + VectorToGCodeStringCoordinate(_machine.Position) + " R" + (decimal)r;
                }
                else
                {
                    gLine = "G2 " + VectorToGCodeStringCoordinate(_machine.Position) + " R" + (decimal)r;
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
            string gLine;
            Vector oldPosition = _machine.Position;
            _machine.Position = v;
            ThrowExceptionIfInNoneScope("AbsArc");
            if (_machine.ExtrusionMode == ExtrusionMode.build)
            {
                if (CCW)
                {
                    _machine.CurrentExtrusion += (_machine.ExtrusionRate * CircleLength(oldPosition, _machine.Position, r));
                    gLine = "G3 " + VectorToGCodeStringCoordinate(_machine.Position) + " E" + (decimal)_machine.CurrentExtrusion + " R" + (decimal)r;
                }
                else
                {
                    _machine.CurrentExtrusion += (_machine.ExtrusionRate * CircleLength(oldPosition, _machine.Position, r));
                    gLine = "G2 " + VectorToGCodeStringCoordinate(_machine.Position) + " E" + (decimal)_machine.CurrentExtrusion + " R" + (decimal)r;
                }
            }
            else
            {
                if (CCW)
                {
                    gLine = "G3 " + VectorToGCodeStringCoordinate(_machine.Position) + " R" + (decimal)r;
                }
                else
                {
                    gLine = "G2 " + VectorToGCodeStringCoordinate(_machine.Position) + " R" + (decimal)r;
                }
            }
            if (VectorDistance(oldPosition, _machine.Position) > Math.Abs(r) * 2)
            {
                throw new Exception("AbsArc radius is too small.");
            }
            _stream.WriteLine(gLine);
        }
        private void SetExtruderTemp(double temp)
        {
            _machine.ExtruderTemp = temp;
            _stream.WriteLine("M104 S" + (decimal)temp);
        }
        private void SetFanPower(double power)
        {
            _machine.FanPower = power;
            _stream.WriteLine("M106 S" + Math.Floor(_machine.FanPower * 255));
        }
        private void SetExtrusionRate(double rate) => _machine.ExtrusionRate = rate;
        private void SetBedTemp(double temp)
        {
            _machine.HotBedTemp = temp;
            _stream.WriteLine("M140 S" + (decimal)temp);
        }
        private Vector Position() => _machine.Position;
        private void Steps(double step)
        {
            Vector movement = new (Math.Cos(DegreesToRadians(_machine.Rotation))*step, 
                                   Math.Sin(DegreesToRadians(_machine.Rotation))*step,
                                   0);
            RelMove(movement);
        }
        private double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0f;
        private void Lift(double step)
        {
            string gLine;
            _machine.Position.Z += step;
            ThrowExceptionIfInNoneScope("Lift");
            if (_machine.ExtrusionMode == ExtrusionMode.build)
            {
                _machine.CurrentExtrusion += _machine.ExtrusionRate * step;
                gLine = "G1 " + VectorToGCodeStringCoordinate(_machine.Position) + " E" + (decimal)_machine.CurrentExtrusion;
            }
            else
            {
                gLine = "G0 " + VectorToGCodeStringCoordinate(_machine.Position);
            }
            _stream.WriteLine(gLine);
        }
        private void Right(double deg) => _machine.Rotation -= deg;
        private void Left(double deg) => _machine.Rotation += deg;
        private double Direction() => _machine.Rotation;
        private void TurnTo(double deg) => _machine.Rotation = deg;
        private void WaitForBedTemp() => _stream.WriteLine("M190 R" + (decimal)_machine.HotBedTemp);
        private void WaitForExtruderTemp() => _stream.WriteLine("M109 R" + (decimal)_machine.ExtruderTemp);
        private void WaitForCurrentMove() => _stream.WriteLine("M400");
        private void WaitForMillis(int millis) => _stream.WriteLine("G4 P" + millis);
    }
}
