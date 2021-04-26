using System;
using System.Collections;
using HotlineHyrule.Entities.EnemyStates.HelperComponents;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateFollowComponent : EnemyStateBaseComponent
    {
        [SerializeField] float followDuration;
        [SerializeField] float moveSpeed;
        float _timer;

        public override void Setup()
        {
            base.Setup();
            EnemyStateFollowColliderHelperComponent.PlayerLeftFollowRange += OnPlayerLeftFollowRange;
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

        void OnPlayerLeftFollowRange(object sender, EventArgs e)
        {
            EnemyComponent.ChangeState(EnemyComponent.PatrolState);
        }
        
        void OnPlayerEntersSightRange(object sender, EventArgs e)
        {
            EnemyComponent.ChangeState(EnemyComponent.ShootProjectileState);
        }

        public override void OnLookingAtPlayer()
        {
            base.OnLookingAtPlayer();
            EnemyComponent.ChangeState(EnemyComponent.ShootProjectileState);
        }

        public override void Exit()
        {
            base.Exit();
            EnemyStateFollowColliderHelperComponent.PlayerLeftFollowRange -= OnPlayerLeftFollowRange;
        }
    }
}