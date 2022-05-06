namespace GOAT_Compiler.Code_Generation
{
    /// <summary>
    /// The class that holds information about the CNC machine.
    /// </summary>
    public class CNCMachine
    {
        public Vector Position { get; set; } = new Vector(0, 0, 0);
        public float CurrentExtrusion { get; set; } = 0;
        public float ExtruderRate { get; set; } = 0;
        public float BedTemp { get; set; } = 0;
        public float ExtruderTemp { get; set; } = 0;
        public float FanPower { get; set; } = 0;
        public bool Build { get; set; } = false;
        public float Rotation { get; set; } = 0;


        /// <summary>
        /// The constructor of the CNCMachine class.
        /// </summary>
        public CNCMachine()
        {
        }
    }
}
