using System.Data;

namespace GraphProblem
{
    internal class Program
    {
        static void Main1Raccolta(string[] args)
        {
            NodeCollection nodes = new();
            var node1 = new Node(1, -4);
            var node2 = new Node(2, -4);
            var node3 = new Node(3, -5);
            var node4 = new Node(4, 4);
            var node5 = new Node(5, 7);
            var node6 = new Node(6, 4);
            var node7 = new Node(7, -2);

            nodes.Add(node1);
            nodes.Add(node2);
            nodes.Add(node3);
            nodes.Add(node4);
            nodes.Add(node5);
            nodes.Add(node6);
            nodes.Add(node7);

            ArcCollection tArcs = new();
            var arc1 = new Arc(node1, node3, 10, 11);
            var arc2 = new Arc(node2, node4, 7, 8);
            var arc3 = new Arc(node3, node5, 6, 11);
            var arc4 = new Arc(node4, node6, 3, 5);
            var arc5 = new Arc(node7, node6, 8, 5);
            var arc11 = new Arc(node2, node5, 8, 8);

            tArcs.Add(arc1);
            tArcs.Add(arc2);
            tArcs.Add(arc3);
            tArcs.Add(arc4);
            tArcs.Add(arc5);
            tArcs.Add(arc11);
            ArcCollection uArcs = new();
            var arc6 = new Arc(node5, node4, 8, 4, 4);
            uArcs.Add(arc6);

            ArcCollection lArcs = new();
            var arc7 = new Arc(node1, node2, 5, 10);
            var arc8 = new Arc(node3, node2, 3, 11);
            var arc9 = new Arc(node6, node5, 10, 5);
            var arc10 = new Arc(node5, node7, 8, 4);

            lArcs.Add(arc7);
            lArcs.Add(arc8);
            lArcs.Add(arc9);
            lArcs.Add(arc10);

            var T = new Graph(nodes, tArcs, false, "T");
            var L = new Graph(nodes, lArcs, false, "L");
            var U = new Graph(nodes, uArcs, false, "U");

            var problem = new GraphProblem(T, L, U, 1, node1, node7);

            problem.Resolve();

        }

        static void Main15Lug2019(string[] args)
        {
            NodeCollection nodes = new();
            var node1 = new Node(1, -5);
            var node2 = new Node(2, -4);
            var node3 = new Node(3, -4);
            var node4 = new Node(4, 4);
            var node5 = new Node(5, 5);
            var node6 = new Node(6, 4);
            //var node7 = new Node(7, -2);

            nodes.Add(node1);
            nodes.Add(node2);
            nodes.Add(node3);
            nodes.Add(node4);
            nodes.Add(node5);
            nodes.Add(node6);
            //nodes.Add(node7);

            ArcCollection tArcs = new();
            var arc1 = new Arc(node1, node3, 7, 5);
            var arc2 = new Arc(node2, node4, 8, 10);
            var arc3 = new Arc(node2, node5, 7, 10);
            var arc4 = new Arc(node4, node6, 3, 8);
            var arc5 = new Arc(node5, node3, 4, 6);
            //var arc11 = new Arc(node2, node5, 8, 8);

            tArcs.Add(arc1);
            tArcs.Add(arc2);
            tArcs.Add(arc3);
            tArcs.Add(arc4);
            tArcs.Add(arc5);
            //tArcs.Add(arc11);
            ArcCollection uArcs = new();
            var arc6 = new Arc(node3, node2, 4, 9, 9);
            uArcs.Add(arc6);

            ArcCollection lArcs = new();
            var arc7 = new Arc(node1, node2, 9, 7);
            var arc8 = new Arc(node5, node4, 5, 4);
            var arc9 = new Arc(node6, node5, 9, 7);
            //var arc10 = new Arc(node5, node7, 8, 4);

            lArcs.Add(arc7);
            lArcs.Add(arc8);
            lArcs.Add(arc9);
            //lArcs.Add(arc10);

            var T = new Graph(nodes, tArcs, false, "T");
            var L = new Graph(nodes, lArcs, false, "L");
            var U = new Graph(nodes, uArcs, false, "U");

            var problem = new GraphProblem(T, L, U, 1, node1, node6);

            problem.Resolve();

        }


