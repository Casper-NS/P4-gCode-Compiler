using System;

namespace GOAT_Compiler.Code_Generation
{
    /// <summary>
    /// The class that holds information about the CNC machine.
    /// </summary>
    public class CNCMachine
    {
        private float _currentExtrusion = 0;
        private float _bedTemp = 0;
        private float _extruderTemp = 0;
        private float _fanPower = 0;

        /// <summary>
        /// The current positon of the extruder.
        /// </summary>
        public Vector Position { get; set; } = new Vector(0, 0, 0);

        /// <summary>
        /// The current extrusion.
        /// </summary>
        public float CurrentExtrusion
        {
            get
            {
                return _currentExtrusion;
            } 
            set 
            {
                if (value < 0)
                {
                    throw new Exception("Extrusion cannot be negative.");
                }
                _currentExtrusion = value;
            } 
        }

        /// <summary>
        /// The current extruder rate.
        /// </summary>
        public float ExtruderRate { get; set; } = 0;
        
        /// <summary>
        /// The current bed temperature.
        /// </summary>        
        public float BedTemp
        {
            get
            {
                return _bedTemp;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Bed temperature cannot be negative.");
                }
                _bedTemp = value;
            }
        }

        /// <summary>
        /// The current extruder temperature.
        /// </summary>
        public float ExtruderTemp 
        {
            get
            {
                return _extruderTemp;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Extruder temperature cannot be negative.");
                }
                _extruderTemp = value;
            }
        }

        /// <summary>
        /// The current fan power.
        /// </summary>
        public float FanPower 
        {
            get
            {
                return _fanPower;
            }
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new Exception("Fan power must be between 0 and 255.");
                }
                _fanPower = value;
            }
        }

        /// <summary>
        /// The current extruder state.
        /// </summary>
        public bool Build { get; set; } = false;

        /// <summary>
        /// The current rotation of the tutle.
        /// </summary>
        public float Rotation { get; set; } = 0;

    }
}
