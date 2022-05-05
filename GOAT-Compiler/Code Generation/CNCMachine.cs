using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
