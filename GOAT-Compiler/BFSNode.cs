﻿using System.Collections.Generic;

namespace GOAT_Compiler
{
    /// <summary>
    /// This class represents a functioncall from a BFSnode.
    /// </summary>
    internal struct FunctionCall
    {
        internal FunctionCall(BFSNode bfsnode, Extrude e)
        {
            BFSNode = bfsnode;
            extrudeType = e;
        }
        internal BFSNode BFSNode;
        internal Extrude extrudeType;
    }

    /// <summary>
    /// BFSNode represents a function with a name, a list of function calls, and what node it came from.
    /// </summary>
    internal class BFSNode
    {
        internal string Name { get; private set; }

        internal List<FunctionCall> FunctionCalls { get; private set; } = new();

        internal Extrude ExtrudeType { get; set; }

        internal Extrude TheExtrudeTypeFromCallStack { get; set; } = Extrude.NotSet;
        
        internal BFSNode TheNodeThatCalledThisOne { get; set; }

        internal BFSNode(string n, Extrude extrude)
        {
            Name = n;
            ExtrudeType = extrude;
        }

        internal void AddFunctionCall(BFSNode dn, Extrude e)
        {
            FunctionCalls.Add(new FunctionCall(dn, e));
        }
    }
}
