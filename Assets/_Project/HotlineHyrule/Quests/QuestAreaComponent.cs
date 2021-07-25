using System;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    public class QuestAreaComponent : MonoBehaviour
    {
        public event EventHandler PlayerEntered;  
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("player")) return;
            PlayerEntered?.Invoke(this, EventArgs.Empty);
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("player")) return;
            PlayerEntered?.Invoke(this, EventArgs.Empty);
        }
    }
}