using System;
using System.Collections.Generic;
using System.Linq;
using Misadventure.Entities.EnemyStates;
using Misadventure.Extensions;
using Misadventure.Items;
using Misadventure.Weapons;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Misadventure.Entities
{
    /// <summary>
    /// prototype; only contains a respawn point for now
    /// </summary>
    public class EnemyComponent : MonoBehaviour
    {
        /// <summary>
        /// The enemy's respawn point.
        /// </summary>
        [Header("General")]
        [SerializeField] public Vector2Int respawnPoint;
        /// <summary>
        /// List of items that can be dropped by a given chance when the enemy is destroyed.
        /// </summary>
        [SerializeField] List<ItemDrop> itemDrops;

        /// <summary>
        /// The enemy's current state.
        /// </summary>
        [Header("AI")]
        [SerializeField] public EnemyBaseStateComponent state;
        /// <summary>
        /// Layers that count as wall.
        /// </summary>
        [SerializeField] LayerMask wallMask;
        /// <summary>
        /// Distance to check for walls at.
        /// </summary>
        [SerializeField] float wallCheckDistance;
        /// <summary>
        /// Total angle of the vision arc.
        /// </summary>
        [SerializeField] float followAngle;
        /// <summary>
        /// Range in which the enemy will follow.
        /// </summary>
        [SerializeField] float followRange;
        /// <summary>
        /// Range in which the enemy will attack.
        /// </summary>
        [SerializeField] float attackRange;
        /// <summary>
        /// Whether a second attack shall be used.
        /// If enabled, Animation for 'attacking2' will be played instead of 'attacking'
        /// </summary>
        public bool UsingAttack2;
        /// <summary>
        /// Name of Animation trigger that shall be triggered on attack
        /// </summary>
        public string AttackAnimationTrigger = "attack";
        /// <summary>
        /// If Rigidbody velocity shall stay on zero or can be changed
        /// </summary>
        public bool LockVelocity;
        /// <summary>
        /// If last rotation shall remain or rotation can be changed
        /// </summary>
        public bool LockRotation;

        /// <summary>
        /// Particle system prefab to spawn when taking damage.
        /// </summary>
        [Header("Effects")]
        [SerializeField] GameObject damageParticleSystemPrefab;

        Vector2 Velocity { get; set; }

        bool IsInMovementAnim => Animator.GetBool("isMoving");

        /// <summary>
        /// The player's current position.
        /// </summary>
        public Vector3 PlayerPosition => Locator.PlayerComponent.transform.position;
        /// <summary>
        /// The direction vector to the player.
        /// </summary>
        public Vector3 PlayerDirection => transform.position.DirectionTo(PlayerPosition);

        /// <summary>
        /// The absolute angle of the current movement direction.
        /// </summary>
        float MovementAngle => Vector3.SignedAngle(Vector3.up, Rigidbody.velocity, Vector3.forward);
        /// <summary>
        /// The absolute angle of the current player direction.
        /// </summary>
        float FollowAngle => Vector3.SignedAngle(Vector3.up, PlayerDirection, Vector3.forward);
        /// <summary>
        /// The relative angle of the current player direction.
        /// </summary>
        float PlayerAngle => Vector3.SignedAngle(transform.up, PlayerDirection, Vector3.forward);
        /// <summary>
        /// The absolute rotation towards the current movement angle.
        /// </summary>
        public Quaternion WalkRotation => Quaternion.Euler(0f, 0f, MovementAngle);
        /// <summary>
        /// The absolute rotation towards the current player direction.
        /// </summary>
        public Quaternion FollowRotation => Quaternion.Euler(0f, 0f, FollowAngle);

        /// <summary>
        /// Whether the player angle is inside the vision arc.
        /// </summary>
        bool IsPlayerInAngle => Mathf.Abs(PlayerAngle) < followAngle;
        /// <summary>
        /// Whether the distance to the player is inside the follow range.
        /// </summary>
        bool IsPlayerInFollowRange => transform.position.DistanceTo(PlayerPosition) <= followRange;
        /// /// <summary>
        /// Whether the distance to the player is inside the attack range.
        /// </summary>
        bool IsPlayerInAttackRange => transform.position.DistanceTo(PlayerPosition) <= attackRange;
        /// <summary>
        /// Whether the enemy is currently able to follow the player.
        /// </summary>
        public bool IsPlayerFollowable => Locator.PlayerComponent && IsPlayerInFollowRange && IsPlayerInAngle && IsPlayerVisible;
        /// <summary>
        /// Whether the enemy is currently able to attack the player.
        /// </summary>
        public bool IsPlayerAttackable => IsPlayerInAttackRange && IsPlayerInAngle && IsPlayerVisible;
        /// <summary>
        /// Whether the player is in front of any walls.
        /// </summary>
        bool IsPlayerVisible
        {
            get
            {
                var raycastHit = Physics2D.Raycast(
                    transform.position,
                    PlayerDirection,
                    followRange,
                    wallMask | 1 << PhysicsLayer.PLAYER
                );

                return raycastHit && raycastHit.transform.gameObject.layer.IsPlayer();
            }
        }

        /// <summary>
        /// Get 2D distance towards Player
        /// </summary>
        public float PlayerDistance => Vector2.Distance(transform.position, Locator.PlayerComponent.transform.position);

        /// <summary>
        /// Whether the enemy has a wall to its left.
        /// </summary>
        public bool IsWallLeft =>
            Physics2D.BoxCast(
                transform.position,
                Collider.bounds.size,
                0f,
                -transform.right,
                wallCheckDistance,
                wallMask
            );
        /// <summary>
        /// Whether the enemy has a wall to its right.
        /// </summary>
        public bool IsWallRight =>
            Physics2D.BoxCast(
                transform.position,
                Collider.bounds.size,
                0f,
                transform.right,
                wallCheckDistance,
                wallMask
            );
        /// <summary>
        /// Whether the enemy has a wall above.
        /// </summary>
        public bool IsWallAbove =>
            Physics2D.BoxCast(
                transform.position,
                Collider.bounds.size,
                0f,
                transform.up,
                wallCheckDistance,
                wallMask
            );

        public static event EventHandler<EntityEventArgs> EnemyKilled;

        Rigidbody2D Rigidbody { get; set; }
        Collider2D Collider { get; set; }
        Animator Animator { get; set; }
        HealthComponent HealthComponent { get; set; }
        public WeaponComponent WeaponComponent { get; set; }

        List<EnemyBaseStateComponent> States { get; set; }

        Quaternion lastRotation;

        public Type PassiveState =>
            States.Find(e =>
                    e is EnemyPatrolStateComponent ||
                    e is EnemyGuardStateComponent ||
                    e is GhostGuardStateComponent ||
                    e is EnemyIdleStateComponent) 
                ?.GetType();

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            Animator = GetComponent<Animator>();
            HealthComponent = GetComponent<HealthComponent>();
            States = GetComponents<EnemyBaseStateComponent>().ToList();

            WeaponComponent = GetComponent<WeaponComponent>();

            HealthComponent.HealthChanged += OnHealthChanged;
        }

        void Start()
        {
            SetState(PassiveState);
        }

        void FixedUpdate()
        {
            if (state && Locator.GameComponent.gameplayEnabled) state.FixedUpdateState();

            Rigidbody.velocity = LockVelocity ?  Vector2.zero : Velocity;
            if (!LockRotation)
                lastRotation = transform.rotation;
            else
                transform.rotation = lastRotation;
        }

        void Update()
        {
            if (state && Locator.GameComponent.gameplayEnabled) state.UpdateState();

            if (Locator.PlayerComponent)
            {
                var weaponDirection = WeaponComponent.WeaponPosition.To(PlayerPosition);
                var weaponAngle = Vector3.SignedAngle(Vector3.up, weaponDirection, Vector3.forward);
                if (WeaponComponent.HasRangedWeapon || WeaponComponent.HasTargetingWeapon)
                {
                    WeaponComponent.SetWeaponRotation(weaponAngle);
                }
            }

#if UNITY_EDITOR
            if (IsPlayerFollowable) Debug.DrawLine(transform.position, PlayerPosition, IsPlayerAttackable ? Color.green : Color.red);
#endif
        }

        public void SetState<TStateType>() where TStateType : EnemyBaseStateComponent => SetState(typeof(TStateType));

        public void SetState(Type stateType)
        {
            if (stateType == null) return;
            if (!stateType.IsSubclassOf(typeof(EnemyBaseStateComponent))) return;

            var nextState = States.Find(e => e.GetType() == stateType || e.GetType().IsSubclassOf(stateType));
            if (!nextState) return;

            if (state)
            {
                state.ExitState();
                state.ChangeRequested -= OnChangeRequested;
            }

            state = nextState;
            state.ChangeRequested += OnChangeRequested;
            state.EnterState();
        }

        void OnChangeRequested(Type stateType) => SetState(stateType);

        public void SetVelocity(Vector2 velocity)
        {
            if (IsInMovementAnim != (velocity != Vector2.zero))
            {
                if (Animator) Animator.SetBool("isMoving", velocity != Vector2.zero);
            }

            Velocity = velocity;
        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (state) state.OnHealthChanged(sender, e);

            if (e.IsDamage)
            {
                if (damageParticleSystemPrefab) Instantiate(damageParticleSystemPrefab, transform.position, Quaternion.identity);
            }

            if (e.IsKilled)
            {
                SetState<EnemyDyingStateComponent>();

                DropItems();

                var entityEventArgs = new EntityEventArgs(gameObject);
                EnemyKilled?.Invoke(this, entityEventArgs);
            }
        }

        void DropItems()
        {
            foreach (var item in itemDrops)
            {
                if (Random.value <= item.dropRate)
                {
                    var itemObject = Instantiate(item.data.ItemPrefab, transform.position, Quaternion.identity);
                    var droppedWeaponComponent = itemObject.GetComponent<DroppedWeaponComponent>();
                    var itemComponent = itemObject.GetComponent<ItemComponent>();

                    var weaponData = item.data as WeaponData;

                    if (!weaponData) continue;
                    if (!droppedWeaponComponent) continue;

                    var chargeOffsetFactor =
                        Random.Range(-weaponData.chargeRandomness, weaponData.chargeRandomness);
                    droppedWeaponComponent.weaponCharges = weaponData.weaponCharges +
                                                           (int)(weaponData.weaponCharges * chargeOffsetFactor);
                }
            }
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (state) state.OnCollisionEnterState(other);
        }
        
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (state != null)
            {
                var style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.white;
                
                Handles.Label(transform.position, state.GetType().Name, style);
            }
        }
#endif
    }
}