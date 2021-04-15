using System;

namespace HotlineHyrule.Entities
{
    public class HealthEventArgs : EventArgs
    {
        /// <summary>
        /// The new value the health was set to.
        /// </summary>
        public int NewHealth { get; }
        /// <summary>
        /// How much the health value was changed by.
        /// </summary>
        public int HealthDifference { get; }

        public HealthEventArgs(int newHealth, int healthDifference) =>
            (NewHealth, HealthDifference) = (newHealth, healthDifference);
    }
}