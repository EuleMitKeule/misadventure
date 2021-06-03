using HotlineHyrule.Extensions;
using UnityEngine;

namespace HotlineHyrule.Entities.CaterpillarStates
{
    public class CaterpillarMovingStateComponent : CaterpillarBaseStateComponent
    {
        [SerializeField] float speed;
        [SerializeField] float maxAngle;

        public override void EnterState(SegmentComponent segment)
        {
            base.EnterState(segment);

            var rigidbody = SegmentToRigidbody[segment];
            rigidbody.velocity = rigidbody.transform.up * speed;
        }
        
        public override void UpdateState(SegmentComponent segment)
        {
            base.UpdateState(segment);

            var rigidbody = SegmentToRigidbody[segment];
            var angle = Random.Range(-maxAngle, maxAngle);
            var newVelocity = rigidbody.velocity.Rotate(angle).normalized * speed;
            rigidbody.velocity = newVelocity;
        }
    }
}