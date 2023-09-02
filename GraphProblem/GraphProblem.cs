using System.Data;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;

namespace GraphProblem
{
    public class GraphProblem
    {
        private Graph T; // Cover tree
        private Graph L; // Empty arcs
        private Graph U; // Saturated arcs

        private int RootIndex;
        private bool BalancesUpdated;

        private DataTable InitialFlux;
        private DataTable InitialCapacity;
        private DataTable? Potential;
        private ArcCollection ViolatingArcs;
        private Node MaxFlowSource;
        private Node MaxFlowDestination;

        private Graph TLU
        {
            get
            {
                return T + LU;
            }
        }

        private Graph LU
        {
            get
            {
                return L + U;
            }
        }

        private Graph TL
        {
            get
            {
                return T + L;
            }
        }

        public GraphProblem(Graph t, Graph l, Graph u, int rootIndex, Node maxFlowSource, Node maxFlowDestination)
        {
            T = t;
            L = l;
            U = u;
            RootIndex = rootIndex;
            BalancesUpdated = false;

            InitialFlux = PartialFlux;
            InitialCapacity = PartialCapacity;

            MaxFlowSource = maxFlowSource;
            MaxFlowDestination = maxFlowDestination;

            ViolatingArcs = new ArcCollection();
            
        }

        private Node? RootNode
        {
            get
            {
                return TLU.GetNode(RootIndex);
            }
        }

        private DataTable VectorTLUTemplate
        {
            get
            {
                DataTable dt = new();

                // devo aggiungere header per ogni indice di arco

                var indexes = TLU.ArcIndexesList;

                foreach (var index in indexes)
                {
                    dt.Columns.Add(index.ToString(), typeof(int));
                }

                return dt;
            }
        }

        private DataTable VectorTLTemplate
        {
            get
            {
                DataTable dt = new();

                // devo aggiungere header per ogni indice di arco

                var indexes = TL.ArcIndexesList;

                foreach (var index in indexes)
                {
                    dt.Columns.Add(index.ToString(), typeof(int));
                }

                return dt;
            }
        }

        private DataTable PartialFlux
        {
            get
            {
                var dt = VectorTLUTemplate;
                dt.TableName = "X";
                var LArcs = L.Arcs;
                var UArcs = U.Arcs;
                var row = dt.NewRow();

                foreach (Arc arc in LArcs)
                {
                    var dtIdx = dt.Columns.IndexOf(arc.Indexes.ToString());
                    row[dtIdx] = 0;
                }

                foreach (Arc arc in UArcs)
                {
                    var dtIdx = dt.Columns.IndexOf(arc.Indexes.ToString());
                    row[dtIdx] = arc.Capacity;
                }

                dt.Rows.Add(row);

                return dt;
            }
        }

        private DataTable PartialCapacity
        {
            get
            {
                var dt = VectorTLUTemplate;
                dt.TableName = "W";

                var LArcs = L.Arcs;
                var UArcs = U.Arcs;

                var row = dt.NewRow();

                foreach (Arc arc in LArcs)
                {
                    var dtIdx = dt.Columns.IndexOf(arc.Indexes.ToString());
                    row[dtIdx] = arc.Capacity;
                }

                foreach (Arc arc in UArcs)
                {
                    var dtIdx = dt.Columns.IndexOf(arc.Indexes.ToString());
                    row[dtIdx] = 0;
                }

                dt.Rows.Add(row);

                return dt;
            }
        }

        private DataTable MaxCapacity
        {
            get
            {
                var dt = VectorTLTemplate;
                dt.TableName = "R";

                var TLArcs = TL.Arcs;

                var row = dt.NewRow();

                foreach (Arc arc in TLArcs)
                {
                    var dtIdx = dt.Columns.IndexOf(arc.Indexes.ToString());
                    row[dtIdx] = arc.Capacity;
                }

                dt.Rows.Add(row);

                return dt;
            }
        }

