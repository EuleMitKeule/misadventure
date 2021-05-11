using UnityEngine;

namespace HotlineHyrule.Graphics
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemAnimationComponent : MonoBehaviour
    {
        ParticleSystem ParticleSystem { get; set; }

        void Awake()
        {
            ParticleSystem = GetComponent<ParticleSystem>();
        }

        public void Play() => ParticleSystem.Play();

        public void Pause() => ParticleSystem.Pause();

        public void Stop() => ParticleSystem.Stop();
    }
}
