using System;

namespace GOAT_Compiler.Code_Generation
{

    public enum ExtrusionMode
    {
        build,
        walk,
        none
    }


    /// <summary>
    /// The class that holds information about the CNC machine.
    /// </summary>
    public class CNCMachine
    {
        private double _currentExtrusion = 0;
        private double _hotBedTemp = 0;
        private double _extruderTemp = 0;
        private double _fanPower = 0;

        /// <summary>
        /// The current positon of the extruder.
        /// </summary>
        public Vector Position { get; set; } = new Vector(0, 0, 0);

        /// <summary>
        /// The current amount of extrusion: totaltDistance*extrusionRate.
        /// </summary>
        public double CurrentExtrusion
        {
            get
            {
                return _currentExtrusion;
            } 
            set 
            {
                if (value < 0)
                {
                    throw new Exception("The amount of extrusion cannot be negative.");
                }
                _currentExtrusion = value;
            } 
        }

        /// <summary>
        /// The current extruder rate.
        /// </summary>
        public double ExtrusionRate { get; set; } = 0;
        
        /// <summary>
        /// The current hot-bed temperature.
        /// </summary>        
        public double HotBedTemp
        {
            get
            {
                return _hotBedTemp;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("The hot-bed temperature cannot be negative.");
                }
                _hotBedTemp = value;
            }
        }

        /// <summary>
        /// The current extruder temperature.
        /// </summary>
        public double ExtruderTemp 
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
        public double FanPower 
        {
            get
            {
                return _fanPower;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new Exception("Fan power must be between 0 and 1.");
                }
                _fanPower = value;
            }
        }

        /// <summary>
        /// The current extruder mode, (build/walk/none).
        /// </summary>
        public ExtrusionMode ExtrusionMode { get; set; } = ExtrusionMode.none;

        /// <summary>
        /// The current rotation of the tutle, where 0 degrees are the positive x-axis.
        /// </summary>
        public double Rotation { get; set; } = 0;

    }
}
