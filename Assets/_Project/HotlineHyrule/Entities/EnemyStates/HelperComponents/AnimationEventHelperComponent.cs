using System;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates.HelperComponents
{
    public class AnimationEventHelperComponent : MonoBehaviour
    {
        ParticleSystem _particleSystem;

        void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public void PlayParticleEffect()
        {
            _particleSystem.Play();
        }
    }
}