        private DataTable MaxFlow
        {
            get
            {
                var dt = VectorTLTemplate;
                dt.TableName = "X";

                var TLArcs = TL.Arcs;

                var row = dt.NewRow();

                foreach (Arc arc in TLArcs)
                {
                    var dtIdx = dt.Columns.IndexOf(arc.Indexes.ToString());
                    row[dtIdx] = 0;
                }

                dt.Rows.Add(row);

                return dt;
            }
        }

        private DataTable EdmondKarspTable
        {
            get
            {
                var dt = new DataTable();

                dt.Columns.Add("Nodi visitati", typeof(string));
                dt.Columns.Add("Nodi da visitare", typeof(string));
                return dt;
            }
        }

        private void DijkstraRecursive(NodeCollection U, DataTable potential, DataTable positions)
        {
            if (U.Count == 0)
            {
                return;
            }

            var u = U.ExtractMinimalPotential(potential);

            if (u is null)
            {
                return;
            }

            var uIndex = u.Index - 1;
            var uPotential = (int)potential.Rows[0][uIndex];
            var outgoingArcs = TLU.GetOutgoingNodeArcs(u);
            NodeCollection destNodes = new();
            var tmp = "";
            
            foreach(Arc arc in outgoingArcs)
            {
                destNodes.Add(arc.Destination);
                var destIndex = arc.Destination.Index - 1;
                var destPotential = (int)potential.Rows[0][destIndex];
                var destPotentialTemplate = (destPotential == int.MaxValue) ? "+inf" : $"{destPotential}";
                tmp += $"\nπ{destIndex} > π{uIndex} + C{arc.Indexes} -> ";
                tmp += $"{destPotentialTemplate} > {uPotential} + {arc.Cost}: ";

                if (destPotential > uPotential + arc.Cost)
                {
                    tmp += $"π{destIndex} = {uPotential + arc.Cost}, p{destIndex} = {u.Index}";

                    positions.Rows[0][destIndex] = u.Index;
                    potential.Rows[0][destIndex] = uPotential + arc.Cost;
                }

                else
                {
                    tmp += "non aggiorno";
                }
            }

            Console.WriteLine($"\nEstrazione {u} -> FC({u.Index}) = {destNodes}");
            Console.WriteLine(tmp + "\n");

            potential.Print();
            positions.Print();
            
            DijkstraRecursive(U, potential, positions);
        }

        private void Dijkstra()
        {
            Console.WriteLine($"\n[CAMMINI MINIMI DI RADICE {RootIndex} - DIJKSTRA]\n");

            var graph = TLU.Clone();

            var potential = graph.BuildInitialDijkstraPotential(RootIndex);
            var positions = graph.BuildInitialDijkstraPositions(RootIndex);

            potential.TableName = "Vettore Potenziale";
            positions.TableName = "Vettore Precedenze";

            potential.Print();
            positions.Print();

            var U = graph.Nodes;

            DijkstraRecursive(U, potential, positions);

            var arcs = new ArcCollection(positions);
            var nodes = arcs.Nodes;
            
            var minTree = new DijkstraTree(RootIndex, nodes, arcs);

            var minFlow = minTree.MinFlow(VectorTLUTemplate);

            Console.WriteLine($"{minTree}\n");
            minFlow.Print();
        }

