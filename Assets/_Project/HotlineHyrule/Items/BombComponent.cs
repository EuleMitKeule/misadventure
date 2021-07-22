using HotlineHyrule.Entities;
using UnityEngine;

namespace HotlineHyrule.Items
{
    public class BombComponent : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D other)
        {

            Debug.Log("BOOM!");
            Debug.Log(other.gameObject.layer);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("BOOM!");
            Debug.Log(other.gameObject.layer);
            var healthComponent = other.GetComponent<HealthComponent>();

            if (!healthComponent) return;

            healthComponent.Health -= healthComponent.maxHealth;
        }
    }
}
