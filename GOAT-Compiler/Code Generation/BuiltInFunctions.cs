using System.Collections.Generic;

namespace GOAT_Compiler
{
    /// <summary>
    /// Contains the built in functions in the program, the move functions and nonmove functions.
    /// </summary>
    internal static class BuiltInFunctions
    {
        /// <summary>
        /// The Dictionary for built in functions, maps from a string to a Symbol
        /// </summary>
        internal static IReadOnlyDictionary<string, Symbol> FunctionsList => _functionsList;

        private static Dictionary<string, Symbol> _functionsList = new()
        {
            { "RelMove", new Symbol("RelMove", Types.Void, Types.Vector) },
            { "AbsMove", new Symbol("AbsMove", Types.Void, Types.Vector) },
            { "RelArcCW", new Symbol("RelArcCW", Types.Void, Types.Vector, Types.FloatingPoint) },
            { "AbsArcCW", new Symbol("AbsArcCW", Types.Void, Types.Vector, Types.FloatingPoint) },
            { "RelArcCCW", new Symbol("RelArcCCW", Types.Void, Types.Vector, Types.FloatingPoint) },
            { "AbsArcCCW", new Symbol("AbsArcCCW", Types.Void, Types.Vector, Types.FloatingPoint) },
            { "Position", new Symbol("Position", Types.Vector) },
            { "Steps", new Symbol("Steps", Types.Void, Types.FloatingPoint) },
            { "Lift", new Symbol("Lift", Types.Void, Types.FloatingPoint) },
            { "Right", new Symbol("Right", Types.Void, Types.FloatingPoint) },
            { "Left", new Symbol("Left", Types.Void, Types.FloatingPoint) },
            { "Direction", new Symbol("Direction", Types.FloatingPoint) },
            { "TurnTo", new Symbol("TurnTo", Types.Void, Types.FloatingPoint) },
            { "SetBedTemp", new Symbol("SetBedTemp", Types.Void, Types.FloatingPoint) },
            { "SetExtrusionRate", new Symbol("SetExtrusionRate", Types.Void, Types.FloatingPoint) },
            { "SetExtruderTemp", new Symbol("SetExtruderTemp", Types.Void, Types.FloatingPoint) },
            { "WaitForBedTemp", new Symbol("WaitForBedTemp", Types.Void) },
            { "WaitForExtruderTemp", new Symbol("WaitForExtruderTemp", Types.Void) },
            { "WaitForCurrentMove", new Symbol("WaitForCurrentMove", Types.Void) },
            { "WaitForMillis", new Symbol("WaitForMillis", Types.Void, Types.FloatingPoint) },
            { "SetFanPower", new Symbol("SetFanPower", Types.Void, Types.FloatingPoint) },
            { "Home", new Symbol("Home", Types.Void) }
        };
    }
}