using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HotlineHyrule.Entities.CaterpillarStates
{
    public abstract class CaterpillarBaseStateComponent : MonoBehaviour
    {
        protected Dictionary<SegmentComponent, Rigidbody2D> SegmentToRigidbody { get; } =
            new Dictionary<SegmentComponent, Rigidbody2D>();
        
        public event EventHandler<CaterpillarStateEventArgs> ChangeRequested;
        
        protected void SetState<TStateType>(SegmentComponent segment) where TStateType : CaterpillarBaseStateComponent
        {
            ChangeRequested?.Invoke(this, new CaterpillarStateEventArgs(typeof(TStateType), segment));
        }
        
        protected void SetState(Type stateType, SegmentComponent segment)
        {
            ChangeRequested?.Invoke(this, new CaterpillarStateEventArgs(stateType, segment));
        }

        public virtual void EnterState(SegmentComponent segment)
        {
            
        }

        public virtual void ExitState(SegmentComponent segment)
        {
            
        }

        public virtual void UpdateState(SegmentComponent segment)
        {
            
        }
        
        protected virtual void Awake()
        {
            var segments = GetComponentsInChildren<SegmentComponent>();

            foreach (var segment in segments)
            {
                var rigidbody = segment.GetComponent<Rigidbody2D>();
                if (!rigidbody) continue;
                
                SegmentToRigidbody.Add(segment, rigidbody);
            }
        }
    }
}