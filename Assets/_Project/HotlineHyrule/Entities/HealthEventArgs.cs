using System;

namespace HotlineHyrule.Entities
{
    public class HealthEventArgs : EventArgs
    {
        public int NewHealth { get; }
        public int HealthDifference { get; }

        public HealthEventArgs(int newHealth, int healthDifference) =>
            (NewHealth, HealthDifference) = (newHealth, healthDifference);
    }
}