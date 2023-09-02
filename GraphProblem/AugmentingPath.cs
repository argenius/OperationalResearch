using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GraphProblem
{
    internal class AugmentingPath: Graph
    {
        private int RootIndex;
        public override string ToString()
        {
            return $"<AugmentingPath {NodesTemplate}>";
        }
        public AugmentingPath(string? label = null): base(label)
        {
            RootIndex = -1;
        }

        public AugmentingPath(NodeCollection nodes, ArcCollection arcs, bool clone = false, string? label = null): base(nodes, arcs, clone, label)
        {
            RootIndex = -1;
        }

        public AugmentingPath(Node node, Arc? arc = null, bool clone = false, string? label = null): base(node, arc, clone, label)
        {
            RootIndex = -1;
        }

        public void SetRootIndex(int index)
        {
            RootIndex = index;
        }

        public Node? RootNode
        {
            get
            {
                return GetNode(RootIndex);
            }
        }

        private string NodesTemplateRecursive(Node? node = null)
        {
            if (node is null)
            {
                if (RootNode is null)
                {
                    return "";
                }

                node = RootNode;
            }

            var outgoingNode = GetOutgoingNodeNodes(node).First;

            var template = $"({node.Index})";

            if (outgoingNode is not null)
            {
                template += "-";
                template += NodesTemplateRecursive(outgoingNode);
            }

            return template;
        }

        private string NodesTemplate
        {
            get
            {
                var template = "";

                if (RootNode is null)
                {

                    var i = 0;
                    foreach (var node in Nodes)
                    {
                        i++;
                        template += $"({node.Index})";

                        if (i < Nodes.Count)
                        {
                            template += "-";
                        }
                    }
                }

                else
                {
                    return NodesTemplateRecursive();
                }

                return template;
            }
        }

    }
}
