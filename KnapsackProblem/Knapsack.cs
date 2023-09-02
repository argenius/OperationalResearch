using System.Collections.ObjectModel;
using System.Data;


namespace KnapsackProblem
{
    public class Knapsack: Collection<KnapsackItem>
    {
        public int Capacity { get; set; }

        public Knapsack(int capacity) : base()
        {
            Capacity = capacity;
        }

        public Knapsack(int capacity, IEnumerable<KnapsackItem> items) : base()
        {
            Capacity = capacity;

            foreach (var item in items)
            {
                Add(item);
            }
        }

        public KnapsackItem MaxValueItem
        {
            get
            {
                return OrderByValue().First();
            }
        }

        public KnapsackItem MinValueItem
        {
            get
            {
                return OrderByValue().Last();
            }
        }

        public KnapsackItem MaxWeightItem
        {
            get
            {
                return OrderByWeight().First();
            }
        }

        public KnapsackItem MinWeightItem
        {
            get
            { 
                return OrderByWeight().Last(); 
            }
        }

        public KnapsackItem MaxYealdItem
        {
            get
            {
                return OrderByYeald().First();
            }
        }

        public KnapsackItem MinYealdItem
        {
            get
            {
                return OrderByYeald().Last();
            }
        }

        public int TotalValue
        {
            get
            {
                return this.Sum(item => item.Value);
            }
        }

        public int TotalWeight
        {
            get
            {
                return this.Sum(item => item.Weight);
            }
        }

        public float TotalYeald
        {
            get
            {
                return (float)TotalValue / (float)TotalWeight;
            }
        }

        public float UpperIntegerEvaluation
        {
            get
            {
                var item = MaxYealdItem;
                var quantity = Capacity / item.Weight;
                return item.Value * quantity;
            }
        }

        public float UpperBinaryEvaluation
        {
            get
            {
                var items = OrderByYeald();
                var remainingCapacity = Capacity;
                var evaluation = 0f;

                foreach (var item in items)
                {
                    if (remainingCapacity <= 0)
                    {
                        break;
                    }

                    var itemWeight = item.Weight;
                    var itemValue = item.Value;

                    if (remainingCapacity < itemWeight)
                    {
                        var quantity = remainingCapacity / itemWeight;
                        evaluation += quantity * itemValue;
                        remainingCapacity = 0;
                        break;
                    }

                    evaluation += itemValue;
                    remainingCapacity -= itemWeight;
                }

                return evaluation;
            }
        }

        public DataTable UpperIntegerSolution
        {
            get
            {
                var dt = VectorTemplateByIndex;
                dt.TableName = "SOLUZIONE INTERA DIVISIBILE [Xi >= 0]";
                var row = dt.NewRow();
                var item = MaxYealdItem;
                var quantity = Capacity / item.Weight;
                foreach (var i in this)
                {
                    if (i.Index == item.Index)
                    {
                        row[i.Index.ToString()] = quantity;
                    }
                    else
                    {
                        row[i.Index.ToString()] = 0;
                    }
                }
                dt.Rows.Add(row);
                return dt;
            }
        }

        public DataTable UpperBinarySolution
        {
            get 
            {
                var items = OrderByIndex();
                var remainingCapacity = Capacity;
                var dt = VectorTemplateByIndex;
                dt.TableName = "SOLUZIONE BINARIA DIVISIBILE [0 <= Xi <= 1]";
                var row = dt.NewRow();

                foreach (var i in items)
                {
                    if (remainingCapacity == 0)
                    {
                        row[i.Index.ToString()] = 0;
                        continue;
                    }

                    if (remainingCapacity < i.Weight)
                    {
                        var q = remainingCapacity / i.Weight;
                        row[i.Index.ToString()] = q;
                        remainingCapacity = 0;
                        continue;
                    }

                    row[i.Index.ToString()] = 1;
                    remainingCapacity -= i.Weight;
                }

                dt.Rows.Add(row);
                return dt;
            }
        }

        public float LowerEvaluation
        {
            get
            {
                var items = OrderByYeald();
                var remainingCapacity = Capacity;
                var evaluation = 0f;

                foreach (var item in items)
                {
                    if (remainingCapacity <= 0)
                    {
                        break;
                    }

                    var itemWeight = item.Weight;
                    var itemValue = item.Value;

                    if (remainingCapacity < itemWeight)
                    {
                        // non lo metto
                        continue;
                    }

                    evaluation += itemValue;
                    remainingCapacity -= itemWeight;
                }

                return evaluation;
            }
        }

