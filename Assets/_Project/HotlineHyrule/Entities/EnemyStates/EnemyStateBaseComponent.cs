using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public abstract class EnemyStateBaseComponent : MonoBehaviour
    {
        protected EnemyComponent enemyComponent;

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
            enemyComponent = GetComponent<EnemyComponent>();
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
    }
}