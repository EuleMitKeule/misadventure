using HotlineHyrule.Pathfinding;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public abstract class EnemyBaseStateComponent : MonoBehaviour
    {
        /// <summary>
        /// (Future) Priority value if enemy can change between multiple states
        /// (the higher the value the higher the priority)
        /// </summary>
        public int priority;

        protected Rigidbody2D Rigidbody { get; private set; }
        protected EnemyComponent EnemyComponent { get; private set; }
        protected WeaponComponent WeaponComponent { get; private set; }
        protected PathfindingComponent PathfindingComponent { get; private set; }
        protected Animator Animator { get; private set; }

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            EnemyComponent = GetComponent<EnemyComponent>();
            WeaponComponent = GetComponent<WeaponComponent>();
            PathfindingComponent = GetComponent<PathfindingComponent>();
            Animator = GetComponent<Animator>();
        }

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void UpdateState() { }
        public virtual void FixedUpdateState() { }
        public virtual void OnCollisionEnterState(Collision2D other) { }
        public virtual void OnHealthChanged(object sender, HealthEventArgs e) { }
    }
}