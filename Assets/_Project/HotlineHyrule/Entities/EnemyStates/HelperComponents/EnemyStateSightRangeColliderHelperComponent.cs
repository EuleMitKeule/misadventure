using System;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates.HelperComponents
{
    public class EnemyStateSightRangeColliderHelperComponent : MonoBehaviour
    {
        public static event EventHandler PlayerEntersSightRange;
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (LayerMask.NameToLayer("player") != other.gameObject.layer) return;
            PlayerEntersSightRange?.Invoke(this, EventArgs.Empty);
        }
    }
}