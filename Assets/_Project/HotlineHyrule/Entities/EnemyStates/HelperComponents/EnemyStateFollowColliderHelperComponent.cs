using System;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates.HelperComponents
{
    public class EnemyStateFollowColliderHelperComponent : MonoBehaviour
    {
        public static event EventHandler PlayerLeftFollowRange;
        
        void OnTriggerExit2D(Collider2D other)
        {
            if (LayerMask.NameToLayer("player") != other.gameObject.layer) return;
            PlayerLeftFollowRange?.Invoke(this, EventArgs.Empty);
        }
    }
}