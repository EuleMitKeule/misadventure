using System;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates.HelperComponents
{
    public class EnemyStateFollowColliderHelperComponent : MonoBehaviour
    {
        Collider2D _collider;

        void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        public static event EventHandler<Collider2D> PlayerStayFollowRange;

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == PhysicsLayer.PLAYER) return;
            
            PlayerStayFollowRange?.Invoke(this, _collider);
        }
    }
}