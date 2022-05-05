using System;

namespace GOAT_Compiler.Code_Generation
{
    /// <summary>
    /// The class that holds information about the CNC machine.
    /// </summary>
    internal class CNCMachine
    {
        private Vector _position = new Vector(0, 0, 0);
        private float _rotation = 0;
        private float _currentExtrusion = 0;
        private float _extruderRate = 0;
        private float _bedTemp = 0;
        private float _extruderTemp = 0;
        private float _fanPower = 0;
        private bool _build = false;

        /// <summary>
        /// The constructor of the CNCMachine class.
        /// </summary>
        public CNCMachine()
        {
        }

        /// <summary>
        /// Updates the current position of the machine.
        /// </summary>
        /// <param name="v">Takes a Vector.</param>
        public void UpdatePosition(Vector v)
        {
            _position = v;
        }

        /// <summary>
        /// Return the current position of the machine.
        /// </summary>
        /// <returns>Returns a Vector.</returns>
        public Vector ReturnPosition()
        {
            return _position;
        }

        /// <summary>
        /// Updates the current rotation of the machine.
        /// </summary>
        /// <param name="r">Takes a float.</param>
        public void UpdateRotation(float r)
        {
            _rotation = r;
        }

        /// <summary>
        /// Returns the current rotation of the machine.
        /// </summary>
        /// <returns>Returns a float.</returns>
        public float ReturnRotation()
        {
            return _rotation;
        }

        /// <summary>
        /// Updates the current extruder rate.
        /// </summary>
        /// <param name="e">Takes a float.</param>
        public void UpdateExtruderRate(float e)
        {
            _extruderRate = e;
        }

        /// <summary>
        /// Returns the current extruder rate.
        /// </summary>
        /// <returns>Returns a float.</returns>
        public float ReturnExtruderRate()
        {
            return _extruderRate;
        }

        /// <summary>
        /// Updates the current bed temperature.
        /// </summary>
        /// <param name="bedTemp">Takes a float.</param>
        public void UpdateBedTemp(float bedTemp)
        {
            _bedTemp = bedTemp;
        }

        /// <summary>
        /// Returns the current bed temperature.
        /// </summary>
        /// <returns></returns>
        public float ReturnBedTemp()
        {
            return _bedTemp;
        }

        /// <summary>
        /// Updates the current extruder temperature.
        /// </summary>
        /// <param name="extruderTemp">Takes a float.</param>
        public void UpdateExtruderTemp(float extruderTemp)
        {
            _extruderTemp = extruderTemp;
        }

        /// <summary>
        /// Returns the current extruder temoerature.
        /// </summary>
        /// <returns>Returns a float.</returns>
        public float ReturnExtruderTemp()
        {
            return _extruderTemp;
        }

        /// <summary>
        /// Updates the fan power.
        /// </summary>
        /// <param name="fanPower">Takes a float.</param>
        public void UpdateFanPower(float fanPower)
        {
            _fanPower = fanPower;
        }

        /// <summary>
        /// Returns the fan power.
        /// </summary>
        /// <returns>Returns a float.</returns>
        public float ReturnFanPower()
        {
            return _fanPower;
        }

        public void AddToExtrusion(float e)
        {
            _currentExtrusion += e;
        }

        public bool GetMode()
        {
            return _build;
        }
        public void SetMode(bool b)
        {
            _build = b;
        }

