using System.Data;

namespace KnapsackProblem
{
    public class KnapsackProblem
    {
        public Knapsack Knapsack { get; set; }

        public KnapsackProblem(int capacity) 
        {
            Knapsack = new Knapsack(capacity);
        }

        public KnapsackProblem(Knapsack knapsack)
        {
            Knapsack = knapsack;
        }

        public KnapsackProblem(int capacity, IEnumerable<KnapsackItem> items)
        {
            Knapsack = new Knapsack(capacity, items);
        }

        public KnapsackProblem(int capacity, List<int> values, List<int> weights)
        {
            Knapsack = Knapsack.Create(capacity, values, weights);
        }

        public void Resolve()
        {
            var vsi = Knapsack.UpperIntegerEvaluation;
            var vsb = Knapsack.UpperBinaryEvaluation;

            var xsi = Knapsack.UpperIntegerSolution;
            var xsb = Knapsack.UpperBinarySolution;

            var xi = Knapsack.LowerSolution;
            var vi = Knapsack.LowerEvaluation;

            xsi.Print();
            xsb.Print();
            xi.Print();
        }
    }
}
