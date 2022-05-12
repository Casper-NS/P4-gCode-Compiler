using GOATCode.analysis;
using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    /// <summary>
    /// NodePosition stores the line number and character number of the node in the source-code.
    /// </summary>
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
    /// <summary>
    /// Adds NodePosition to nodes. This is used for exceptions.
    /// </summary>
    public class NodePositionVisitor : DepthFirstAdapter
    {
        public NodePosition GetPosition(Node node) => positions[node];
        public bool HasNode(Node node) => positions.ContainsKey(node);
        /// <summary>
        /// Dictionary from node to a NodePosition, this position is used for exceptions (line nr. and char nr.).
        /// </summary>
        private readonly Dictionary<Node, NodePosition> positions = new Dictionary<Node, NodePosition>();
        /// <summary>
        /// A list of nodes that have no position yet.
        /// </summary>
        private readonly List<Node> notSet = new();
        /// <summary>
        /// Marks node as having no line number assigned
        /// </summary>
        /// <param name="node"></param>
        public override void DefaultIn(Node node) => notSet.Add(node);
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
