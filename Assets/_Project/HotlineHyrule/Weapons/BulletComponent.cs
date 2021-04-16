using UnityEngine;

namespace HotlineHyrule.Weapons
{
    public class BulletComponent : MonoBehaviour
    {
        /// <summary>
        /// The sprite changed to after an impact.
        /// </summary>
        [SerializeField] Sprite impactSprite;
        /// <summary>
        /// Layermask that contains layers that count as impacts.
        /// </summary>
        [SerializeField] public LayerMask impactMask;

        SpriteRenderer SpriteRenderer { get; set; }
        Rigidbody2D Rigidbody { get; set; }

        void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (impactMask.value != (impactMask.value | 1 << other.gameObject.layer)) return;

            Rigidbody.velocity = Vector2.zero;
            SpriteRenderer.sprite = impactSprite;
        }

        void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}
