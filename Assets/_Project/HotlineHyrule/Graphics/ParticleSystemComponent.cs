using UnityEngine;

namespace HotlineHyrule.Graphics
{
    public class ParticleSystemComponent : MonoBehaviour
    {
        ParticleSystem ParticleSystem { get; set; }

        void Awake()
        {
            ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        public void Play() => ParticleSystem.Play();

        public void Pause() => ParticleSystem.Pause();

        public void Stop() => ParticleSystem.Stop();
    }
}
