using System;

namespace Misadventure.Entities
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
        /// <summary>
        /// Whether the entity took damage.
        /// </summary>
        public bool IsDamage => HealthDifference < 0;
        /// <summary>
        /// Whether the enemy was killed.
        /// </summary>
        public bool IsKilled => NewHealth <= 0;

        public HealthEventArgs(int newHealth, int healthDifference) =>
            (NewHealth, HealthDifference) = (newHealth, healthDifference);
    }
}