using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GraphProblem
{
    public class Graph
    {
        // Graph is a collection of nodes and edges
        private static int counter = 0;
        private int _EdmondsKarpValue;
        private int _EdmondsKarpStepCounter;
        private string Label;
        private NodeCollection _Nodes;
        private ArcCollection _Arcs;

        public ArcCollection Arcs
        {
            get
            {
                return _Arcs.OrderByIndex();
            }
        }

        public NodeCollection Nodes
        {
            get
            {
                return _Nodes.OrderByIndex();
            }
        }

        private DataTable VectorTemplate
        {
            get
            {
                DataTable dt = new();

                // devo aggiungere header per ogni indice di arco

                var indexes = ArcIndexesList;

                foreach (var index in indexes)
                {
                    dt.Columns.Add(index.ToString(), typeof(int));
                }

                return dt;
            }
        }

        public DataTable Flow
        {
            get
            {
                var dt = VectorTemplate;
                dt.TableName = "X";
                var row = dt.NewRow();

                foreach (Arc arc in Arcs)
                {
                    row[arc.Indexes.ToString()] = arc.Flow;
                }

                dt.Rows.Add(row);

                return dt;
            }
        }

        public DataTable Capacity
        {
            get
            {
                var dt = VectorTemplate;
                dt.TableName = "W";
                var row = dt.NewRow();

                foreach (Arc arc in Arcs)
                {
                    row[arc.Indexes.ToString()] = arc.Capacity;
                }

                dt.Rows.Add(row);

                return dt;
            }
        }

        public DataTable ResidualCapacity
        {
            get
            {
                var dt = VectorTemplate;
                dt.TableName = "R";
                var row = dt.NewRow();

                foreach (Arc arc in Arcs)
                {
                    row[arc.Indexes.ToString()] = arc.ResidualCapacity;
                }

                dt.Rows.Add(row);

                return dt;
            }
        }

        public int Balance
        {
            get
            {
                return _Nodes.BalancesSum;
            }
        }

        public bool Balanced
        {
            get
            {
                return Balance == 0;
            }
        }

        public bool HasSaturatedArcs
        {
            get
            {
                return _Arcs.HasSaturatedArcs;
            }
        }

        public ArcCollection SaturatedArcs
        {
            get
            {
                return _Arcs.SaturatedArcs;
            }
        }

        public bool HasEmptyArcs
        {
            get
            {
                return _Arcs.HasEmptyArcs;
            }
        }

        public List<int> ArcIndexesList
        {
            get
            {
                return _Arcs.IndexesList;
            }
        }

        public NodeCollection Leafs
        {
            get
            {
                var result = new NodeCollection();
                var nodes = _Nodes;
                
                foreach (Node node in nodes)
                {
                    if (IsLeaf(node))
                    {
                        result.Add(node);
                    }
                }

                return result;
            }
        }

        public bool HasLeafs
        {
            get
            {
                return Leafs.Count > 0;
            }
        }

        public bool Empty
        {
            get
            {
                return _Nodes.Count == 0;
            }
        }

        public bool Cyclic
        {
            get
            {
                var visited = new List<int>();
                var nodes = _Nodes;

                foreach (Node node in nodes)
                {
                    if (!visited.Contains(node.Index))
                    {
                        if (DFS(node, null, visited))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public Cycle Cycle
        {
            get
            {
                var visited = new List<int>();
                var nodes = _Nodes;

                foreach (Node node in nodes)
                {
                    if (!visited.Contains(node.Index))
                    {
                        if (DFSArcs(node, null, visited, out List<Arc> cycleEdges))
                        {
                            return new Cycle(cycleEdges);
                        }
                    }
                }

                return new Cycle();
            }
        }

        public Graph(string? label = null)
        {
            Graph.counter++;

            _Nodes = new NodeCollection();
            _Arcs = new ArcCollection();
            Label = label ?? counter.ToString();
            _EdmondsKarpStepCounter = 0;
            _EdmondsKarpValue = 0;
        }

        public Graph(NodeCollection nodes, ArcCollection arcs, bool clone = false, string? label = null)
        {
            Graph.counter++;

            if (clone)
            {
                _Nodes = nodes.Clone();
                _Arcs = arcs.Clone();
            }
            else
            {
                _Nodes = nodes;
                _Arcs = arcs;
            }

            Label = label ?? counter.ToString();
            _EdmondsKarpStepCounter = 0;
            _EdmondsKarpValue = 0;
        }

        public Graph(Node node, Arc? arc = null, bool clone = false, string? label = null)
        {
            Graph.counter++;

            if (clone)
            {
                _Nodes = new NodeCollection{ node.Clone() };

                if (arc is null)
                {
                    _Arcs = new ArcCollection();
                }

                else
                {
                    _Arcs = new ArcCollection { arc.Clone() };
                }

            }

            else
            {
                _Nodes = new NodeCollection { node };
                
                if (arc is null)
                {
                    _Arcs = new ArcCollection();
                }

                else
                {
                    _Arcs = new ArcCollection { arc };
                }

            }

            Label = label ?? counter.ToString();
            _EdmondsKarpStepCounter = 0;
            _EdmondsKarpValue = 0;
        }

        #region Utility

        private bool DFS(Node node, Node? parent, List<int> visited)
        {
            visited.Add(node.Index);

            foreach (Arc arc in Arcs)
            {
                if (arc.Source.Index == node.Index || arc.Destination.Index == node.Index)
                {
                    var nextNode = arc.Source.Index == node.Index ? arc.Destination : arc.Source;

                    if (!visited.Contains(nextNode.Index))
                    {
                        if (DFS(nextNode, node, visited))
                        {
                            return true;
                        }
                    }

                    else if (parent is null || nextNode.Index != parent.Index)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool DFSArcs(Node node, Node? parent, List<int> visited, out List<Arc> cycleEdges)
        {
            visited.Add(node.Index);

            cycleEdges = new List<Arc>();

            foreach (Arc arc in Arcs)
            {
                if (arc.Source.Index == node.Index || arc.Destination.Index == node.Index)
                {
                    var nextNode = arc.Source.Index == node.Index ? arc.Destination : arc.Source;

                    if (!visited.Contains(nextNode.Index))
                    {
                        if (DFSArcs(nextNode, node, visited, out cycleEdges))
                        {

                            cycleEdges.Add(arc);
                            return true;
                        }
                    }

                    else if (parent is null || nextNode.Index != parent.Index)
                    {

                        cycleEdges.Add(arc);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsLeaf(Node node)
        {
            var inArcs = GetIncomingNodeArcs(node).Count;
            var outArcs = GetOutgoingNodeArcs(node).Count;

            return (inArcs == 1 && outArcs == 0) || (inArcs == 0 && outArcs == 1);
        }

        private string ArcsTemplate
        {
            get
            {
                var template = "{";

                var i = 0;

                foreach (Arc arc in Arcs)
                {
                    template += arc.ToString();

                    if (i < _Arcs.Count - 1)
                    {
                        template += ", ";
                    }

                    i++;
                }

                return template + "}";
            }
        }

        public override string ToString()
        {
            return $"<Graph {Label}: {ArcsTemplate}>";
        }

        public void AddNode(Node node)
        {
            _Nodes.Add(node);
        }

        public void RemoveNode(Node node, bool removeArcs = true)
        {
            _Nodes.Remove(node);

            if (removeArcs)
            {
                  _Arcs.RemoveNodeArcs(node);
            }
        }

        public void RemoveNode(int nodeIndex)
        {
            _Nodes.Remove(nodeIndex);
        }

        public Node? GetNode(int nodeIndex)
        {
            return Nodes.GetNodeByINdex(nodeIndex);
        }

        public ArcCollection GetNodeArcs(Node node)
        {
            return _Arcs.GetNodeArcs(node);
        }

        public Arc GetLeafArc(Node node)
        {
            if (!IsLeaf(node))
            {
                throw new Exception("Node is not a leaf");
            }

            var arcs = GetNodeArcs(node);

            return arcs[0];
        }

        public void UpdateNodeBalance(int index, int balance)
        {
            _Nodes.UpdateNodeBalance(index, balance);
        }

        public Node? PostponedVisitLeaf
        {
            get
            {
                var leafs = Leafs.OrderBy(leaf => -leaf.Index).ToList();

                if (_Nodes.Count != 2)
                {
                    return leafs[0];
                }

                var node1 = _Nodes[0];
                var node2 = _Nodes[1];

                if (GetIncomingNodeArcs(node1).Count > 0)
                {
                    return node1;
                }

                if (GetIncomingNodeArcs(node2).Count > 0)
                {
                    return node2;
                }

                return null;
            }
        }

        public ArcCollection GetIncomingNodeArcs(Node node)
        {
            return _Arcs.GetIncomingNodeArcs(node);
        }

        public ArcCollection GetOutgoingNodeArcs(Node node)
        {
            return _Arcs.GetOutgoingNodeArcs(node);
        }

        public NodeCollection GetIncomingNodeNodes(Node node)
        {
            var arcs = GetIncomingNodeArcs(node);
            var result = new NodeCollection();

            foreach(Arc arc in arcs)
            {
                result.Add(arc.Source);
            }

            return result;
        }

        public NodeCollection GetOutgoingNodeNodes(Node node)
        {
            var arcs = GetOutgoingNodeArcs(node);
            var result = new NodeCollection();

            foreach (Arc arc in arcs)
            {
                result.Add(arc.Destination);
            }

            return result.OrderByIndex();
        }

        public bool ContainsNode(Node node)
        {
            return _Nodes.Contains(node);
        }

        public bool ContainsArc(Arc arc)
        {
            return Arcs.Contains(arc);
        }

        public bool ContainsArc(int indexes)
        {
            return Arcs.Contains(indexes);
        }

        public void AddArc(Arc arc)
        {
            _Arcs.Add(arc);
        }

        public void RemoveArc(Arc arc)
        {
            _Arcs.Remove(arc);
        }

        public void RemoveArc(int indexes)
        {
            _Arcs.Remove(indexes);
        }

        public void UpdateBalances()
        {
            var arcs = Arcs;

            foreach(Arc arc in arcs)
            {
                if (!arc.Saturated)
                {
                    continue;
                }
                var oldValue = arc.Source.Balance;
                arc.Source.Balance += arc.Flow;
                var newValue = arc.Source.Balance;

                Console.WriteLine($"b{arc.Source.Index} = {oldValue} + {arc.Flow} = {newValue}");

                oldValue = arc.Destination.Balance;
                arc.Destination.Balance -= arc.Flow;
                newValue = arc.Destination.Balance;

                Console.WriteLine($"b{arc.Destination.Index} = {oldValue} - {arc.Flow} = {newValue}");

            }
        }

        public DataTable BuildInitialDijkstraPotential(int rootIndex)
        {
            var i = 0;
            var nodes = _Nodes.Count;

            var potential = new DataTable();

            while (i < nodes)
            {
                potential.Columns.Add($"{i + 1}", typeof(int));
                i++;
            }

            var row = potential.NewRow();

            i = 0;

            while (i < nodes)
            {
                row[(i+1).ToString()] = i + 1 == rootIndex ? 0 : int.MaxValue;
                i++;
            }

            potential.Rows.Add(row);

            return potential;
        }

        public DataTable BuildInitialDijkstraPositions(int rootIndex)
        {
            var i = 0;
            var nodes = _Nodes.Count;

            var potential = new DataTable();

            while (i < nodes)
            {
                potential.Columns.Add($"{i + 1}", typeof(int));
                i++;
            }

            var row = potential.NewRow();

            i = 0;

            while (i < nodes)
            {
                row[(i + 1).ToString()] = i + 1 == rootIndex ? rootIndex : -1;
                i++;
            }

            potential.Rows.Add(row);

            return potential;
        }
        
        private AugmentingPath FindAugmentingPath(int rootIndex, Node startNode, Node endNode)
        {
            Console.WriteLine($"\n[PASSO {++_EdmondsKarpStepCounter}]");
            var queue = new Queue<Node>();
            var visited = new Dictionary<Node, bool>();
            var predecessors = new Dictionary<Node, AugmentingPath?>();
            foreach (var node in Nodes)
            {
                visited[node] = false;
                predecessors[node] = null;
            }

            queue.Enqueue(startNode);
            visited[startNode] = true;

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();

                var extractedNodeTemplate = $"Nodo estratto: {currentNode}";

                var outgoingArcs = GetOutgoingNodeArcs(currentNode).OrderByIndex();
                var outgoingNodes = GetOutgoingNodeNodes(currentNode).OrderByIndex();
                var outingStarTemplate = $"Stella uscente: {outgoingNodes}";

                Console.WriteLine($"\n{extractedNodeTemplate}\n{outingStarTemplate}");

                foreach (var arc in outgoingArcs)
                {
                    Console.WriteLine($"r{arc.Indexes} = {arc.ResidualCapacity}");

                    if (!visited[arc.Destination] && arc.ResidualCapacity > 0)
                    {
                        queue.Enqueue(arc.Destination);
                        var curGraph = new AugmentingPath(currentNode, arc);
                        predecessors[arc.Destination] = curGraph;
                        visited[arc.Destination] = true;

                        if (arc.Destination == endNode && arc.ResidualCapacity > 0)
                        {
                            var augumentingPath = new AugmentingPath();
                            var endGraph = new AugmentingPath(endNode);

                            while (endGraph is not null)
                            {
                                var node = endGraph.Nodes.First;
                                var arcNode = endGraph.Arcs.First;
                                augumentingPath.AddNode(node);

                                if (arcNode is not null)
                                {
                                    augumentingPath.AddArc(arcNode);
                                }

                                endGraph = predecessors[node];
                            }
                            Console.WriteLine($"\n[!] CAMMINO AUMENTANTE TROVATO: {augumentingPath}");
                            var delta = 0;
                            var deltas = new List<int>();
                            var deltasTemplate = "min([";

                            var j = 0;
                            foreach (var augArc in augumentingPath.Arcs)
                            {
                                var idx = augArc.Indexes.ToString();
                                deltas.Add(augArc.ResidualCapacity);
                                deltasTemplate += $"{augArc.ResidualCapacity}";

                                if (++j < augumentingPath.Arcs.Count)
                                {
                                    deltasTemplate += ", ";
                                }
                            }

                            deltasTemplate += "])";
                            delta = deltas.Min();
                            _EdmondsKarpValue += delta;
                            Console.WriteLine($"\nDelta = {deltasTemplate} = {delta}");
                            Console.WriteLine($"V = {_EdmondsKarpValue}\n");
                            Console.WriteLine("\n[AGGIORNAMENTO FLUSSO]\n");
                            foreach (var augArc in augumentingPath.Arcs)
                            {
                                var idx = augArc.Indexes.ToString();
                                var curCap = augArc.ResidualCapacity;
                                var curFlow = augArc.Flow;
                                var newFlow = curFlow + delta;
                                augArc.Flow = newFlow;

                                Console.WriteLine($"X{idx} = {curFlow} + {delta} = {newFlow}");
                                Console.WriteLine($"W{idx} = {curCap} - {delta} = {augArc.ResidualCapacity}\n");
                            }

                            Flow.Print();
                            Console.WriteLine("");
                            ResidualCapacity.Print();
                            //augumentingPath.OrderByIndex();
                            augumentingPath.SetRootIndex(rootIndex);
                            return augumentingPath;
                        }

                    }
                }
            
            }

            var cut = new NodeCollection(visited.Keys.Where(key => visited[key]).ToArray());
            var nt = new NodeCollection(visited.Keys.Where(key => !visited[key]).ToArray());
            var cutCapacity = 0;

            foreach (Arc arc in Arcs)
            {
                if (cut.Contains(arc.Source) && nt.Contains(arc.Destination))
                {
                    cutCapacity += arc.Capacity;
                }
            }

            Console.WriteLine($"\n[TAGLIO DI CAPACITA {startNode.Index}-{endNode.Index}]\n");

            Console.WriteLine($"Ns = {cut}");
            Console.WriteLine($"Nt = {nt}\n");
            Console.WriteLine($"Us = {cutCapacity}");
            Console.WriteLine($"V = {_EdmondsKarpValue}");

            if (cutCapacity == _EdmondsKarpValue)
            {
                Console.WriteLine("X Ottimo\n");
                return new AugmentingPath();
            }

            Console.WriteLine("X non ottimo, bisogna procedere per archi fittizzi!\n");

            return new AugmentingPath();
        }

        public void FordFulkerson(Node root, Node source, Node dest)
        {
            Console.WriteLine($"\n[ALGORITMO DI FORD FULKERSON - FLUSSO MASSIMO DA {source.Index} A {dest.Index}]");
            Console.WriteLine("\n[INIZIALIZZAZIONE VETTORE FLUSSO E CAPACITA RESIDUA]\n");
            ResetFlow();
            Flow.Print();
            ResidualCapacity.Print();

            var result = FindAugmentingPath(root.Index, root, dest);

            while (result.Arcs.Count > 0)
            {
                result = FindAugmentingPath(root.Index, root, dest);
            }

            Flow.Print();
            ResidualCapacity.Print();
        }

        public void ResetFlow()
        {
            _Arcs.ResetFlow();
        }

        public Graph Merge(Graph graph)
        {
            return Merge(this, graph);
        }

        public virtual Graph Clone()
        {
            return new Graph(_Nodes.Clone(), _Arcs.Clone(), false, Label);
        }

        public static Graph Merge(Graph graph1, Graph graph2)
        {
            var nodes = new NodeCollection(graph1._Nodes.Clone());
            var arcs = new ArcCollection(graph1.Arcs.Clone());
            var nodes2 = new NodeCollection(graph2._Nodes.Clone());
            foreach (Node node in nodes2)
            {
                nodes.Add(node.Clone());
            }

            foreach (Arc arc in graph2._Arcs)
            {
                arcs.Add(arc.Clone());
            }

            return new Graph(nodes, arcs);
        }

        public static Graph operator + (Graph graph1, Graph graph2)
        {
              return Merge(graph1, graph2);
        }

        #endregion

    }
}
