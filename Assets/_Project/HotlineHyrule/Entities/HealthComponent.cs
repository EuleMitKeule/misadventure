using System;
using System.Collections;
using HotlineHyrule.Items;
using UnityEditor;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    /// <summary>
    /// Handles the health of the entity it's attached to.
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        /// <summary>
        /// The maximum amount of health the entity can have.
        /// </summary>
        [SerializeField] public int maxHealth;
        /// <summary>
        /// The amount of health the entity spawns with.
        /// </summary>
        [SerializeField] public int startHealth;
        
        int _health;
        /// <summary>
        /// The amount of health the entity currently has; clamped to [0, maxHealth].
        /// </summary>
        public int Health
        {
            get { return _health; }
            set
            {
                var lastHealth = _health;
                _health = Mathf.Clamp(value, 0, maxHealth);

                var healthEventArgs = new HealthEventArgs(_health, value - lastHealth);
                HealthChanged?.Invoke(this, healthEventArgs);
            }
        }

        /// <summary>
        /// Is invoked when the health value has changed.
        /// </summary>
        public event EventHandler<HealthEventArgs> HealthChanged;

        void Awake()
        {
            ResetHealth();
        }

        /// <summary>
        /// Resets health to the start value, clamped to [0, maxHealth].
        /// </summary>
        void ResetHealth() => Health = startHealth;

        public void Consume(HealthItemData healthItem)
        {
            if (healthItem.healRate == 0)
            {
                Health += healthItem.healTotal;
                return;
            }

            if (healthItem.healAmount == 0) return;
            if (Mathf.Sign(healthItem.healTotal) - Mathf.Sign(healthItem.healAmount) > float.Epsilon) return;
            StartCoroutine(HealRoutine(healthItem));
        }

        IEnumerator HealRoutine(HealthItemData healthItem)
        {
            var healTotal = healthItem.healTotal;

            while (Mathf.Abs(healTotal) >= Mathf.Abs(healthItem.healAmount))
            {
                healTotal -= healthItem.healAmount;
                Health += healthItem.healAmount;
                yield return new WaitForSeconds(1 / healthItem.healRate);
            }
        }
    }
}
