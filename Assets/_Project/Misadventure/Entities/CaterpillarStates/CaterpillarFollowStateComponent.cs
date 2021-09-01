using System.Collections.Generic;
using UnityEngine;

namespace Misadventure.Entities.CaterpillarStates
{
    public class CaterpillarFollowStateComponent : CaterpillarBaseStateComponent
    {
        Dictionary<SegmentComponent, float> SegmentToSpeedOffset { get; } =
            new Dictionary<SegmentComponent, float>();
        
        public override void EnterState(SegmentComponent segment)
        {
            base.EnterState(segment);

            if (!SegmentToSpeedOffset.ContainsKey(segment)) SegmentToSpeedOffset.Add(segment, 0f);
            SegmentToSpeedOffset[segment] = Random.Range(-CaterpillarComponent.MaxSpeedOffset, CaterpillarComponent.MaxSpeedOffset);
        }

        public override void FixedUpdateState(SegmentComponent segment)
        {
            if (!CaterpillarComponent.IsPlayerVisible(segment)) SetState<CaterpillarMovingStateComponent>(segment);

            var rigidbody = SegmentToRigidbody[segment];
            var direction = CaterpillarComponent.GetPlayerDirection(segment);
            var speed = CaterpillarComponent.FollowSpeed + SegmentToSpeedOffset[segment];

            rigidbody.velocity = direction * speed;
        }
    }
}