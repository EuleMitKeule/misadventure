using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HotlineHyrule.Entities.CaterpillarStates;
using HotlineHyrule.Extensions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HotlineHyrule.Entities
{
    public class CaterpillarComponent : SerializedMonoBehaviour
    {
        [OdinSerialize] int HealthPerHead { get; set; }
        [OdinSerialize] public float MoveSpeed { get; set; }
        [OdinSerialize] public float FollowSpeed { get; set; }
        [OdinSerialize] public float MaxTurnAngle { get; set; }
        [OdinSerialize] float ViewAngle { get; set; }
        [OdinSerialize] public float ViewDistance { get; set; }
        [OdinSerialize] int Damage { get; set; }
        [OdinSerialize] float DamageInterval { get; set; }
        [OdinSerialize] public LayerMask ViewPlayerMask { get; set; }
        
        float NextDamageTime { get; set; }
        List<SegmentComponent> Segments { get; set; }
        Dictionary<SegmentComponent, CaterpillarBaseStateComponent> SegmentToState { get; } = new Dictionary<SegmentComponent, CaterpillarBaseStateComponent>();
        List<CaterpillarBaseStateComponent> States { get; set; }
        Dictionary<SegmentComponent, HealthComponent> SegmentToHealthComponent { get; } =
            new Dictionary<SegmentComponent, HealthComponent>();

        void Awake()
        {
            Segments = GetComponentsInChildren<SegmentComponent>().ToList();
            States = GetComponents<CaterpillarBaseStateComponent>().ToList();

            for (var i = 0; i < Segments.Count; i++)
            {
                var segment = Segments[i];

                if (i >= 1) segment.SetParentSegment(Segments[i - 1]);
                if (i <= Segments.Count - 2) segment.SetChildSegment(Segments[i + 1]);
                
                var healthComponent = segment.gameObject.AddComponent<HealthComponent>();
                healthComponent.maxHealth = HealthPerHead;
                healthComponent.startHealth = HealthPerHead;
                SegmentToHealthComponent.Add(segment, healthComponent);

                healthComponent.HealthChanged += OnHealthChanged;
                segment.PlayerColliding += OnPlayerColliding;
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

        void FixedUpdate()
        {
            foreach (var segment in Segments)
            {
                if (!SegmentToState.ContainsKey(segment)) continue;
                
                var state = SegmentToState[segment];
                if (state) state.FixedUpdateState(segment);
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

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            var healthComponent = sender as HealthComponent;
            if (!healthComponent) return;
            
            var segment = healthComponent.GetComponent<SegmentComponent>();
            if (!segment) return;
            
            if (!segment.IsHead)
            {
                var head = segment.Head;

                if (!head || !SegmentToHealthComponent.ContainsKey(head)) return;
                
                var headHealthComponent = SegmentToHealthComponent[head];
                headHealthComponent.Health += e.HealthDifference;
                
                return;
            }
            
            SetState<CaterpillarFollowStateComponent>(segment.Head);

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

        void OnPlayerColliding(object sender, PlayerEventArgs e)
        {
            if (Time.time < NextDamageTime) return;
            NextDamageTime = Time.time + 1 / DamageInterval;
            
            var healthComponent = e.PlayerComponent.GetComponent<HealthComponent>();
            healthComponent.Health -= Damage;
        }

        void Split(SegmentComponent headSegment)
        {
            var segments = headSegment.GetSegments();

            if (segments.Count <= 1) return; 

            foreach (var segment in segments)
            {
                var healthComponent = SegmentToHealthComponent[segment];
                healthComponent.HealthChanged -= OnHealthChanged;
                healthComponent.ResetHealth();
                healthComponent.HealthChanged += OnHealthChanged;
            }
            
            var newHeadIndex = segments.Count / 2;
            var newHeadSegment = segments[newHeadIndex];
            newHeadSegment.SplitHere();
            
            SetState<CaterpillarMovingStateComponent>(newHeadSegment);
        }

        void Kill(SegmentComponent segment)
        {
            SegmentToState.Remove(segment);
            SegmentToHealthComponent[segment].HealthChanged -= OnHealthChanged;
            SegmentToHealthComponent.Remove(segment);
            
            Destroy(segment.gameObject);
        }
        
        public Vector3 GetPlayerDirection(SegmentComponent segment) =>
            segment.transform.position.DirectionTo(Locator.PlayerComponent.transform.position);

        public bool IsPlayerVisible(SegmentComponent segment)
        {
            var lookAngle = Vector3.SignedAngle(segment.LookDirection, GetPlayerDirection(segment), Vector3.forward);
            if (Mathf.Abs(lookAngle) > ViewAngle / 2) return false;
            
            var raycastHit =
                Physics2D.Raycast(segment.transform.position, GetPlayerDirection(segment), ViewDistance, ViewPlayerMask);
            if (!raycastHit.transform) return false;
            
            var playerComponent = raycastHit.transform.gameObject.GetComponent<PlayerComponent>();
            return playerComponent;
        }
    }
}