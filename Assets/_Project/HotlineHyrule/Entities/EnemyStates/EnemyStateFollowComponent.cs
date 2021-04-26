using HotlineHyrule.Entities.EnemyStates.HelperComponents;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateFollowComponent : EnemyStateBaseComponent
    {
        /// <summary>
        /// Duration for the enemy following the player
        /// </summary>
        [SerializeField] float followDuration;
        /// <summary>
        /// Move speed for enemy while following the player
        /// </summary>
        [SerializeField] float moveSpeed;
        /// <summary>
        /// Collider component on that the following is based on.
        /// As long as the player does not leave this collider, the
        /// enemy will follow them. 
        /// </summary>
        [SerializeField] Collider2D followRangeCollider;
        
        float _timer;

        public override void Setup()
        {
            base.Setup();
            EnemyStateFollowColliderHelperComponent.PlayerStayFollowRange += OnPlayerStayFollowRange;
            _timer = 0f;
        }

        public override void FixedStateUpdate()
        {
            base.FixedStateUpdate();

            if (_timer > followDuration)
            {
                EnemyComponent.ChangeState(EnemyComponent.PatrolState);
                return;
            }
            _timer += Time.deltaTime;
            
            var dir = transform.position - Player.transform.position;
            var angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Rigidbody.velocity = new Vector2(transform.up.x, transform.up.y) * moveSpeed * Time.deltaTime;
        }

        void OnPlayerStayFollowRange(object sender, Collider2D col)
        {
            if (col != followRangeCollider) return;
            _timer = 0f;
        }

        public override void OnLookingAtPlayer()
        {
            base.OnLookingAtPlayer();
            EnemyComponent.ChangeState(EnemyComponent.ShootProjectileState);
        }

        public override void Exit()
        {
            base.Exit();
            _timer = 0f;
            EnemyStateFollowColliderHelperComponent.PlayerStayFollowRange -= OnPlayerStayFollowRange;
        }
    }
}