        static void Main17Feb2020(string[] args)
        {
            NodeCollection nodes = new();
            var node1 = new Node(1, -4);
            var node2 = new Node(2, -3);
            var node3 = new Node(3, -7);
            var node4 = new Node(4, 2);
            var node5 = new Node(5, 6);
            var node6 = new Node(6, 6);
            //var node7 = new Node(7, -2);

            nodes.Add(node1);
            nodes.Add(node2);
            nodes.Add(node3);
            nodes.Add(node4);
            nodes.Add(node5);
            nodes.Add(node6);
            //nodes.Add(node7);

            ArcCollection tArcs = new();
            var arc1 = new Arc(node1, node3, 9, 5);
            var arc2 = new Arc(node2, node3, 3, 11);
            var arc3 = new Arc(node2, node4, 6, 9);
            var arc4 = new Arc(node2, node6, 5, 8);
            var arc5 = new Arc(node5, node6, 5, 8);
            //var arc11 = new Arc(node2, node5, 8, 8);

            tArcs.Add(arc1);
            tArcs.Add(arc2);
            tArcs.Add(arc3);
            tArcs.Add(arc4);
            tArcs.Add(arc5);
            //tArcs.Add(arc11);
            ArcCollection uArcs = new();
            var arc6 = new Arc(node3, node5, 6, 11, 11);
            uArcs.Add(arc6);

            ArcCollection lArcs = new();
            var arc7 = new Arc(node1, node2, 9, 5);
            var arc8 = new Arc(node3, node4, 9, 5);
            var arc9 = new Arc(node3, node6, 10, 9);
            var arc10 = new Arc(node4, node6, 4, 7);

            lArcs.Add(arc7);
            lArcs.Add(arc8);
            lArcs.Add(arc9);
            lArcs.Add(arc10);
            //lArcs.Add(arc10);

            var T = new Graph(nodes, tArcs, false, "T");
            var L = new Graph(nodes, lArcs, false, "L");
            var U = new Graph(nodes, uArcs, false, "U");

            var problem = new GraphProblem(T, L, U, 1, node1, node6);

            problem.Resolve();

        }

        static void Main(string[] args)
        {
            NodeCollection nodes = new();
            var node1 = new Node(1, -5);
            var node2 = new Node(2, -3);
            var node3 = new Node(3, -6);
            var node4 = new Node(4, 4);
            var node5 = new Node(5, 4);
            var node6 = new Node(6, 6);

            nodes.Add(node1);
            nodes.Add(node2);
            nodes.Add(node3);
            nodes.Add(node4);
            nodes.Add(node5);
            nodes.Add(node6);

            ArcCollection tArcs = new();
            var arc1 = new Arc(node1, node2, 8, 12);
            var arc2 = new Arc(node2, node3, 4, 11);
            var arc3 = new Arc(node3, node4, 8, 11);
            var arc4 = new Arc(node3, node5, 9, 8);
            var arc5 = new Arc(node4, node6, 4, 12);

            tArcs.Add(arc1);
            tArcs.Add(arc2);
            tArcs.Add(arc3);
            tArcs.Add(arc4);
            tArcs.Add(arc5);

            ArcCollection uArcs = new();
            var arc6 = new Arc(node1, node3, 8, 5, 5);
            uArcs.Add(arc6);

            ArcCollection lArcs = new();
            var arc7 = new Arc(node2, node6, 4, 11);
            var arc8 = new Arc(node3, node6, 7, 8);
            var arc9 = new Arc(node5, node6, 8, 4);
            var arc10 = new Arc(node2, node4, 3, 5);

            lArcs.Add(arc7);
            lArcs.Add(arc8);
            lArcs.Add(arc9);
            lArcs.Add(arc10);

            var T = new Graph(nodes, tArcs, false, "T");
            var L = new Graph(nodes, lArcs, false, "L");
            var U = new Graph(nodes, uArcs, false, "U");

            var problem = new GraphProblem(T, L, U, 1, node1, node6);

            problem.Resolve();

        }


    }
}