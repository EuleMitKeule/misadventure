using System.Collections.Generic;
using System.Net.Http.Headers;
using HotlineHyrule.Extensions;
using UnityEngine;

namespace HotlineHyrule.Entities.CaterpillarStates
{
    public class CaterpillarMovingStateComponent : CaterpillarBaseStateComponent
    {
        float Speed => CaterpillarComponent.MoveSpeed;
        float MaxAngle => CaterpillarComponent.MaxTurnAngle;

        public override void EnterState(SegmentComponent segment)
        {
            base.EnterState(segment);

            var rigidbody = SegmentToRigidbody[segment];
            rigidbody.velocity = rigidbody.transform.up * Speed;
        }
        
        public override void FixedUpdateState(SegmentComponent segment)
        {
            base.FixedUpdateState(segment);

            if (CaterpillarComponent.IsPlayerVisible(segment)) SetState<CaterpillarFollowStateComponent>(segment);
            
            var rigidbody = SegmentToRigidbody[segment];
            var angle = Random.Range(-MaxAngle, MaxAngle);

            var aboveHit = segment.WallAbove;
            if (aboveHit)
            {
                var wallAngle = Vector3.SignedAngle(segment.LookDirection, aboveHit.normal, Vector3.forward);
                var sign = Mathf.Sign(wallAngle);
                
                var leftHit = segment.WallLeft;
                var rightHit = segment.WallRight;

                if (leftHit && rightHit) angle = 180;
                else if (leftHit) angle = -MaxAngle;
                else if (rightHit) angle = MaxAngle;
                else angle = MaxAngle * sign;
            }
            
            var newVelocity = segment.LookDirection.Rotate(angle).normalized * Speed;
            rigidbody.velocity = newVelocity;
        }
    }
}