        #region BuildInMethods
        public void RelMove(Vector v)
        {
            string gLine = "";
            Vector v1 = ReturnPosition();
            UpdatePosition(new Vector(v1.X + v.X, v1.Y + v.Y, v1.Z + v.Z));
            if (GetMode())
            {
                AddToExtrusion(ReturnExtruderRate());
                gLine = "G1 X" + ReturnPosition().X + " Y" + ReturnPosition().Y + " Z" + ReturnPosition().Z + " E" + ReturnExtruderRate();
            }
            else
            {
                gLine = "G1 X" + ReturnPosition().X + " Y" + ReturnPosition().Y + " Z" + ReturnPosition().Z;
            }
        }
        public void AbsMove(Vector v)
        {
            string gLine = "";
            UpdatePosition(v);
            if (GetMode())
            {
                AddToExtrusion(ReturnExtruderRate());
                gLine = "G1 X" + ReturnPosition().X + " Y" + ReturnPosition().Y + " Z" + ReturnPosition().Z + " E" + ReturnExtruderRate();
            }
            else
            {
                gLine = "G1 X" + ReturnPosition().X + " Y" + ReturnPosition().Y + " Z" + ReturnPosition().Z;
            }
        }
        public void RelArc(Vector v, float r)
        {
            string gLine = "";
            Vector v1 = ReturnPosition();
            if (VectorDistance(v1, v) > r*2)
            {
                throw new Exception("RelArc radius is too small.");
            }
            UpdatePosition(new Vector(v1.X + v.X, v1.Y + v.Y, v1.Z + v.Z));
            Vector v2 = ReturnPosition();
            if (GetMode())
            {
                if(r < 0) { 
                    AddToExtrusion(ReturnExtruderRate());
                    gLine = "G3 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + ReturnExtruderRate() + " R" + -r;
                }
                else
                {
                    AddToExtrusion(ReturnExtruderRate());
                    gLine = "G2 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + ReturnExtruderRate() + " R" + r;
                }
            }
            else
            {
                if (r < 0)
                {
                    AddToExtrusion(ReturnExtruderRate());
                    gLine = "G3 X" + "X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + ReturnPosition().Z + " R" + -r;
                }
                else
                {
                    AddToExtrusion(ReturnExtruderRate());
                    gLine = "G2 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + ReturnPosition().Z + " R" + r;
                }
            }
        }
        private float VectorDistance(Vector v1, Vector v2) 
        {
            return (float)Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2) + Math.Pow(v1.Z - v2.Z, 2));
        }

        public void AbsArc(Vector v, float r)
        {
            string gLine = "";
            Vector v1 = ReturnPosition();
            if (VectorDistance(v1, v) > r * 2)
            {
                throw new Exception("AbsArc radius is too small.");
            }
            UpdatePosition(v);
            Vector v2 = ReturnPosition();
            if (GetMode())
            {
                if (r < 0)
                {
                    AddToExtrusion(ReturnExtruderRate());
                    gLine = "G3 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + ReturnExtruderRate() + " R" + -r;
                }
                else
                {
                    AddToExtrusion(ReturnExtruderRate());
                    gLine = "G2 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + " E" + ReturnExtruderRate() + " R" + r;
                }
            }
            else
            {
                if (r < 0)
                {
                    AddToExtrusion(ReturnExtruderRate());
                    gLine = "G3 X" + "X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + ReturnPosition().Z + " R" + -r;
                }
                else
                {
                    AddToExtrusion(ReturnExtruderRate());
                    gLine = "G2 X" + v2.X + " Y" + v2.Y + " Z" + v2.Z + ReturnPosition().Z + " R" + r;
                }
            }
        }
        public void SetExtruderTemp(float temp)
        {
            string gLine = "";
            UpdateExtruderTemp(temp);
            gLine = "M104 S" + ReturnExtruderTemp().ToString();
        }
        public void SetFanPower(float power)
        {
            string gLine = "";
            UpdateFanPower(power);
            gLine = "M106 S" + ReturnFanPower().ToString();
        }
        public void SetExtruderRate(float rate)
        {
            string gLine = "";
            UpdateExtruderRate(rate);
            gLine = "M220 S" + ReturnExtruderRate().ToString();
        }
        public void SetBedTemp(float temp)
        {
            string gLine = "";
            UpdateBedTemp(temp);
            gLine = "M140 S" + ReturnBedTemp().ToString();
        }
        public Vector Position(){
            return ReturnPosition();
        }
        public void Steps(float step)//Needs more
        {
            //string gLine = "";
            //UpdatePosition(new Vector(ReturnPosition().X + step, ReturnPosition().Y + step, ReturnPosition().Z + step));
        }
        public void Lift(float step)
        {
            
        }
        public void Right(float deg)
        {
            
        }
        public void Left(float deg)
        {
            
        }
        public float Direction()
        {
            return ReturnRotation();
        }
        public void TurnTo(float deg)
        {
            
        }
        public void WaitForBedTemp()
        {
        }
        public void WaitForExtruderTemp()
        {
        }
        public void WaitForCurrentMove()
        {
            
        }
        public void WaitForMillis()
        {
            
        }
        #endregion
    }
}
