namespace ISTUK.MathLibrary
{
    using System;

    public static class RandomNumber
    {
        private static Random rand = new Random();

        public static double Between(double lowerBound, double upperBound)
        {
            return ((rand.NextDouble() * (upperBound - lowerBound)) + lowerBound);
        }

        public static double Get(double nominalValue, double range)
        {
            return ((((rand.NextDouble() * range) * 2.0) + nominalValue) - range);
        }
    }
}

