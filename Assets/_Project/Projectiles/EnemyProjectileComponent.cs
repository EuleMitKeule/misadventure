using HotlineHyrule.Entities;
using UnityEngine;

namespace System
{
    public class EnemyProjectileComponent : MonoBehaviour
    {
        [SerializeField] float moveSpeed;
        [SerializeField] int damage;

        Rigidbody2D _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            _rb.velocity = transform.up * moveSpeed * Time.deltaTime;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("player"))
            {
                var healthComp = other.gameObject.GetComponent<HealthComponent>();
                healthComp.Health -= damage;
                Destroy(gameObject);
            };

            if (other.gameObject.layer == LayerMask.NameToLayer("wall"))
            {
                Destroy(gameObject);
            }
        }
    }
}