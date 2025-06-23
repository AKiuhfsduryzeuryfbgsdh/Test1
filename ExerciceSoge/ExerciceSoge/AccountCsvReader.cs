using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ExerciceSoge
{
    public class AccountCsvReader
    {
        private int compteAuLength = "Compte au ".Length;
        private int dateLength = "28/02/2023".Length; // example used to compute length


        /// <summary>
        /// 
        /// </summary>
        /// <param name="csvPathname"></param>
        /// <returns></returns>
        public AccountCsvData ReadCsv(string csvPathname)
        {
            // Read file
            string[] csvLines = File.ReadAllLines(csvPathname);

            AccountCsvData accountCsvData = new();
            int i = 0; // current line index

            // Read 1st line: balance amount and date
            ReadBalanceData(csvLines[i++], accountCsvData);

            // Read lines 2-3 (2 currencies)
            int k = 0;

            try
            {
                const int currenciesCount = 2;
                for (k = 0; k < currenciesCount; k++)
                    ReadCurrency(csvLines[i++], accountCsvData);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException($"Wrong currency rate At CSV line {k}", e);
            }

            // Skip operations header
            i++;

            // Read operations
            k = i;
            try
            {
                for (k = i; k < csvLines.Length; k++)
                    ReadOperation(csvLines[i++], accountCsvData);
            }
            catch (KeyNotFoundException e)
            {
                throw new KeyNotFoundException($"Unknown currency at CSV line {k}", e);
            }

            return accountCsvData;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="accountCsvData"></param>
        private void ReadBalanceData(string line, AccountCsvData accountCsvData)
        {
            // Pattern is "Compte au 28/02/2023 : 8300.00 EUR")
            string[] split = line.Split(':');
            if (split.Length != 2)
                throw new FormatException(split.Length.ToString());

            int i = 0;

            // Extract balance date (part is: "Compte au 28/02/2023 ")
            string part = split[i++];

            string dateString = part.Substring(compteAuLength, dateLength);
            DateTime dateTime = Utilities.ParseDate(dateString);
            accountCsvData.BalanceDate = dateTime;

            // Extract balance amount (part is: " 8300.00 EUR")
            part = split[i++];

            string[] split2 = part.TrimStart(' ').Split(' ');
            if (split2.Length != 2)
                throw new FormatException(split.Length.ToString());
            if (split2[1].ToUpper() != "EUR")
                throw new FormatException(split2[1]);

            string amountString = split2[0];
            decimal amount = decimal.Parse(amountString, Utilities.Provider);
            accountCsvData.BalanceAmount = amount;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="accountCsvData"></param>
        private void ReadCurrency(string line, AccountCsvData accountCsvData)
        {
            // Pattern is "JPY/EUR : 0.482" (note: value does not look realistic)
            string[] split = line.Split(':');
            if (split.Length != 2)
                throw new FormatException(split.Length.ToString());

            int i = 0;
            CultureInfo provider = CultureInfo.InvariantCulture;

            // Extract balance date (part is: "JPY/EUR ")
            string part = split[i++];

            string[] split2 = part.TrimEnd(' ').Split('/');
            if (split2.Length != 2)
                throw new FormatException(split.Length.ToString());
            if (split2[1].ToUpper() != "EUR")
                throw new FormatException(split2[1]);

            string currency = split2[0].Substring(0, 3).ToUpper(); // 3 chars for a currency

            // Extract rate (part is: " 0.482")
            part = split[i++];

            string rateString = part.TrimStart(' ');
            decimal rate = decimal.Parse(rateString, provider);

            accountCsvData.AddCurrency(currency, rate);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="k"></param>
        /// <param name="accountCsvData"></param>
        private void ReadOperation(string line, AccountCsvData accountCsvData)
        {
            // Pattern is: "06/10/2022;-504.61;EUR;Loisir"
            string[] split = line.Split(';');
            if (split.Length != 4)
                throw new FormatException(split.Length.ToString());

            int i = 0;
            CultureInfo provider = CultureInfo.InvariantCulture;

            // Extract operation date (part is: "06/10/2022")
            string part = split[i++];
            DateTime dateTime = Utilities.ParseDate(part);

            // Extract operation amount (part is: "-504.61")
            part = split[i++];
            decimal amount = decimal.Parse(part, provider);

            // Extract operation currency (part is: "EUR")
            part = split[i++];
            string ccy = part.ToUpper();

            // Extract classification (part is: "Loisir")
            part = split[i++];
            string classification = part;

            // Create operation
            accountCsvData.AddOperation(dateTime, amount, ccy, classification);
        }

    }
}
