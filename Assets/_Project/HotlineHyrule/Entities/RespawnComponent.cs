using System;
using HotlineHyrule.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Entities
{
    [RequireComponent(typeof(HealthComponent))]
    public class RespawnComponent : MonoBehaviour
    {
        [SerializeField] InputAction respawnAction;
        
        bool IsDead { get; set; }
        Vector2 RespawnPosition { get; set; }
        
        HealthComponent HealthComponent { get; set; }
        Rigidbody2D Rigidbody { get; set; }

        public event EventHandler<EventArgs> Respawned;
        
        void Awake()
        {
            HealthComponent = GetComponent<HealthComponent>();
            Rigidbody = GetComponent<Rigidbody2D>();

            var playerComponent = GetComponent<PlayerComponent>();
            var enemyComponent = GetComponent<EnemyComponent>();

            if (playerComponent != null) RespawnPosition = Locator.LevelComponent.playerRespawnPosition.ToWorld();
            else if (enemyComponent != null) RespawnPosition = enemyComponent.respawnPoint.ToWorld();
            
            HealthComponent.HealthChanged += OnHealthChanged;
            respawnAction.started += OnButtonRespawn;

            respawnAction.Enable();
        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (e.NewHealth != 0) return;

            IsDead = true;
        }

        void OnButtonRespawn(InputAction.CallbackContext context)
        {
            if (!IsDead) return;
            
            Rigidbody.position = RespawnPosition;
            IsDead = false;
            
            Respawned?.Invoke(this, EventArgs.Empty);
        }
    }
}