using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HotlineHyrule.Entities.EnemyStates
{
    [RequireComponent(typeof(EnemyComponent))]
    public class EnemyStatePatrolComponent : EnemyStateBaseComponent
    {
        [SerializeField] float moveSpeed = 100f;

        Rigidbody2D _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public override void FixedStateUpdate()
        {
            _rb.velocity = transform.up * (moveSpeed * Time.deltaTime);
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (LayerMask.LayerToName(other.gameObject.layer) != "wall") return;

            var angle = 90f;

            if (Random.value >= 0.5f) angle *= -1f;

            transform.eulerAngles += Vector3.forward * angle;
        }
    }
}