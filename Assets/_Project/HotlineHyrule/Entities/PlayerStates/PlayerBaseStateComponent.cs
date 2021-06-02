using HotlineHyrule.Items;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Entities.PlayerStates
{
    public class PlayerBaseStateComponent : MonoBehaviour
    {
        protected Rigidbody2D Rigidbody { get; private set; }
        protected PlayerComponent PlayerComponent { get; private set; }
        protected WeaponComponent WeaponComponent { get; private set; }
        protected LoadoutComponent LoadoutComponent { get; private set; }
        protected ItemPickupComponent ItemPickupComponent { get; private set; }
        protected HealthComponent HealthComponent { get; private set; }

        protected virtual void Awake()
        {
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