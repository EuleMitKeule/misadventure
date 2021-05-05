using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{

    public class EnemyStateDyingComponent : EnemyStateBaseComponent
    {
        public override void Setup()
        {
            base.Setup();
            Rigidbody.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            foreach (var collider in GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
            Animator.SetTrigger("dying");
        }
    }
}