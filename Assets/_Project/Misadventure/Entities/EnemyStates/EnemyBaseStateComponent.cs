using System;
using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Pathfinding;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public abstract class EnemyBaseStateComponent : MonoBehaviour
    {
        public event Action<Type> ChangeRequested;
        protected void SetState<TStateType>() where TStateType : EnemyBaseStateComponent =>
            ChangeRequested?.Invoke(typeof(TStateType));
        protected void SetState(Type stateType) =>
            ChangeRequested?.Invoke(stateType);

        protected Type AfterAttackState =>
            States.Find(e => e is EnemySearchStateComponent)?.GetType() ?? EnemyComponent.PassiveState;

        protected List<EnemyBaseStateComponent> States { get; private set; }
        protected Rigidbody2D Rigidbody { get; private set; }
        protected EnemyComponent EnemyComponent { get; private set; }
        protected WeaponComponent WeaponComponent { get; private set; }
        protected PathfindingComponent PathfindingComponent { get; private set; }
        protected Animator Animator { get; private set; }

        protected virtual void Awake()
        {
            States = GetComponents<EnemyBaseStateComponent>().ToList();
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