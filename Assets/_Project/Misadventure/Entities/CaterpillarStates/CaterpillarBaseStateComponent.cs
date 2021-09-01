using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misadventure.Entities.CaterpillarStates
{
    public abstract class CaterpillarBaseStateComponent : SerializedMonoBehaviour
    {
        protected CaterpillarComponent CaterpillarComponent { get; private set; }
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
            if (!SegmentToRigidbody.ContainsKey(segment))
            {
                var rigidbody = segment.GetComponent<Rigidbody2D>();
                SegmentToRigidbody.Add(segment, rigidbody);
            }
        }

        public virtual void ExitState(SegmentComponent segment)
        {
            
        }

        public virtual void UpdateState(SegmentComponent segment)
        {
            
        }

        public virtual void FixedUpdateState(SegmentComponent segment)
        {
            
        }
        
        protected virtual void Awake()
        {
            CaterpillarComponent = GetComponent<CaterpillarComponent>();
            
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