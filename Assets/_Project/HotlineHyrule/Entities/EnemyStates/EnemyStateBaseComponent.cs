using HotlineHyrule.Pathfinding;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public abstract class EnemyStateBaseComponent : MonoBehaviour
    {
        protected Rigidbody2D Rigidbody { get; private set; }
        protected EnemyComponent EnemyComponent { get; private set; }
        protected WeaponComponent WeaponComponent { get; private set; }
        protected PathfindingComponent PathfindingComponent { get; private set; }
        protected Animator Animator { get; private set; }

        protected GameObject PlayerObject;

        /// <summary>
        /// (Future) Priority value if enemy can change between multiple states
        /// (the higher the value the higher the priority)
        /// </summary>
        public int priority = 0;

        /// <summary>
        /// Things that shall be setup when the state is currently set for an enemy
        /// </summary>
        public virtual void Setup()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            EnemyComponent = GetComponent<EnemyComponent>();
            WeaponComponent = GetComponentInChildren<WeaponComponent>();
            PathfindingComponent = GetComponent<PathfindingComponent>();
            Animator = GetComponent<Animator>();
            PlayerObject = Locator.PlayerComponent.gameObject;
        }
        /// <summary>
        /// Things that shall be updated for the current state of the enemy
        /// </summary>
        public virtual void StateUpdate() { }
        /// <summary>
        /// Things that shall be (fixed) updated for the current state of the enemy
        /// </summary>
        public virtual void FixedStateUpdate() { }
        /// <summary>
        /// Things that shall be cleaned up when the enemy leaves the state
        /// </summary>
        public virtual void Exit() { }
        /// <summary>
        /// Things that shall happen when the enemy looks at the player
        /// </summary>
        public virtual void OnLookingAtPlayer() { }
    }
}