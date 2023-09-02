using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphProblem
{
    public class Cycle: ArcCollection
    {

        public Cycle()
        {

        }

        public Cycle(IList<Arc> list) : base(list)
        {
            RemoveLeafs();
        }

        private void RemoveLeafs()
        {
            foreach(var node in Nodes)
            {
                int degree = GetNodeDegree(node);

                if (degree == 1)
                {
                    RemoveNodeArcs(node);
                }
            }
        }

        protected string NodesTemplate
        {
            get
            {
                var nodes = Nodes;
                var tmp = "";

                foreach (Node node in nodes)
                {
                    tmp += $"({node.Index})";

                    if (node != nodes.Last())
                    {
                        tmp += "-";
                    }
                }

                return tmp;
            }
        }

        public override string ToString()
        {
            return $"<Cycle nodes: {NodesTemplate}, arcs: {ArcsTemplate}>";
        }

        private int _ArcIndex(Arc arc)
        {
            int j = 0;

            foreach (Arc _arc in this)
            {
                if (arc == _arc)
                {
                    return j;
                }

                j += 1;
            }

            throw new Exception($"Arc not found: {arc}");
        }
    
        public bool HaveConcordantDirection(Arc arc1, Arc arc2)
        {
            if (arc1 == arc2 || arc1.Destination == arc2.Source || arc1.Source == arc2.Destination)
            {
                return true;
            }

            return IsReachable(arc1, arc2, new List<int>());
        }

        public bool IsReachable(Arc start, Arc target, List<int> visited)
        {
            if (start == target)
            {
                return true;
            }

            visited.Add(start.Indexes);

            List<Arc> next_arcs = this.Where(arc => arc.Source.Index == start.Destination.Index && arc.Indexes != start.Indexes).ToList();

            foreach (Arc next_arc in next_arcs)
            {
                if (!visited.Contains(next_arc.Indexes) && IsReachable(next_arc, target, visited))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