        private void EarlyRootVisit()
        {
            Console.WriteLine("\n[VISITA ANTICIPATA DELLA RADICE]\n");
            var dt = new DataTable();
            dt.TableName = "π";

            
            var vectorB = new List<double>() { 0 };
            var i = 0;
            var arcs = T.Arcs;
            var firstRowA = new List<double>() { 1 };

            while (i < arcs.Count)
            {
                firstRowA.Add(0);
                i++;
            }

            var firstRowMatrix = Vector<double>.Build.DenseOfEnumerable(firstRowA);
            
            //matrixA = matrixA.InsertRow(0, firstRowMatrix);
            
            var matrixA = Matrix<double>.Build.DenseOfRowVectors(firstRowMatrix);

            Console.WriteLine("+ π1 = 0");

            i = 1;

            foreach(Arc arc in arcs)
            {
                var j = 0;
                var rowA = new List<double>();

                while (j <= arcs.Count)
                {
                    if (j + 1 == arc.Source.Index)
                    {
                        rowA.Add(-1);
                    }
                    else if (j + 1 == arc.Destination.Index)
                    {
                        rowA.Add(1);
                    }
                    else
                    {
                        rowA.Add(0);
                    }

                    j++;
                }

                var rowMatrix = Vector<double>.Build.DenseOfEnumerable(rowA);

                matrixA = matrixA.InsertRow(i, rowMatrix);
                vectorB.Add(arc.Cost);

                Console.WriteLine($"- π{arc.Source.Index} + π{arc.Destination.Index} = {arc.Cost}");
                i++;
            }
            
            var b = Vector<double>.Build.DenseOfEnumerable(vectorB);
            
            var x = matrixA.Solve(b);
            var k = 1;

            foreach(double component in x)
            {
                dt.Columns.Add(k.ToString(), typeof(int));
                k++;
            }

            var dtRow = dt.NewRow();

            k = 1;
            foreach(double component in x)
            {
                dtRow[k.ToString()] = component;
                k++;
            }

            dt.Rows.Add(dtRow);

            Console.WriteLine("\n");

            Potential = dt;

            Potential.Print();
            
            //Console.WriteLine($"\nπ =");
        }

        private void OptimalityTest()
        {
            Console.WriteLine("\n[TEST OTTIMALITÀ POTENZIALE - COSTI RIDOTTI]\n");

            var L = this.L;
            var arcs = L.Arcs;  
            var optimus = true;

            var vArcs = new List<Arc>();
            
            Console.WriteLine("L:\n");
            
            foreach(Arc arc in arcs)
            {
                var sourcePotential = (int)Potential.Rows[0][arc.Source.Index.ToString()];
                var destinationPotential = (int)Potential.Rows[0][arc.Destination.Index.ToString()];

                var template = $"C{arc.Indexes}(π) = C{arc.Indexes} + π{arc.Source.Index} - π{arc.Destination.Index} = ";
                var value = arc.Cost + sourcePotential - destinationPotential;
                template += $"{arc.Cost} + {sourcePotential} - {destinationPotential} = {value}";

                if (value < 0)
                {
                    template += " < 0 [Condizione di Bellman violata]";
                    optimus = false;
                    vArcs.Add(arc);
                }

                Console.WriteLine(template);
            }

            var U = this.U;
            arcs = U.Arcs;
            Console.WriteLine("\nU:\n");

            foreach (Arc arc in arcs)
            {
                var sourcePotential = (int)Potential.Rows[0][arc.Source.Index.ToString()];
                var destinationPotential = (int)Potential.Rows[0][arc.Destination.Index.ToString()];

                var template = $"C{arc.Indexes}(π) = C{arc.Indexes} + π{arc.Source.Index} - π{arc.Destination.Index} = ";
                var value = arc.Cost + sourcePotential - destinationPotential;
                template += $"{arc.Cost} + {sourcePotential} - {destinationPotential} = {value}";

                if (value > 0)
                {
                    template += " > 0 [Condizione di Bellman violata]";
                    optimus = false;
                    vArcs.Add(arc);
                }

                Console.WriteLine(template);
            }

            if (optimus)
            {
                Console.WriteLine($"\nFlusso X è ottimo");
                return;
            }

            ViolatingArcs = new ArcCollection(vArcs);

            Simplex();
        }

