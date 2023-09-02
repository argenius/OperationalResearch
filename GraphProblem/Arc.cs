using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphProblem
{
    public class Arc
    {
        public Node Source { get; set; }

        public Node Destination { get; set; }
        
        public int Cost { get; set; }
        
        public int Capacity { get; set; }

        public int Flow { get; set; }

        public int ResidualCapacity { 
            get { return Capacity - Flow; }
        }

        public bool Saturated
        {
            get { return ResidualCapacity == 0; }
        }

        public bool Empty
        {
            get
            {
                return Flow == 0;
            }
        }

        public int Indexes
        {
            get { return Source.Index * 10 + Destination.Index; }
        }

        public Arc(Node source, Node destination, int cost = int.MaxValue, int capacity = 0, int flow = 0)
        {
            Source = source;
            Destination = destination;
            Cost = cost;
            Capacity = capacity;
            Flow = flow;
        }

        public bool IsIncoming(Node node)
        {
            return Destination == node;
        }

        public bool IsOutgoing(Node node)
        {
            return Source == node;
        }

        public static bool operator == (Arc arc1, Arc arc2)
        {
            return arc1.Source == arc2.Source && arc1.Destination == arc2.Destination;
        }

        public static bool operator != (Arc arc1, Arc arc2)
        {
            return arc1.Source != arc2.Source || arc1.Destination != arc2.Destination;
        }

        public static bool operator == (Arc arc, int indexes)
        {
            return arc.Indexes == indexes;
        }

        public static bool operator != (Arc arc, int indexes)
        {
            return arc.Indexes != indexes;
        }

        public Arc Clone()
        {
            return new Arc(Source, Destination, Cost, Capacity);
        }

        public override string ToString()
        {
            return $"<Arc ({Source.Index},{Destination.Index})>";
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

            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return Source.Index * 10 + Destination.Index;
        }

    }
}
