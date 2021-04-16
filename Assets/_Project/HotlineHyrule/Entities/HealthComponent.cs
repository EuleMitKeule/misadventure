using System;
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
                
                Debug.Log($"{value} => {_health}");

                var healthEventArgs = new HealthEventArgs(_health, value - lastHealth);
                HealthChanged?.Invoke(this, healthEventArgs);
            }
        }
        
        RespawnComponent RespawnComponent { get; set; }

        /// <summary>
        /// Is invoked when the health value has changed.
        /// </summary>
        public event EventHandler<HealthEventArgs> HealthChanged;

        void Awake()
        {
            RespawnComponent = GetComponent<RespawnComponent>();
            
            if (RespawnComponent) RespawnComponent.Respawned += OnRespawned;

            ResetHealth();
        }

        /// <summary>
        /// Resets health to the start value, clamped to [0, maxHealth].
        /// </summary>
        void ResetHealth() => Health = startHealth;

        /// <summary>
        /// Resets health to the start value when the entity has respawned.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRespawned(object sender, EventArgs e)
        {
            Debug.Log("OnRespawned");
            ResetHealth();
        }
    }
}
