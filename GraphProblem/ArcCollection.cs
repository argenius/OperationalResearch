using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProblem
{
    public class ArcCollection: Collection<Arc>
    {


        public ArcCollection()
        {

        }

        public ArcCollection(IList<Arc> list) : base(list)
        {

        }

        public ArcCollection(DataTable positions)
        {
            var i = 1;

            var row = positions.Rows[0];

            foreach(DataColumn column in positions.Columns)
            {
                var pred = (int)row[column.ColumnName];

                if (pred == i)
                {
                    i++;
                    continue;
                }

                Add(new Arc(new Node(pred, 1), new Node(i, 1)));

                i++;
            }
        }

        public new void Add(Arc arc)
        {
            if (!Contains(arc))
            {
                base.Add(arc);
            }
        }

        protected string ArcsTemplate
        {
            get
            {
                var tmp = "";

                foreach (Arc arc in this)
                {
                    tmp += $"({arc.Source.Index},{arc.Destination.Index})";

                    if (arc != this.Last())
                    {
                        tmp += ", ";
                    }
                }

                return tmp;
            }
        }

        public override string ToString()
        {
            return $"<ArcCollection({Count}): {ArcsTemplate}>";
        }

        public List<int> IndexesList
        {
            get
            {
                var arcs = OrderByIndex();
                var indexes = new List<int>();

                foreach (var arc in arcs)
                {
                      indexes.Add(arc.Indexes);
                }

                return indexes;
            }
        }

        public ArcCollection OrderByIndex()
        {
            return new ArcCollection(this.OrderBy(a => a.Indexes).ToList());
        }

        public Arc? GetFirstMatchResidualCapacity(int capacity)
        {
            var arcs = OrderByIndex();

            foreach (Arc arc in arcs)
            {
                if (arc.ResidualCapacity == capacity)
                {
                    return arc;
                }
            }

            return null;
        }

        public Arc? GetFirstMatchFlow(int flow)
        {
            var arcs = OrderByIndex();

            foreach (Arc arc in arcs)
            {
                if (arc.Flow == flow)
                {
                    return arc;
                }
            }

            return null;
        }

        public int GetNodeDegree(Node node)
        {
            var outArcs = GetNodeArcs(node);
            return outArcs.Count;
        }

        public new void Remove(Arc arc)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == arc)
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        public void Remove(int indexes)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Indexes == indexes)
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveNodeArcs(Node node)
        {
            var arcs = GetNodeArcs(node);

            foreach (Arc arc in arcs)
            {
                Remove(arc);
            }
        }

        public Arc? First
        {
            get
            {
                var col = OrderByIndex();

                if (col.Count > 0)
                {
                    return col[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public new bool Contains(Arc arc)
        {
            foreach (Arc a in this)
            {
                if (a.Source == arc.Source && a.Destination == arc.Destination)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(int indexes)
        {
            foreach (Arc a in this)
            {
                if (a.Indexes == indexes)
                {
                    return true;
                }
            }

            return false;
        }

        public ArcCollection EmptyArcs
        {
            get
            {
                var col = new ArcCollection();

                foreach (Arc arc in this)
                {
                    if (arc.Empty)
                    {
                        col.Add(arc);
                    }
                }

                return col;
            }
        }

        public ArcCollection SaturatedArcs
        {
            get
            {
                var col = new ArcCollection();

                foreach (Arc arc in this)
                {
                    if (arc.Saturated)
                    {
                        col.Add(arc);
                    }
                }

                return col;
            }
        }

        public bool HasSaturatedArcs
        {
            get
            {
                return SaturatedArcs.Count > 0;
            }
        }

        public bool HasEmptyArcs
        {
            get
            {
                foreach (Arc arc in this)
                {
                    if (arc.Empty)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public NodeCollection Nodes
        {
            get
            {
                var col = new NodeCollection();

                foreach (Arc arc in this)
                {
                    if (!col.Contains(arc.Source))
                    {
                        col.Add(arc.Source);
                    }

                    if (!col.Contains(arc.Destination))
                    {
                        col.Add(arc.Destination);
                    }
                }

                return col;
            }
        }

        public ArcCollection GetNodeArcs(Node node)
        {
            var col = new ArcCollection();

            foreach (Arc arc in this)
            {
                if (arc.Source == node || arc.Destination == node)
                {
                    col.Add(arc);
                }
            }

            return col;
        }

        public ArcCollection GetIncomingNodeArcs(Node node)
        {
            var col = new ArcCollection();

            foreach (Arc arc in this)
            {
                if (arc.Destination == node)
                {
                    col.Add(arc);
                }
            }

            return col;
        }

        public ArcCollection GetIncomingNodeArcs(int nodeIndex)
        {
            var col = new ArcCollection();

            foreach (Arc arc in this)
            {
                if (arc.Destination == nodeIndex)
                {
                    col.Add(arc);
                }
            }

            return col;
        }

        public ArcCollection GetOutgoingNodeArcs(Node node)
        {
            var col = new ArcCollection();

            foreach (Arc arc in this)
            {
                if (arc.Source == node)
                {
                    col.Add(arc);
                }
            }

            return col;
        }

        public ArcCollection GetOutgoingNodeArcs(int nodeIndex)
        {
            var col = new ArcCollection();

            foreach (Arc arc in this)
            {
                if (arc.Source == nodeIndex)
                {
                    col.Add(arc);
                }
            }

            return col;
        }

        public void ResetFlow(ArcCollection? exceptionList = null)
        {
            if (exceptionList is null)
            {
                exceptionList = new ArcCollection();
            }
            foreach (Arc arc in this)
            {
                if (!exceptionList.Contains(arc))
                {
                    arc.Flow = 0;
                }
            }
        }

        public ArcCollection Clone()
        {
            var clone = new ArcCollection();

            foreach (Arc arc in this)
            {
                clone.Add(arc.Clone());
            }
            return clone;
        }
    
    }
}
