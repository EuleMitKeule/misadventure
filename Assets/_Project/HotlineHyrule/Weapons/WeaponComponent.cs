using System.Collections;
using HotlineHyrule.Entities;
using HotlineHyrule.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Weapons
{
    /// <summary>
    /// Handles the behavior of the weapon it's attached to.
    /// </summary>
    public class  WeaponComponent : MonoBehaviour
    {
        /// <summary>
        /// The weapon data corresponding to this weapon.
        /// </summary>
        [SerializeField] public WeaponData weaponData;
        /// <summary>
        /// The attack input action.
        /// </summary>
        [SerializeField] InputAction attackAction;

        /// <summary>
        /// The last point in time the weapon was used at.
        /// </summary>
        float LastAttackTime { get; set; }
        /// <summary>
        /// Whether the weapon component's parent is the player.
        /// </summary>
        bool IsPlayer => GetComponentInParent<PlayerComponent>();
        /// <summary>
        /// Whether the attack input is currently being registered.
        /// </summary>
        bool IsAttacking => attackAction.ReadValue<float>() != 0f;
        /// <summary>
        /// Whether enough time has passed since the last usage for the weapon to be used again.
        /// </summary>
        bool CanAttack => Time.time >= LastAttackTime + 1 / weaponData.attackRate;
        /// <summary>
        /// Whether the current weapon is a ranged one.
        /// </summary>
        public bool HasRangedWeapon => weaponData is RangedWeaponData;
        /// <summary>
        /// Whether the current weapon is a melee one.
        /// </summary>
        public bool HasMeleeWeapon => weaponData is MeleeWeaponData;
        /// <summary>
        /// The ranged weapon data of the ranged weapon.
        /// </summary>
        public RangedWeaponData RangedWeaponData => (RangedWeaponData)weaponData;

        /// <summary>
        /// The offset of the projectile spawn position relative to the weapon position.
        /// </summary>
        MeleeWeaponData MeleeWeaponData => (MeleeWeaponData)weaponData;
        Vector3 ProjectileSpawnOffset =>
            Transform.right * RangedWeaponData.spawnPosition.x +
            Transform.up * RangedWeaponData.spawnPosition.y;
        /// <summary>
        /// The spawn position of the projectile.
        /// </summary>
        Vector3 ProjectileSpawnPosition => ProjectileSpawnOffset + Transform.position;
        Coroutine MeleeAttackCoroutine { get; set; }

        Transform Transform { get; set; }
        SpriteRenderer SpriteRenderer { get; set; }

        void Awake()
        {
            Transform = transform;
            SpriteRenderer = GetComponent<SpriteRenderer>();

            attackAction.Enable();
        }

        void Update()
        {
            if (IsAttacking) PerformAttack();
        }

        /// <summary>
        /// Sets the current weapon to the given one.
        /// </summary>
        /// <param name="newWeaponData"></param>
        public void SetWeapon(WeaponData newWeaponData)
        {
            weaponData = newWeaponData;
            SpriteRenderer.sprite = newWeaponData.weaponSprite;
        }

        /// <summary>
        /// Performs an attack if possible.
        /// </summary>
        void PerformAttack()
        {
            if (!CanAttack) return;
            LastAttackTime = Time.time;

            if (HasRangedWeapon) PerformRangedAttack();
            else if (HasMeleeWeapon) PerformMeleeAttack();
        }

        /// <summary>
        /// Performs a ranged attack with the equipped ranged weapon.
        /// </summary>
        void PerformRangedAttack()
        {
            if (!HasRangedWeapon) return;
            
            var projectileObject = Instantiate(RangedWeaponData.projectilePrefab, ProjectileSpawnPosition, Transform.rotation);

            projectileObject.SetActive(false);

            var projectileComponent = projectileObject.GetComponent<ProjectileComponent>();
            projectileComponent.impactMask = new LayerMask();
            projectileComponent.impactMask.value |= 1 << PhysicsLayer.WALL;
            projectileComponent.impactMask.value |= 1 << (IsPlayer ? PhysicsLayer.ENEMY : PhysicsLayer.PLAYER);

            projectileObject.SetActive(true);

            var projectileRigidbody = projectileObject.GetComponent<Rigidbody2D>();
            projectileRigidbody.velocity = Transform.up * RangedWeaponData.projectileSpeed;
        }

        void PerformMeleeAttack()
        {
            if (!HasMeleeWeapon) return;
            
            StartMeleeAttackRoutine();
        }

        void StartMeleeAttackRoutine() => MeleeAttackCoroutine ??= StartCoroutine(MeleeAttackRoutine());

        void StopMeleeAttackRoutine()
        {
            StopCoroutine(MeleeAttackCoroutine);
            MeleeAttackCoroutine = null;
        }

        IEnumerator MeleeAttackRoutine()
        {
            foreach (var meleeAttackArea in MeleeWeaponData.attackAreas)
            {
                var layerMask = 1 << (IsPlayer ? PhysicsLayer.ENEMY : PhysicsLayer.PLAYER);
                var colliders = Physics.OverlapConeAll((Vector2) transform.position + meleeAttackArea.offset, meleeAttackArea.radius,
                    Transform.up, meleeAttackArea.startAngle, meleeAttackArea.stopAngle, Transform.eulerAngles.z, layerMask);
                
                Debug.DrawLine((Vector2)transform.position + meleeAttackArea.offset, transform.position + Transform.up.RotateAroundZ(meleeAttackArea.startAngle) * meleeAttackArea.radius, Color.green, 1f);
                Debug.DrawLine((Vector2)transform.position + meleeAttackArea.offset, transform.position + Transform.up.RotateAroundZ(meleeAttackArea.stopAngle) * meleeAttackArea.radius, Color.red, 1f);
                if (colliders[0]) Debug.DrawLine((Vector2)transform.position + meleeAttackArea.offset, colliders[0].bounds.center, Color.yellow, 1f);
                
                yield return new WaitForSeconds(1 / MeleeWeaponData.areaAttackRate);
            }
            
            StopMeleeAttackRoutine();
        }
    }
}
