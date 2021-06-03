using System;
using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Entities.CaterpillarStates;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HotlineHyrule.Entities
{
    public class CaterpillarComponent : MonoBehaviour
    {
        [SerializeField] int healthPerHead;
        
        List<SegmentComponent> Segments { get; set; }
        Dictionary<SegmentComponent, CaterpillarBaseStateComponent> SegmentToState { get; } = new Dictionary<SegmentComponent, CaterpillarBaseStateComponent>();
        List<CaterpillarBaseStateComponent> States { get; set; }
        Dictionary<SegmentComponent, HealthComponent> SegmentToHealthComponent { get; } =
            new Dictionary<SegmentComponent, HealthComponent>();
        Dictionary<SegmentComponent, EventHandler<HealthEventArgs>> SegmentToHealthCallback { get; } =
            new Dictionary<SegmentComponent, EventHandler<HealthEventArgs>>();

        void Awake()
        {
            Segments = GetComponentsInChildren<SegmentComponent>().ToList();
            States = GetComponents<CaterpillarBaseStateComponent>().ToList();

            foreach (var segment in Segments)
            {
                var healthComponent = gameObject.AddComponent<HealthComponent>();
                healthComponent.maxHealth = healthPerHead;
                healthComponent.startHealth = healthPerHead;
                SegmentToHealthComponent.Add(segment, healthComponent);

                var callback = new EventHandler<HealthEventArgs>((sender, e) => OnHealthChanged(sender, e, segment));
                SegmentToHealthCallback.Add(segment, callback);
                healthComponent.HealthChanged += callback;
            }
        }

        void Start()
        {
            SetState<CaterpillarMovingStateComponent>(Segments[0].Head);
        }

        void Update()
        {
            foreach (var segment in SegmentToState.Keys)
            {
                var state = SegmentToState[segment];
                if (state) state.UpdateState(segment);
            }
        }
        
        void SetState<TStateType>(SegmentComponent segment) where TStateType : CaterpillarBaseStateComponent => SetState(typeof(TStateType), segment);

        void SetState(Type stateType, SegmentComponent segment)
        {
            if (!stateType.IsSubclassOf(typeof(CaterpillarBaseStateComponent))) return;

            var nextState = States.First(e => e.GetType() == stateType);
            if (!nextState) return;
            
            if (SegmentToState.ContainsKey(segment))
            {
                var state = SegmentToState[segment];
                if (state)
                {
                    state.ExitState(segment);
                    state.ChangeRequested -= OnChangeRequested;
                }
            }
            else SegmentToState.Add(segment, nextState);
            
            SegmentToState[segment] = nextState;
            nextState.ChangeRequested += OnChangeRequested;
            nextState.EnterState(segment);
        }

        void OnChangeRequested(object sender, CaterpillarStateEventArgs e) => SetState(e.StateType, e.Segment);

        void OnHealthChanged(object sender, HealthEventArgs e, SegmentComponent segment)
        {
            if (!segment.IsHead)
            {
                var head = segment.Head;

                if (!head || !SegmentToHealthComponent.ContainsKey(head)) return;
                
                var headHealthComponent = SegmentToHealthComponent[head];
                headHealthComponent.Health += e.HealthDifference;
                
                return;
            }

            if (e.IsKilled)
            {
                if (!segment.ChildSegment)
                {
                    Kill(segment);
                    return;
                }
                
                Split(segment);
            }
        }

        void Split(SegmentComponent headSegment)
        {
            var segments = headSegment.GetSegments();

            if (segments.Count <= 1) return; 

            foreach (var segment in segments)
            {
                var healthComponent = SegmentToHealthComponent[segment];
                healthComponent.HealthChanged -= SegmentToHealthCallback[segment];
                healthComponent.ResetHealth();
                healthComponent.HealthChanged += SegmentToHealthCallback[segment];
            }
            
            var newHeadIndex = segments.Count / 2;
            var newHeadSegment = segments[newHeadIndex];
            newHeadSegment.SplitHere();
            
            SetState<CaterpillarMovingStateComponent>(newHeadSegment);
        }

        void Kill(SegmentComponent segment)
        {
            SegmentToState.Remove(segment);
            Destroy(segment.gameObject);
        }
    }
}