        private void Simplex()
        {
            var firstViolatingArc = ViolatingArcs.First;

            if (firstViolatingArc is null)
            {
                return;
            }

            var T = this.T.Clone();

            Console.WriteLine("\n[SIMPLESSO - RICERCA OTTIMO]\n");

            Console.WriteLine($"Arco entrante: {firstViolatingArc}");
            
            if (T.Cyclic)
            {
                Console.WriteLine($"Premature cycle: {T.Cycle}");
                Console.WriteLine($"T è già ciclico, controlla di aver inserito correttamente i dati");
                throw new Exception("");
            }

            T.AddArc(firstViolatingArc);

            if (T.Cyclic)
            {
                var cycle = T.Cycle;
                var cycleCol = cycle;
                var cycleNodes = cycleCol.Nodes;

                Console.WriteLine($"\nTrovato ciclo: {cycle}\n");

                var cPlus = new ArcCollection();
                var cMinus = new ArcCollection();

                // fisso il verso concorde se appartiene a L
                var cValue = L.ContainsArc(firstViolatingArc);
                var c = new Cycle(cycle);

                foreach (var arcCycle in cycle)
                {
                    var concordDirection = c.HaveConcordantDirection(firstViolatingArc, arcCycle);

                    if (cValue && concordDirection)
                    {
                        cPlus.Add(arcCycle);
                    }
                    else if (!cValue && concordDirection)
                    {
                        cMinus.Add(arcCycle);
                    }
                    else if (cValue && !concordDirection)
                    {
                        cMinus.Add(arcCycle);
                    }
                    else if (!cValue && !concordDirection)
                    {
                        cPlus.Add(arcCycle);
                    }
                }

                Console.WriteLine($"C+ = {cPlus}");
                Console.WriteLine($"C- = {cMinus}");

                var thetas = new List<int>();
                var thetasTemplate = "[";

                Console.WriteLine("\n[C+]");

                var i = 1;
                foreach (var arc in cPlus)
                {
                    // devo fare capacita meno flusso
                    var idx = arc.Indexes.ToString();
                    var cap = (int)InitialCapacity.Rows[0][idx];
                    var flx = (int)InitialFlux.Rows[0][idx];
                    arc.Capacity = cap;
                    arc.Flow = flx;
                    var value = cap - flx;
                    Console.WriteLine($"u{arc.Indexes} - x{arc.Indexes} = {cap} - {flx} = {value}");
                    thetas.Add(value >= 0 ? value : int.MaxValue);
                    i++;
                }

                int thetaPlus;

                if (thetas.Count == 0)
                {
                    thetaPlus = int.MaxValue;
                }

                else
                {
                    thetaPlus = thetas.Min();
                }

                thetasTemplate += string.Join(", ", thetas);
                thetasTemplate += "]";

                Console.WriteLine($"\nTheta+ = min({thetasTemplate.Replace($"{int.MaxValue}", "inf")}) = {thetaPlus.ToString().Replace($"{int.MaxValue}", "inf")}");
                thetas.Clear();
                
                Console.WriteLine($"\n[C-]");

                i = 1;
                foreach (var arc in cMinus)
                {
                    // prendo solo il flusso
                    var idx = arc.Indexes.ToString();
                    var flx = (int)InitialFlux.Rows[0][idx];
                    arc.Flow = flx;
                    Console.WriteLine($"x{arc.Indexes} = {flx}");
                    thetas.Add(flx >= 0 ? flx : int.MaxValue);
                    i++;
                }

                int thetaMinus;

                if (thetas.Count == 0)
                {
                    thetaMinus = int.MaxValue;
                }

                else
                {
                    thetaMinus = thetas.Min();
                }

                thetasTemplate = "[";

                thetasTemplate += string.Join(", ", thetas);
                thetasTemplate += "]";

                Console.WriteLine($"\nTheta- = min({thetasTemplate.Replace($"{int.MaxValue}", "inf")}) = {thetaMinus.ToString().Replace($"{int.MaxValue}", "inf")}");

                var theta = Math.Min(thetaPlus, thetaMinus);

                Console.WriteLine($"\nTheta = min([{thetaPlus.ToString().Replace($"{int.MaxValue}", "inf")}, {thetaMinus.ToString().Replace($"{int.MaxValue}", "inf")}]) = {theta}");

                var cPlusArc = cPlus.GetFirstMatchResidualCapacity(theta);

                // In C- devo prendere il primo arco tale per cui flx è uguale a Theta
                var cMinusArc = cMinus.GetFirstMatchFlow(theta);
                var cs = new ArcCollection();
                
                if (cPlusArc is not null)
                {
                    cs.Add(cPlusArc);
                }

                if (cMinusArc is not null)
                {
                    cs.Add(cMinusArc);
                }

                var outgoingArc = cs.First;

                Console.WriteLine($"\nArco uscente: {outgoingArc}");

                /*
                             print("\n[AGGIORNAMENTO FLUSSI]\n")

            for arc in cPlus:
                idx = self._indexes.index(arc.indexes)
                flx = self._flux[idx]
                cap = self._capacity[idx]
                valueFlx = flx + theta
                valueCap = cap - theta
                print(f"x{arc.indexes} = {flx} + {theta} = {valueFlx}")
                print(f"w{arc.indexes} = {cap} - {flx} = {valueCap}")

                self._flux[idx] = valueFlx
                self._capacity[idx] = valueCap
            print("")
            for arc in cMinus:
                idx = self._indexes.index(arc.indexes)
                flx = self._flux[idx]
                cap = self._capacity[idx]
                valueFlx = flx - theta
                valueCap = cap + theta
                print(f"x{arc.indexes} = {flx} - {theta} = {valueFlx}")
                print(f"w{arc.indexes} = {cap} + {flx} = {valueCap}")

                self._flux[idx] = valueFlx
                self._capacity[idx] = valueCap

                 */

                Console.WriteLine("\n[AGGIORNAMENTO FLUSSI]\n");

                foreach (var arc in cPlus)
                {
                    var idx = arc.Indexes.ToString();
                    var flx = (int)InitialFlux.Rows[0][idx];
                    var cap = (int)InitialCapacity.Rows[0][idx];
                    var valueFlx = flx + theta;
                    var valueCap = cap - theta;
                    Console.WriteLine($"\nX{arc.Indexes} = {flx} + {theta} = {valueFlx}");
                    Console.WriteLine($"W{arc.Indexes} = {cap} - {theta} = {valueCap}");

                    InitialFlux.Rows[0][idx] = valueFlx;
                    InitialCapacity.Rows[0][idx] = valueCap;
                }

                foreach (var arc in cMinus)
                {
                    var idx = arc.Indexes.ToString();
                    var flx = (int)InitialFlux.Rows[0][idx];
                    var cap = (int)InitialCapacity.Rows[0][idx];
                    var valueFlx = flx - theta;
                    var valueCap = cap + theta;
                    Console.WriteLine($"\nX{arc.Indexes} = {flx} - {theta} = {valueFlx}");
                    Console.WriteLine($"W{arc.Indexes} = {cap} + {theta} = {valueCap}");

                    InitialFlux.Rows[0][idx] = valueFlx;
                    InitialCapacity.Rows[0][idx] = valueCap;
                }

                Console.WriteLine("\n[AGGORIONAMENTO COMPLETATO]\n");

                InitialFlux.Print();
                InitialCapacity.Print();

                if (outgoingArc is not null)
                    T.RemoveArc(outgoingArc);

                Console.WriteLine($"{T}");

            }
        }

