using UnityEngine;

namespace HotlineHyrule.Entities.PlayerStates
{
    public class PlayerBaseStateComponent : MonoBehaviour
    {
        protected Rigidbody2D Rigidbody { get; private set; }
        protected PlayerComponent PlayerComponent { get; private set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            PlayerComponent = GetComponent<PlayerComponent>();
        }

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void FixedUpdateState() { }
    }
}