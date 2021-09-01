using System;
using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Entities.EnemyStates;
using HotlineHyrule.Items;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Entities.PlayerStates
{
    public class PlayerBaseStateComponent : MonoBehaviour
    {
        public event Action<Type> ChangeRequested;
        protected void SetState<TStateType>() where TStateType : PlayerBaseStateComponent =>
            ChangeRequested?.Invoke(typeof(TStateType));
        protected void SetState(Type stateType) =>
            ChangeRequested?.Invoke(stateType);

        protected List<EnemyBaseStateComponent> States { get; private set; }
        protected Rigidbody2D Rigidbody { get; private set; }
        protected PlayerComponent PlayerComponent { get; private set; }
        protected WeaponComponent WeaponComponent { get; private set; }
        protected LoadoutComponent LoadoutComponent { get; private set; }
        protected ItemPickupComponent ItemPickupComponent { get; private set; }
        protected HealthComponent HealthComponent { get; private set; }

        protected virtual void Awake()
        {
            States = GetComponents<EnemyBaseStateComponent>().ToList();
            Rigidbody = GetComponent<Rigidbody2D>();
            PlayerComponent = GetComponent<PlayerComponent>();
            WeaponComponent = GetComponent<WeaponComponent>();
            LoadoutComponent = GetComponent<LoadoutComponent>();
            ItemPickupComponent = GetComponent<ItemPickupComponent>();
            HealthComponent = GetComponent<HealthComponent>();
        }

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void FixedUpdateState() { }
    }
}