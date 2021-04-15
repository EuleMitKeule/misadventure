using System;
using UnityEditor;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] public int maxHealth;
        [SerializeField] int startHealth;
        int _health;
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

        public event EventHandler<HealthEventArgs> HealthChanged;

        void Awake() => ResetHealth();

        public void ResetHealth() => Health = startHealth;
    }
}
