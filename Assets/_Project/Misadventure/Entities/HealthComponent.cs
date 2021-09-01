using System;
using System.Collections;
using System.Runtime.CompilerServices;
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
                if (IsInvincible && value < _health) return;
                var lastHealth = _health;
                _health = Mathf.Clamp(value, 0, maxHealth);

                var healthEventArgs = new HealthEventArgs(_health, value - lastHealth);
                HealthChanged?.Invoke(this, healthEventArgs);
            }
        }

        public bool IsInvincible { get; set; }

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
        public void ResetHealth() => Health = startHealth;

        public void Consume(HealthItemData healthItem)
        {
            if (healthItem.HealRate == 0)
            {
                Health += healthItem.HealTotal;
                return;
            }

            if (healthItem.HealAmount == 0) return;
            if (Mathf.Sign(healthItem.HealTotal) - Mathf.Sign(healthItem.HealAmount) > float.Epsilon) return;
            StartCoroutine(HealRoutine(healthItem));
        }

        IEnumerator HealRoutine(HealthItemData healthItem)
        {
            var healTotal = healthItem.HealTotal;

            while (Mathf.Abs(healTotal) >= Mathf.Abs(healthItem.HealAmount))
            {
                healTotal -= healthItem.HealAmount;
                Health += healthItem.HealAmount;
                yield return new WaitForSeconds(1 / healthItem.HealRate);
            }
        }
    }
}
