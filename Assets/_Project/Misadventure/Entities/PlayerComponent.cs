using System;
using System.Collections.Generic;
using System.Linq;
using Misadventure.Entities.PlayerStates;
using Misadventure.Items;
using Misadventure.Level;
using Misadventure.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Misadventure.Entities
{
    /// <summary>
    /// Handles the player's movement.
    /// </summary>
    public class PlayerComponent : MonoBehaviour, IMovementComponent
    {
        /// <summary>
        /// The player's movement speed.
        /// </summary>
        [Header("Physics")] [SerializeField] public float movementSpeed;
        /// <summary>
        /// The damping value applied to the movement axis.
        /// </summary>
        [SerializeField] float moveDamping;
        /// <summary>
        /// The force to apply when performing a dodge move.
        /// </summary>
        [SerializeField] public float dodgeForce = 1f;
        /// <summary>
        /// The duration a dodge move takes.
        /// </summary>
        [SerializeField] public float dodgeDuration = 1f;
        /// <summary>
        /// The delay between dodge moves.
        /// </summary>
        [SerializeField] public float dodgeDelay = 2f;

        /// <summary>
        /// The minimum amount of velocity that is considered movement for animation purposes.
        /// </summary>
        [Header("Animation")]
        [SerializeField] float moveAnimationThreshold;
        /// <summary>
        /// The animator of the player's legs.
        /// </summary>
        [SerializeField] Animator legsAnimator;
        /// <summary>
        /// The particle system to spawn when taking damage.
        /// </summary>
        [SerializeField] GameObject damageParticleSystemPrefab;

        /// <summary>
        /// The movement input action.
        /// </summary>
        [Header("Input")]
        [SerializeField] public InputAction walkAction;
        /// <summary>
        /// The input action to perform a dodge move.
        /// </summary>
        [SerializeField] public InputAction dodgeAction;

        float NextDodgeTime { get; set; }
        /// <summary>
        /// The damped input axis.
        /// </summary>
        public Vector2 WalkAxis { get; set; }
        /// <summary>
        /// The current mouse position in world space.
        /// </summary>
        Vector2 MousePosition => MainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        /// <summary>
        /// The direction from player to look target.
        /// </summary>
        Vector2 LookDirection => (MousePosition - Rigidbody.position).normalized;
        /// <summary>
        /// The distance between player and look target.
        /// </summary>
        float LookDistance => (MousePosition - Rigidbody.position).magnitude;
        /// <summary>
        /// The angle between look direction and y axis.
        /// </summary>
        float LookAngle => Vector2.SignedAngle(Vector2.up, LookDirection);
        /// <summary>
        /// The direction from weapon to look target.
        /// </summary>
        Vector2 WeaponDirection => (ClampedMousePosition - (Vector2)WeaponComponent.WeaponPosition).normalized;
        /// <summary>
        /// The angle between weapon direction and y axis.
        /// </summary>
        float WeaponAngle => Vector2.SignedAngle(Vector2.up, WeaponDirection);
        /// <summary>
        /// Whether the look target is within the weapon's deadzone.
        /// </summary>
        bool IsInDeadzone => LookDistance < WeaponComponent.RangedWeaponData.deadzoneRadius;
        /// <summary>
        /// The position of the look target projected onto the deadzone circle.
        /// </summary>
        Vector2 DeadzonedMousePosition => Rigidbody.position + LookDirection * WeaponComponent.RangedWeaponData.deadzoneRadius;
        /// <summary>
        /// The position of the look target clamped to the outside of the deadzone.
        /// </summary>
        Vector2 ClampedMousePosition => IsInDeadzone ? DeadzonedMousePosition : MousePosition;
        public bool IsMoving => Rigidbody.velocity.magnitude > moveAnimationThreshold;
        /// <summary>
        /// Multiplies the player's movement speed.
        /// </summary>
        public float MovementAttackFactor { get; set; }
        /// <summary>
        /// Multiplies the player's movement speed.
        /// </summary>
        public float MovementItemFactor { get; set; }

        public event EventHandler MovementStarted;

        Rigidbody2D Rigidbody { get; set; }
        HealthComponent HealthComponent { get; set; }
        WeaponComponent WeaponComponent { get; set; }
        LoadoutComponent LoadoutComponent { get; set; }
        Camera MainCamera { get; set; }

        public PlayerBaseStateComponent State { get; set; }
        List<PlayerBaseStateComponent> States { get; set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            HealthComponent = GetComponent<HealthComponent>();
            WeaponComponent = GetComponent<WeaponComponent>();
            LoadoutComponent = GetComponent<LoadoutComponent>();
            States = GetComponents<PlayerBaseStateComponent>().ToList();

            if (!legsAnimator) legsAnimator = transform.Find("legs").GetComponent<Animator>();

            Locator.PlayerComponent = this;
            MainCamera = Camera.main;
            
            ResetBuffs();

            dodgeAction.started += OnButtonDodge;

            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;

            Locator.LevelComponent.LevelFinished += OnLevelFinished;

            if (e.PlayerStateData)
            {
                HealthComponent.Health = e.PlayerStateData.currentHealth;
            }
            
            HealthComponent.HealthChanged += OnHealthChanged;

            SetState<PlayerFrozenStateComponent>();
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            dodgeAction.Disable();
            walkAction.Disable();

            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }

        void OnLevelFinished(object sender, EventArgs e)
        {
            SetState<PlayerFrozenStateComponent>();
        }

        void Update()
        {
            ProcessInput();
            HandleAnimation();
            HandleRotation();
        }

        void FixedUpdate()
        {
            if (State) State.FixedUpdateState();
        }

        void SetState<TStateType>() where TStateType : PlayerBaseStateComponent => SetState(typeof(TStateType));

        void SetState(Type stateType)
        {
            if (!stateType.IsSubclassOf(typeof(PlayerBaseStateComponent))) return;

            var nextState = States.First(e => e.GetType() == stateType);
            if (!nextState) return;

            if (State)
            {
                State.ExitState();
                State.ChangeRequested -= OnChangeRequested;
            }

            State = nextState;
            State.ChangeRequested += OnChangeRequested;
            State.EnterState();
        }

        void OnChangeRequested(Type stateType) => SetState(stateType);

        void OnButtonDodge(InputAction.CallbackContext context)
        {
            if (Time.time < NextDodgeTime) return;
            NextDodgeTime = Time.time + dodgeDelay;

            SetState<PlayerDodgeStateComponent>();
        }

        /// <summary>
        /// Dampens the current raw movement input axis.
        /// </summary>
        void ProcessInput()
        {
            var value = walkAction.ReadValue<Vector2>();
            WalkAxis = Vector2.MoveTowards(WalkAxis, value, 1 / moveDamping);
        }

        /// <summary>
        /// Performs changes in animation.
        /// </summary>
        void HandleAnimation()
        {
            var isInMovingState = legsAnimator.GetBool("isMoving");

            if (isInMovingState != IsMoving)
            {
                if (IsMoving) MovementStarted?.Invoke(this, EventArgs.Empty);

                legsAnimator.SetBool("isMoving", IsMoving);
            }
        }

        void HandleRotation()
        {
            if (State is PlayerFrozenStateComponent) return;

            Rigidbody.rotation = LookAngle;
            if (WeaponComponent.HasRangedWeapon) WeaponComponent.SetWeaponRotation(WeaponAngle);
        }

        /// <summary>
        /// Applies effects of a given movement item.
        /// </summary>
        /// <param name="movementItem">The item to consume.</param>
        public void Consume(MovementItemData movementItem)
        {
            MovementItemFactor = movementItem.movementFactor;
            Invoke(nameof(ResetBuffs), movementItem.duration);
        }

        /// <summary>
        /// Resets any present item effects.
        /// </summary>
        void ResetBuffs() => (MovementAttackFactor, MovementItemFactor) = (1f, 1f);

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (e.IsDamage)
            {
                if (damageParticleSystemPrefab) Instantiate(damageParticleSystemPrefab, transform.position, Quaternion.identity);
            }

            if (e.IsKilled)
            {
                SetState<PlayerFrozenStateComponent>();
            }
        }

        public PlayerStateData GetStateData()
        {
            var stateData = ScriptableObject.CreateInstance<PlayerStateData>();
            stateData.currentHealth = HealthComponent.Health;
            stateData.loadoutSlots = LoadoutComponent.loadoutSlots;
            stateData.currentLoadoutSlot = LoadoutComponent.CurrentLoadoutSlot;

            return stateData;
        }

        public void EnableMovement() => SetState<PlayerIdleStateComponent>();
    }
}
