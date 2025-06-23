using System;
using System.Globalization;

namespace ExerciceSoge
{
    class Utilities
    {

        public static CultureInfo Provider = CultureInfo.InvariantCulture;

        public static DateTime ParseDate(string dateString)
        {
            DateTime dateTime = DateTime.ParseExact(dateString, "dd/MM/yyyy", Provider);
            return dateTime;
        }
    }
}
