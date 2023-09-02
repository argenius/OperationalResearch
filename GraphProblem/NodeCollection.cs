using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProblem
{
    public class NodeCollection: Collection<Node>
    {
        public NodeCollection()
        {
        }

        public NodeCollection(IList<Node> list) : base(list)
        {

        }

        public override string ToString()
        {
            return $"<NodeCollection({Count}): {NodesTemplate}>";
        }

        private string NodesTemplate
        {
            get
            {
                var template = "";

                var i = 0;

                foreach (Node node in this)
                {
                    template += $"<Node {node.Index}>";

                    if (i < Count - 1)
                    {
                        template += ", ";
                    }

                    i++;
                }

                return template;
            }
        }

        public Node? First
        {
            get
            {
                var nodes = OrderByIndex();

                if (nodes == null || Count == 0)
                {
                    return null;
                }

                return nodes[0];
            }
        }

        public int BalancesSum
        {
            get {
                int sum = 0;

                foreach (Node node in this)
                {
                    sum += node.Balance;
                }

                return sum;
            }
        }

        public void UpdateNodeBalance(int index, int balance)
        {
            foreach(Node node in this)
            {
                if (node.Index == index)
                {
                    node.Balance = balance;
                    return;
                }
            }
        }

        public new void Remove(Node node)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == node)
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        public void Remove(int nodeIndex)
        {
            RemoveAt(nodeIndex - 1);
        }

        public new bool Contains(Node node)
        {
            foreach (Node n in this)
            {
                if (n == node)
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool Contains(int nodeIndex)
        {
            foreach (Node n in this)
            {
                if (n == nodeIndex)
                {
                    return true;
                }
            }
            return false;
        }

        public new void Add(Node node)
        {
            if (Contains(node))
                return;

            base.Add(node);
        }

        public Node? ExtractMinimalPotential(DataTable potential)
        {
            var nodes = OrderByIndex();
            Node? minNode = null;
            var minValue = int.MaxValue;
            var row = potential.Rows[0];

            foreach(Node node in nodes)
            {
                var idx = node.Index.ToString();

                if (minValue > (int)row[idx])
                {
                    minValue = (int)row[idx];

                    minNode = node;
                }
            }

            if (minNode is not null)
                Remove(minNode);

            return minNode;
        }

        public Node? GetNodeByINdex(int nodeIndex)
        {
            foreach(Node node in this)
            {
                if (node == nodeIndex)
                {
                    return node;
                }
            }

            return null;
        }

        public NodeCollection Clone()
        {
            var clone = new NodeCollection();

            foreach (Node node in this)
            {
                clone.Add(node.Clone());
            }
            return clone;
        }
    
        public NodeCollection OrderByIndex()
        {
            return new NodeCollection(this.OrderBy(n => n.Index).ToList());
        }
    }

}