        private void PostponedLeafVisit(Graph? T = null)
        {
            if (T is null)
            {
                Console.WriteLine("\n[VISITA POSTICIPATA PER FOGLIE]\n");
                T = this.T.Clone();
            }

            if (T.Empty)
            {
                return;
            }

            if (!T.HasLeafs)
            {                 
                return;
            }

            var leaf = T.PostponedVisitLeaf;
            
            
            if (leaf is null)
            {
                return;
            }

            var arcLeaf = T.GetLeafArc(leaf);

            if (arcLeaf is null)
            {
                return;
            }

            var tmp = "";
            int leafBalance = 0;
            int cap = 0;
            int oldW = 0;
            if (arcLeaf.IsIncoming(leaf))
            {
                arcLeaf.Source.Balance += leaf.Balance;
                T.UpdateNodeBalance(arcLeaf.Source.Index, arcLeaf.Source.Balance);
                leafBalance = leaf.Balance;
                InitialFlux.Rows[0][arcLeaf.Indexes.ToString()] = leafBalance;
                cap = arcLeaf.Capacity - leaf.Balance;
                oldW = arcLeaf.Capacity;
                InitialCapacity.Rows[0][arcLeaf.Indexes.ToString()] = cap;

                //Console.WriteLine($"Updated {arcLeaf.Source} balance: {arcLeaf.Source.Balance}");
            }
            else if (arcLeaf.IsOutgoing(leaf))
            {
                arcLeaf.Destination.Balance += leaf.Balance;
                T.UpdateNodeBalance(arcLeaf.Destination.Index, arcLeaf.Destination.Balance);
                leafBalance = -leaf.Balance;
                InitialFlux.Rows[0][arcLeaf.Indexes.ToString()] = leafBalance;
                cap = arcLeaf.Capacity + leaf.Balance;
                oldW = arcLeaf.Capacity;
                InitialCapacity.Rows[0][arcLeaf.Indexes.ToString()] = cap;

                //Console.WriteLine($"Updated {arcLeaf.Destination} balance: {arcLeaf.Destination.Balance}");
            }
            var indexes = arcLeaf.Indexes.ToString();

            Console.WriteLine($"Estrazione foglia: {leaf}\tX{indexes} = {leafBalance} \tW{indexes} = {oldW} - {leafBalance} = {cap}");

            T.RemoveNode(leaf);

            PostponedLeafVisit(T);
        }

