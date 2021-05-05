using System;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates.HelperComponents
{
    public class AnimationEventHelperComponent : MonoBehaviour
    {
        [SerializeField] ParticleSystem particleSystem;

        public void InitParticleObject()
        {
            Instantiate(particleSystem.gameObject, transform.position, Quaternion.identity);
        }

        public void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}