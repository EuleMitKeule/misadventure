using System.Net.Http.Headers;
using HotlineHyrule.Extensions;
using Sirenix.Serialization;
using UnityEngine;

namespace HotlineHyrule.Entities.CaterpillarStates
{
    public class CaterpillarFollowStateComponent : CaterpillarBaseStateComponent
    {
        public override void FixedUpdateState(SegmentComponent segment)
        {
            if (!CaterpillarComponent.IsPlayerVisible(segment)) SetState<CaterpillarMovingStateComponent>(segment);

            var rigidbody = SegmentToRigidbody[segment];
            rigidbody.velocity = CaterpillarComponent.GetPlayerDirection(segment) * CaterpillarComponent.FollowSpeed;
        }
    }
}