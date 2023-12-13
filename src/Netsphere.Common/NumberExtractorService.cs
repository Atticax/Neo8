using System;
using System.Collections.Generic;

namespace Netsphere.Common
{
    public class NumberExtractorService : IService
    {
        private static readonly Random _random = new Random();

        public int ExtractIndex(params int[] probabilities)
        {
            int num1 = 0;
            List<ProbabilityStruct> probabilityStructList = new List<ProbabilityStruct>();
            for (int index = 0; index < probabilities.Length; ++index)
            {
                probabilityStructList.Add(new ProbabilityStruct()
                {
                    Index = index,
                    Probability = num1 + (probabilities[index] - 1)
                });
                num1 += probabilities[index];
            }
            int num2 = 0;
            int num3 = _random.Next(0, num1 - 1);
            for (int index = 0; index < probabilityStructList.Count; ++index)
            {
                if (num3 >= num2 && num3 <= probabilityStructList[index].Probability)
                    return probabilityStructList[index].Index;
                num2 = probabilityStructList[index].Probability;
            }
            return -1;
        }

        private struct ProbabilityStruct
        {
            public int Index;
            public int Probability;
        }
    }
}
