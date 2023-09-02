using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProblem
{
    public class Node
    {
        public int Index { get; set; }

        public int Balance { get; set; }

        public bool Producer
        {
            get
            {
                return Balance < 0;
            }
        }

        public bool Consumer
        {
            get
            {
                return Balance > 0;
            }
        }

        public Node(int index, int balance)
        {
            Index = index;
            Balance = balance;
        }

        public Node Clone()
        {
            return new Node(Index, Balance);
        }

        private string SignTemplate
        {
            get
            {
                if (Consumer)
                {
                    return "+";
                }

                else if (!Producer)
                {
                    return " ";
                }

                return "";
            }
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            if (this.Index == ((Node)obj).Index)
            {
                return true;
            }

            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return Index*10 + Balance;
        }
    
        public override string ToString()
        {
            return $"<Node ({Index}), b: {SignTemplate}{Balance}>";
        }

        public static bool operator !=(Node node1, Node node2)
        {
            return node1.Index != node2.Index;
        }

        public static bool operator ==(Node node1, Node node2)
        {
            return node1.Index == node2.Index;
        }

        public static bool operator ==(Node node1, int index)
        {
            return node1.Index == index;
        }

        public static bool operator !=(Node node1, int index)
        {
            return node1.Index != index;
        }


    }

    internal static class NodeFactory
    {
        public static Node Create(int index, int balance)
        {
            return new Node(index, balance);
        }
    }
}
