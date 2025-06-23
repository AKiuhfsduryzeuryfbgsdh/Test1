using System;
using System.Collections.Generic;

namespace ExerciceSoge
{
    public class AccountManager
    {
        protected AccountCsvReader _accountCsvReader = new();


        public void ComputeAndDisplayBalance(string csvPathname, string balanceComputeDateString)
        {
            // Read CSV first
            AccountCsvData accountCsvData = _accountCsvReader.ReadCsv(csvPathname);

            DateTime balanceComputeDate = Utilities.ParseDate(balanceComputeDateString);
            (decimal Sum, IEnumerable<string> Classifications) balanceAndStats = ComputeBalance(balanceComputeDate, accountCsvData);

            string cr = Environment.NewLine;
            Console.WriteLine($"{cr}Date: {balanceComputeDate.ToString("dd/MM/yyyy")} - Balance amount: {balanceAndStats.Sum:0.##}{cr}");

            if (balanceAndStats.Classifications != null)
            {
                int i = 1;
                foreach (string classification in balanceAndStats.Classifications)
                {
                    Console.WriteLine($"Rank {i++} - Classification: {classification}");
                }
            }

            Console.WriteLine();
        }


        private (decimal Sum, IEnumerable<string> Classifications) ComputeBalance(DateTime balanceComputeDate, AccountCsvData accountCsvData)
        {
            // I make the assumption that all operations took place BEFORE balance date.
            // So, balance won't change after its date
            // and we will subtract operations that took place *striclty* before the requested balance compute date

            decimal balance;

            if (balanceComputeDate >= accountCsvData.BalanceDate)
            {
                balance = accountCsvData.BalanceAmount;
                return (balance, null);
            }

            // Process going back in time. Take into account operations which satisfy: compute date < operation date <= balance date (because we compute end of day balance I guess)
            // Improvement: use dichotomy to go faster
            (decimal Sum, IEnumerable<string> Classifications) res = accountCsvData.ComputeOperations(balanceComputeDate, accountCsvData.BalanceDate);

            decimal sumMinusPreviousOps = accountCsvData.BalanceAmount - res.Sum;

            return (sumMinusPreviousOps, res.Classifications);
        }


    }
}
