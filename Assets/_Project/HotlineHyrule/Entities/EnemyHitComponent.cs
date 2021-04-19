using System;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    public class EnemyHitComponent : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D other)
        {
            if (LayerMask.LayerToName(other.gameObject.layer) != "projectile") return;
            
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}