using System;
using UnityEngine;

namespace HotlineHyrule.Weapons
{
    [Serializable]
    public class MeleeAttackArea
    {
        [SerializeField] public Vector2 offset;
        [SerializeField] public float radius;
        [SerializeField] public float startAngle;
        [SerializeField] public float stopAngle;
        [SerializeField] public float damage;
    }
}