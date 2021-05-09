using HotlineHyrule.Items;
using HotlineHyrule.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Entities
{
    /// <summary>
    /// Handles the player's movement.
    /// </summary>
    public class PlayerComponent : MonoBehaviour, IMovementComponent
    {
        /// <summary>
        /// The player's movement speed.
        /// </summary>
        [Header("Physics")]
        [SerializeField] float movementSpeed;
        /// <summary>
        /// The damping value applied to the movement axis.
        /// </summary>
        [SerializeField] float moveDamping;
        /// <summary>
        /// Multiplies the player's movement speed.
        /// </summary>

        /// <summary>
        /// The minimum amount of velocity that is considered movement for animation purposes.
        /// </summary>
        [Header("Animation")]
        [SerializeField] float moveAnimationThreshold;
        [SerializeField] Animator legsAnimator;

        /// <summary>
        /// The movement input action.
        /// </summary>
        [Header("Input")]
        [SerializeField] InputAction walkAction;

        /// <summary>
        /// The damped input axis.
        /// </summary>
        Vector2 WalkAxis { get; set; }
        /// <summary>
        /// The current mouse position in world space.
        /// </summary>
        Vector2 MousePosition => CameraMain.ScreenToWorldPoint(Mouse.current.position.ReadValue());
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
        bool IsMoving => Rigidbody.velocity.magnitude > moveAnimationThreshold;
        public float MovementAttackFactor { get; set; }
        float MovementItemFactor { get; set; }
        
        Rigidbody2D Rigidbody { get; set; }
        WeaponComponent WeaponComponent { get; set; }
        Camera CameraMain { get; set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            WeaponComponent = GetComponentInChildren<WeaponComponent>();
            if (!legsAnimator) legsAnimator = transform.Find("legs").GetComponent<Animator>();

            Locator.PlayerComponent = this;
            
            ResetBuffs();
        }

        void Start()
        {
            CameraMain = Camera.main;

            walkAction.Enable();
        }

        void Update()
        {
            ProcessInput();
            HandleAnimation();
            
            Rigidbody.rotation = LookAngle;
            if (WeaponComponent.HasRangedWeapon) WeaponComponent.SetWeaponRotation(WeaponAngle);
        }

        void FixedUpdate()
        {
            Rigidbody.velocity = WalkAxis * (movementSpeed * MovementAttackFactor * MovementItemFactor);
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
            legsAnimator.SetBool("isMoving", IsMoving);
        }

        public void Consume(MovementItemData movementItem)
        {
            MovementItemFactor = movementItem.movementFactor;
            Invoke(nameof(ResetBuffs), movementItem.duration);
        }

        void ResetBuffs() => (MovementAttackFactor, MovementItemFactor) = (1f, 1f);
    }
}
