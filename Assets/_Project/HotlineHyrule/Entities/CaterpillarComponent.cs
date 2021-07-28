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
        [BoxGroup("General")]
        [OdinSerialize]
        [MinValue("SegmentCount")]
        [HideInPlayMode]
        int Health { get; set; }
        [BoxGroup("General")]
        [ShowInInspector]
        [HideInPlayMode]
        int HealthPerHead => Health / SegmentCount;
        [BoxGroup("General")]
        [ShowInInspector]
        [HideInEditorMode]
        [ProgressBar(0f, "Health", 1f, 0f, 0f)]
        int CurrentHealth { get; set; }
        [BoxGroup("General")]
        [OdinSerialize]
        [MinValue(0)]
        int Damage { get; set; }
        [BoxGroup("General")]
        [OdinSerialize]
        [MinValue(0f)]
        [SuffixLabel("1/s")]
        float DamageFrequency { get; set; }
        [BoxGroup("General")]
        [OdinSerialize]
        RuntimeAnimatorController HeadController { get; set; }
        [BoxGroup("General")]
        [OdinSerialize]
        RuntimeAnimatorController BodyController { get; set; }
        [BoxGroup("General")]
        [OdinSerialize]
        RuntimeAnimatorController TailController { get; set; }
        [BoxGroup("AI")]
        [OdinSerialize]
        [MinValue(0f)]
        [SuffixLabel("u/s")]
        public float MoveSpeed { get; set; }
        [BoxGroup("AI")]
        [OdinSerialize]
        [MinValue(0f)]
        [SuffixLabel("u/s")]
        public float FollowSpeed { get; set; }
        [BoxGroup("AI")]
        [OdinSerialize]
        [PropertyRange(0f, 360f)]
        [SuffixLabel("°")]
        float ViewAngle { get; set; }
        [BoxGroup("AI")]
        [OdinSerialize]
        [MinValue(float.Epsilon)]
        [SuffixLabel("u")]
        public float ViewDistance { get; set; }
        [BoxGroup("AI")]
        [OdinSerialize]
        [LabelText("What is Player")]
        LayerMask PlayerMask { get; set; }
        [BoxGroup("Segment")]
        [OdinSerialize]
        [Required]
        [LabelText("Prefab")]
        [HideInPlayMode]
        GameObject SegmentPrefab { get; set; }
        [BoxGroup("Segment")]
        [OdinSerialize]
        [MinValue(1)]
        [LabelText("Count")]
        [HideInPlayMode]
        int SegmentCount { get; set; }
        [BoxGroup("Segment")]
        [OdinSerialize]
        [MinValue("NodeDistance")]
        [LabelText("Distance")]
        [SuffixLabel("u")]
        public float SegmentDistance { get; set; }
        [BoxGroup("Segment")]
        [OdinSerialize]
        [MinValue(float.Epsilon)]
        [LabelText("Path Precision")]
        [SuffixLabel("u")]
        public float NodeDistance { get; set; }
        [BoxGroup("Physics")]
        [OdinSerialize]
        [PropertyRange(0f, 360f)]
        [SuffixLabel("°")]
        [LabelText("Max Rotation Step")]
        public float MaxTurnAngle { get; set; }
        [BoxGroup("Physics")]
        [OdinSerialize]
        [PropertyRange(0f, "FollowSpeed")]
        [SuffixLabel("u/s")]
        public float MaxSpeedOffset { get; private set; }
        [BoxGroup("Physics")]
        [OdinSerialize]
        [MinValue(float.Epsilon)]
        [SuffixLabel("u")]
        public float WallCheckDistance { get; set; }
        [BoxGroup("Physics")]
        [OdinSerialize]
        [LabelText("What is Wall")]
        public LayerMask WallMask { get; set; }
        [BoxGroup("Physics")]
        [OdinSerialize]
        [PropertyRange(0f, 100f)]
        [SuffixLabel("%")]
        [LabelText("Head Rotation Damping")]
        float HeadRotationDampingPercentage { get; set; }
        public float HeadRotationDamping => 1f - HeadRotationDampingPercentage / 100f;
        [BoxGroup("Physics")]
        [OdinSerialize]
        [MinValue(0f)]
        [SuffixLabel("u/s")]
        public float HeadMovementThreshold { get; set; }
        public LayerMask ViewPlayerMask => PlayerMask | WallMask;
        [FoldoutGroup("Debug")]
        [ShowInInspector]
        [ReadOnly]
        float NextDamageTime { get; set; }
        List<SegmentComponent> Segments { get; set; }
        [FoldoutGroup("Debug")]
        [ShowInInspector]
        [ReadOnly]
        Dictionary<SegmentComponent, CaterpillarBaseStateComponent> SegmentToState { get; } = new Dictionary<SegmentComponent, CaterpillarBaseStateComponent>();
        List<CaterpillarBaseStateComponent> States { get; set; }
        [FoldoutGroup("Debug")]
        [ShowInInspector]
        [ReadOnly]
        Dictionary<SegmentComponent, HealthComponent> SegmentToHealthComponent { get; } =
            new Dictionary<SegmentComponent, HealthComponent>();
        [FoldoutGroup("Debug")]
        [ShowInInspector]
        [ReadOnly]
        Dictionary<SegmentComponent, Animator> SegmentToAnimator { get; } =
            new Dictionary<SegmentComponent, Animator>();

        public static event EventHandler<EntityEventArgs> CaterpillarKilled;
        
        void Awake()
        {
            Segments = new List<SegmentComponent>();
            States = GetComponents<CaterpillarBaseStateComponent>().ToList();

            for (var i = 0; i < SegmentCount; i++)
            {
                var segmentObject = Instantiate(SegmentPrefab, transform);
                var segment = segmentObject.GetComponent<SegmentComponent>();
                
                Segments.Add(segment);

                var animator = segment.GetComponent<Animator>();
                SegmentToAnimator.Add(segment, animator);

                animator.runtimeAnimatorController = i == 0 ? HeadController : i == SegmentCount - 1 ? TailController : BodyController;
                animator.SetBool("isMoving", true);
            }

            for (var i = 0; i < SegmentCount; i++)
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

            CurrentHealth = Health;
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
            NextDamageTime = Time.time + 1 / DamageFrequency;
            
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

                if (segment.IsTail)
                {
                    var tailAnimator = SegmentToAnimator[segment].runtimeAnimatorController = TailController;
                }
            }
            
            var newHeadIndex = segments.Count / 2;
            var newHeadSegment = segments[newHeadIndex];
            newHeadSegment.SplitHere();

            var animator = SegmentToAnimator[newHeadSegment];
            if (animator) animator.runtimeAnimatorController = HeadController;
            
            SetState<CaterpillarMovingStateComponent>(newHeadSegment);
        }

        void Kill(SegmentComponent segment)
        {
            var animator = SegmentToAnimator[segment];
            if (animator) animator.SetTrigger("dying");

            CurrentHealth -= HealthPerHead;
            
            SegmentToState.Remove(segment);
            SegmentToHealthComponent[segment].HealthChanged -= OnHealthChanged;
            SegmentToHealthComponent.Remove(segment);
            Segments.Remove(segment);
            
            Destroy(segment.gameObject);

            if (Segments.Count == 0)
            {

                var entityEventArgs = new EntityEventArgs(gameObject);
                CaterpillarKilled?.Invoke(this, entityEventArgs);
                
                Destroy(gameObject);
            }
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