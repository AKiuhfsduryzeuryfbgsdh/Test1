using System;

namespace ExerciceSoge
{
    class Program
    {
        // TODO: extract interfaces and use dependency injection
        // TODO: Unit tests

        static int Main(string[] args)
        {
            //args = new string[] { "Account_20230228.csv", "28/02/2023" };
            //args = new string[] { "Account_20230228.csv", "27/02/2023" };
            //args = new string[] { "Account_20230228.csv", "21/02/2023" };
            //args = new string[] { "Account_20230228.csv", "20/02/2023" };
            //args = new string[] { "Account_20230228.csv", "19/02/2023" };
            args = new string[] { "Account_20230228.csv", "01/01/2022" };

            if (args.Length != 2)
            {
                string currentExecutable = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine($"Syntax: {currentExecutable} <csv pathname> <balance date (dd/mm/yyyy)>");
                return 1;
            }

            AccountManager accountManager = new();
            accountManager.ComputeAndDisplayBalance(args[0], args[1]);
            return 0;
        }
    }
}