        private bool CheckDegenereFlux(DataTable flux, DataTable capacity)
        {
            // Devo vedere se ci sono in x archi vuoti che appartengono a T
            var degenere = false;
            var fluxRow = flux.Rows[0];
            var capacityRow = capacity.Rows[0];

            foreach(DataColumn column in flux.Columns)
            {
                int indexes = int.Parse(column.ColumnName);

                if ((int)fluxRow[column.ColumnName] == 0 && T.ContainsArc(indexes))
                {
                    degenere = true;
                }
            }

            if (degenere)
            {
                return true;
            }


            foreach (DataColumn column in capacity.Columns)
            {
                int indexes = int.Parse(column.ColumnName);

                if ((int)capacityRow[column.ColumnName] == 0 && T.ContainsArc(indexes))
                {
                    degenere = true;
                }
            }

            return degenere;

        }

        private void UpdateBalances()
        {
            if (BalancesUpdated)
            {
                return;
            }

            U.UpdateBalances();
            BalancesUpdated = true;
        }

        private void FindMaxFlow()
        {
            var graph = TLU.Clone();
            graph.FordFulkerson(RootNode, MaxFlowSource, MaxFlowDestination);

        }

        public void Resolve()
        {
            Console.WriteLine("[PROBLEMA RETE CAPACITATA]\n");

            Console.WriteLine("[DATI INSERITI]\n");
        
            Console.WriteLine($"ALBERO DI COPERTURA \tT = {T}");
            Console.WriteLine($"ARCHI VUOTI \t\tL = {L}");
            Console.WriteLine($"ARCHI SATURI \t\tU = {U}");

            Console.WriteLine($"\n[VALORI PARZIALI VETTORI FLUSSO E CAPACITA RISPETTO A L E U]\n");

            var partialFlux = PartialFlux;
            var partialCapacity = PartialCapacity;

            partialFlux.Print();
            partialCapacity.Print();

            Console.WriteLine("\n[AGGIORNAMENTO BILANCI NODI RAGGIUNTI DA ARCHI SATURI]\n");
            
            UpdateBalances();

            PostponedLeafVisit();

            Console.WriteLine("\n[VALORI INIZIALI VETTORI FLUSSO E CAPACITA RISPETTO A L E U]\n");

            var flux = InitialFlux;
            var cap = InitialCapacity;

            flux.Print();
            cap.Print();

            Console.WriteLine($"Flusso degenere: {CheckDegenereFlux(flux, cap)}");

            EarlyRootVisit();
            OptimalityTest();
            Dijkstra();
            FindMaxFlow();

        }

    }
}
