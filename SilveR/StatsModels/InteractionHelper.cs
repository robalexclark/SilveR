using Combinatorics.Collections;
using System;
using System.Collections.Generic;

namespace SilveR.StatsModels
{
    internal static class InteractionHelper
    {
        internal static List<string> DetermineInteractions(IList<string> factors)
        {
            List<string> interactions = new List<string>();

            for (int i = 2; i <= factors.Count; i++)
            {
                Combinations<string> combinations = new Combinations<string>(factors, i, GenerateOption.WithoutRepetition);
                foreach (IList<string> combination in combinations)
                {
                    interactions.Add(String.Join(" * ", combination));
                }
            }

            return interactions;
        }
    }
}
