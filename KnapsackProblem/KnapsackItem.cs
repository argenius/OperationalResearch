using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnapsackProblem
{
    public class KnapsackItem
    {
        public int Index { get; set; }

        public int Value { get; set; }
        
        public int Weight { get; set; }

        public float Yeald
        {
            get 
            {
                return (float)Value / (float)Weight;
            }
        }

        public KnapsackItem(int index, int value, int weight)
        {
            Index = index;
            Value = value;
            Weight = weight;
        }

        public KnapsackItem Clone()
        {
            return new KnapsackItem(Index, Value, Weight);
        }

        public static bool operator ==(KnapsackItem item1, KnapsackItem item2)
        {
            return item1.Index == item2.Index;
        }

        public static bool operator !=(KnapsackItem item1, KnapsackItem item2)
        {
            return item1.Index != item2.Index;
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

            return ((KnapsackItem)obj).Index == Index;
        }

        public override int GetHashCode()
        {
            return this.Index;
        }
    }
}
