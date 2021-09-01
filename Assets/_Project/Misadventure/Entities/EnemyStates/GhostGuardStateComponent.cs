using System;
using System.Numerics;
using Misadventure.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Misadventure.Entities.EnemyStates
{
    public class GhostGuardStateComponent : EnemyBaseStateComponent
    {
        [SerializeField] float moveSpeed;
        [SerializeField] float radius;

        float Circumference => 2 * Mathf.PI * radius;
        float RotationTime => Circumference / moveSpeed;
        [ShowInInspector]
        Vector3 GuardPosition { get; set; }
        Complex PatrolComplex => radius * Complex.Pow(Math.E,
            Complex.ImaginaryOne * Time.time * 2 * Mathf.PI / RotationTime);
        Vector3 PatrolPoint => PatrolComplex.ToVector3() + GuardPosition;
        Vector3 PatrolDirection => transform.position.DirectionTo(PatrolPoint);
        protected override void Awake()
        {
            base.Awake();

            GuardPosition = transform.position;
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            
            if (EnemyComponent.IsPlayerAttackable)
            {
                SetState<EnemyAttackStateComponent>();
            }

            if (EnemyComponent.IsPlayerFollowable)
            {
                SetState<EnemyFollowStateComponent>();
            }

            EnemyComponent.SetVelocity(PatrolDirection * moveSpeed);
        }

        public override void OnHealthChanged(object sender, HealthEventArgs e)
        {
            base.OnHealthChanged(sender, e);

            SetState<EnemyFollowStateComponent>();
        }
    }
}