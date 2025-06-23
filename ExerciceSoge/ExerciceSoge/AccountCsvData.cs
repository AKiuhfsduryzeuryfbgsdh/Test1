using System;
using System.Collections.Generic;
using System.Linq;

namespace ExerciceSoge
{
    public class AccountCsvData
    {
        public DateTime BalanceDate { get; internal set; }
        
        public decimal BalanceAmount { get; internal set; }

        public Dictionary<string, Currency> _currencies = new();

        public SortedList<DateTime, List<Operation>> _operations = new();


        public AccountCsvData()
        {
            // Add pseudo EUR/EUR conversion for consistent processing (avoids a "if" for EUR)
            AddCurrency("EUR", 1m);
        }


        internal void AddCurrency(string currencyName, decimal rate)
        {
            if (rate == 0m)
                throw new ArgumentException("Rate can't be 0");

            Currency ccy = new Currency() { Name = currencyName, Rate = rate };
            _currencies.Add(currencyName, ccy);
        }


        internal void AddOperation(DateTime date, decimal amount, string ccy, string classification)
        {
            Operation operation = new Operation
            {
                Date = date,
                Amount = amount,
                AmountEur = amount * _currencies[ccy.ToUpper()].Rate, // Might throw KeyNotFoundException
                Currency = ccy,
                Classification = classification
            };

            if (_operations.TryGetValue(operation.Date, out List<Operation> ops))
            {
                ops.Add(operation);
            }
            else
            {
                _operations.Add(operation.Date, new() { operation });
            }
        }


        /// <summary>
        /// Compute sum of operations strictly AFTER requested balance date
        /// </summary>
        /// <param name="requestedBalanceDate"></param>
        /// <param name="knownBalanceDate"></param>
        /// <returns></returns>
        internal (decimal, IEnumerable<string>) ComputeOperations(DateTime requestedBalanceDate, DateTime knownBalanceDate)
        {
            ClassificationStatistics stats = new();

            //foreach(List<Operation> ops in _operations.Where(o => o.Key <= knownBalanceDate && o.Key > requestedBalanceDate).Select(o => o.Value))
            //{
            //    foreach (Operation op in ops)
            //    {
            //        stats.Add(op);
            //    }
            //}

            foreach (KeyValuePair<DateTime, List<Operation>> ops in _operations)
            {
                if (ops.Key <= knownBalanceDate && ops.Key > requestedBalanceDate)
                {
                    foreach (Operation op in ops.Value)
                    {
                        stats.Add(op);
                    }
                }
            }

            return (stats.Sum, stats.GetClassifications());
        }

    }
}
