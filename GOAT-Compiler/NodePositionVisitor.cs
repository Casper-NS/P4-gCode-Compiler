﻿using GOATCode.analysis;
using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    public class NodePosition
    {
        public int Line { get; }
        public int Character { get; }

        public NodePosition(int line, int character)
        {
            Line = line;
            Character = character;
        }
        public override string ToString()
        {
            if (!(Line >= 0 && Character >= 0))
            {
                return "(?, ?)";
            }
            return $"({Line}, {Character})";
        }
    }
    public class NodePositionVisitor : DepthFirstAdapter
    {
        public NodePosition GetPosition(Node node)
        {
            return positions[node];
        }
        public bool HasNode(Node node)
        {
            return positions.ContainsKey(node);
        }

        private Dictionary<Node, NodePosition> positions = new Dictionary<Node, NodePosition>();
        List<Node> notSet = new();
        public override void DefaultIn(Node node)
        {
            // mark node as having no line number assigned
            notSet.Add(node);
        }
        public override void DefaultCase(Node node)
        {
            if (node is Token token)
            {
                // update all nonassigned nodes to the position of this token
                foreach (Node notSetNode in notSet)
                {
                    positions.Add(notSetNode, new NodePosition(token.Line, token.Pos));
                }
                notSet.Clear();
            }
        }
    }
}