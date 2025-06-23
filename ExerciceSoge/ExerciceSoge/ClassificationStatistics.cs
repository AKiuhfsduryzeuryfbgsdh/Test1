using System;
using System.Collections.Generic;
using System.Linq;

namespace ExerciceSoge
{
    public class ClassificationStatistics
    {
        public decimal Sum { get; private set; } = 0m;

        private Dictionary<string, decimal> _amountsByClassification = new();


        internal void Add(Operation op)
        {
            Sum += op.AmountEur;

            if (_amountsByClassification.TryGetValue(op.Classification, out decimal currentSum))
            {
                // Found, add amount to current sum for this classification
                _amountsByClassification[op.Classification] = currentSum + op.AmountEur;
            }
            else
            {
                // Not found, init with amount
                _amountsByClassification.Add(op.Classification, op.AmountEur);
            }

        }




        /// <summary>
        /// Returns classification ordered by DESCENDING sum of amounts
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<string> GetClassifications()
        {
            SortedList<decimal, string> sorted = new();
            foreach(KeyValuePair<string, decimal> kvp in _amountsByClassification)
            {
                // Add only debits in EUR
                if (kvp.Value < 0)
                    sorted.Add(kvp.Value, kvp.Key);
            }

            // No need to revert because operations are all negative, so... the most important ones in absolute values are the first in the sorted list :)
            return sorted.Select(x => x.Value);

            //IEnumerable<string> reversed = sorted.Reverse().Select(x => x.Value);
            //return reversed;
        }
    }
}
