using System;

namespace ExerciceSoge
{
    public class Operation
    {
        public DateTime Date { get; internal set; }
        public decimal Amount { get; internal set; }
        public decimal AmountEur { get; internal set; }
        public string Currency { get; internal set; }
        public string Classification { get; internal set; }
    }
}
