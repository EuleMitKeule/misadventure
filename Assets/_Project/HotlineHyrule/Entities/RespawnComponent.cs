using System;
using HotlineHyrule.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Entities
{
    /// <summary>
    /// Handles the respawning of a dead player.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class RespawnComponent : MonoBehaviour
    {
        /// <summary>
        /// The respawn input action.
        /// </summary>
        [SerializeField] public InputAction respawnAction;
        
        /// <summary>
        /// Whether the player is currently dead.
        /// </summary>
        bool IsDead { get; set; }
        /// <summary>
        /// The position the player respawns at in world space.
        /// </summary>
        Vector2 RespawnPosition { get; set; }
        
        HealthComponent HealthComponent { get; set; }
        Rigidbody2D Rigidbody { get; set; }

        /// <summary>
        /// Is invoked when the player has respawned.
        /// </summary>
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

        /// <summary>
        /// Checks if the player died whenever it's health changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (e.NewHealth != 0) return;

            IsDead = true;
        }

        /// <summary>
        /// Respawns the player when it's dead and the respawn input was received.
        /// </summary>
        /// <param name="context"></param>
        void OnButtonRespawn(InputAction.CallbackContext context)
        {
            if (!IsDead) return;
            
            Rigidbody.position = RespawnPosition;
            IsDead = false;
            
            Respawned?.Invoke(this, EventArgs.Empty);
        }
    }
}