        public DataTable LowerSolution
        {
            get
            {
                var items = OrderByYeald();
                var remainingCapacity = Capacity;
                var dt = VectorTemplateByIndex;
                dt.TableName = "SOLUZIONE BINARIA INDIVISIBILE [Xi = 0 || Xi = 1]";
                var row = dt.NewRow();
                foreach (var item in items)
                {
                    if (remainingCapacity == 0 || remainingCapacity < item.Weight)
                    {
                        row[item.Index.ToString()] = 0;
                        continue;
                    }

                    row[item.Index.ToString()] = 1;
                    remainingCapacity -= item.Weight;
                }

                dt.Rows.Add(row);
                return dt;
            }
        }

        public DataTable OptimalRelaxedSolution
        {
            get
            {
                var dt = VectorTemplateByIndex;
                var row = dt.NewRow();
                var item = MaxYealdItem;
                var component = item.Yeald;
                var i = 1;

                foreach (var o in OrderByYeald())
                {
                    if (i == 1)
                    {
                        row[o.Index.ToString()] = component;
                    }
                    else
                    {
                        row[o.Index.ToString()] = 0;
                    }

                    i++;
                }

                dt.Rows.Add(row);
                return dt;
            }
        }

        public List<int> BaseOptimalRelaxedSolution
        {
            get
            {
                var optimal = OptimalRelaxedSolution;
                var y1 = (float)optimal.Rows[0][0];
                var baseList = new List<int>();
                var i = 1;

                foreach(var item in OrderByYeald())
                {
                    var value = item.Weight * y1 - item.Value;
                    var check = value == 0;

                    if (check)
                    {
                        baseList.Add(i);
                    }

                    i++;
                }

                return baseList;
            }
        }

        #region Utility

        private DataTable VectorTemplateByIndex
        {
            get
            {
                var table = new DataTable();

                foreach (var item in OrderByIndex())
                {
                    table.Columns.Add(item.Index.ToString(), typeof(object));
                }

                return table;
            }
        }

        private DataTable VectorTemplateByYeald
        {
            get
            {
                var table = new DataTable();

                foreach (var item in OrderByYeald())
                {
                    table.Columns.Add(item.Index.ToString(), typeof(object));
                }

                return table;
            }
        }

        public DataTable YealdVectorByYeald
        {
            get
            {
                var dt = VectorTemplateByYeald;
                var row = dt.NewRow();
                var yealds = OrderByYeald();
                foreach (var item in yealds) { row[item.Index.ToString()] = item.Yeald; }
                dt.Rows.Add(row);
                return dt;
            }
        }

        public DataTable YealdVectorByIndex
        {
            get
            {
                var dt = VectorTemplateByIndex;
                var row = dt.NewRow();
                var yealds = OrderByIndex();
                foreach (var item in yealds) { row[item.Index.ToString()] = item.Yeald; }
                dt.Rows.Add(row);
                return dt;
            }
        }

        public Knapsack OrderByIndex()
        {
            return new Knapsack(Capacity, this.OrderBy(item => item.Index).ToList());
        }

        public Knapsack OrderByValue()
        {
            return new Knapsack(Capacity, this.OrderBy(item => item.Value).ToList());
        }

        public Knapsack OrderByWeight()
        {
            return new Knapsack(Capacity, this.OrderBy(item => item.Weight).ToList());
        }

        public Knapsack OrderByYeald()
        {
            return new Knapsack(Capacity, this.OrderBy(item => item.Yeald).ToList());
        }
        
        public new void Add(KnapsackItem item)
        {
            if (item is null || Contains(item))
            {
                return;
            }

            base.Add(item);
        }

        public void Add(int index, int value, int weight)
        {
            Add(new KnapsackItem(index, value, weight));
        }

        public void Add(int value, int weight)
        {
            Add(new KnapsackItem(Count + 1, value, weight));
        }

        public static Knapsack Create(int capacity, List<int>? values = null, List<int>? weights = null)
        {
            if (values is null)
            {
                values = new List<int>();
            }

            if (weights is null)
            {
                weights = new List<int>();
            }

            if (values.Count != weights.Count)
            {
                throw new ArgumentException("Values and weights must have the same number of elements.");
            }

            var knapsack = new Knapsack(capacity);

            for (int i = 0; i < values.Count; i++)
            {
                knapsack.Add(i+1, values[i], weights[i]);
            }

            return knapsack;
        }

#endregion
    }
}
