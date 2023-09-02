using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphProblem
{


    internal class DijkstraTree: Graph
    {
        public int RootIndex;

        public DataTable MinFlow(DataTable dt)
        {
            dt.TableName = "FLUSSO OTTIMO CAMMINI MINIMI X";

            var row = dt.NewRow();
            var indexesList = new List<int>();

            foreach (Arc arc in Arcs)
            {
                indexesList.Add(arc.Indexes);
                row[arc.Indexes.ToString()] = arc.Flow;
            }

            foreach (DataColumn col in dt.Columns)
            {
                var idx = int.Parse(col.ColumnName);

                if (!indexesList.Contains(idx))
                {
                    row[idx.ToString()] = 0;
                }
            }

            dt.Rows.Add(row);

            return dt;
        }

        public DijkstraTree(int rootIndex, NodeCollection nodes, ArcCollection arcs, bool loadFlow = true) : base(nodes, arcs, true, "ALBERO DEI CAMMINI MINIMI")
        {
            RootIndex = rootIndex;

            SetNodeBalances();
            ResetFlow();
            if (loadFlow)
                LoadFlow();
        }

        private void SetNodeBalances()
        {
            foreach (Node node in Nodes)
            {
                if (node.Index == RootIndex)
                {
                    node.Balance = Nodes.Count - 1;
                }
                else
                {
                    node.Balance = 1;
                }

            }
        }

        private void ResetFlow()
        {
            foreach (Arc arc in Arcs)
            {
                arc.Flow = 0;
            }
        }

        public override DijkstraTree Clone()
        {
            return new DijkstraTree(RootIndex, Nodes, Arcs, false);
        }

        private void UpdateArcFlow(Arc arc, int flow)
        {
            foreach (Arc a in Arcs)
            {
                if (a == arc)
                {
                    a.Flow = flow;
                }
            }
        }

        private void UpdateNodeBalance(Node node, int balance)
        {
            foreach (Node n in Nodes)
            {
                if (n == node)
                {
                    n.Balance += balance;
                }
            }
        }

        private void LoadFlow(DijkstraTree? tree = null)
        {
            tree ??= Clone();

            var leafs = tree.Leafs;

            if (leafs.Count == 0)
            {
                return;
            }

            Node? nodeLeaf = null;

            foreach (Node leaf in leafs)
            {
                var inc = tree.GetIncomingNodeArcs(leaf).First;

                if (inc is null)
                {
                    continue;
                }

                nodeLeaf = leaf;
            }

            if (nodeLeaf is null)
            {
                return;
            }

            var incoming = tree.GetIncomingNodeArcs(nodeLeaf).First;

            UpdateArcFlow(incoming, nodeLeaf.Balance);
            tree.UpdateNodeBalance(incoming.Source, nodeLeaf.Balance);
            //UpdateNodeBalance(incoming.Source, leaf.Balance);

            //incoming.Source.Balance += nodeLeaf.Balance;

            tree.RemoveNode(nodeLeaf);

            if (tree.Nodes.Count == 0)
            {
                return;
            }

            LoadFlow(tree);
        }

    }
}
