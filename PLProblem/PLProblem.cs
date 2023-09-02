using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLProblem
{
    public class PLProblem
    {
        private List<float> ObjectiveFunction;
        private List<float> VectorB;
        private List<List<float>> matrixA;

        public PLProblem()
        {
            ObjectiveFunction = new List<float>();
            VectorB = new List<float>();
            matrixA = new List<List<float>>();
        }

        public PLProblem(List<float> objectiveFunction, List<float> vectorB, List<List<float>> matrixA)
        {
            ObjectiveFunction = objectiveFunction;
            VectorB = vectorB;
            this.matrixA = matrixA;
        }


    }
}
