using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Misadventure.Sound
{
    public class SoundComponent : MonoBehaviour
    {
        [SerializeField] AudioMixerGroup bgmAudioMixerGroup;
        [SerializeField] AudioMixerGroup sfxAudioMixerGroup;

        AudioSource _bgm1AudioSource;
        AudioSource _bgm2AudioSource;
        AudioSource _sfxAudioSource;

        bool _bgm1Active;

        IEnumerator _fadeInCoroutine;
        IEnumerator _fadeOutCoroutine;

        public void PlaySound(AudioClip clip)
        {
            if (!clip) return;
            
            _sfxAudioSource.PlayOneShot(clip);
        }
        
        void Awake()
        {
            Locator.SoundComponent = this;

            _bgm1AudioSource = gameObject.AddComponent<AudioSource>();
            _bgm2AudioSource = gameObject.AddComponent<AudioSource>();
            _sfxAudioSource = gameObject.AddComponent<AudioSource>();

            _bgm1AudioSource.outputAudioMixerGroup = bgmAudioMixerGroup;
            _bgm2AudioSource.outputAudioMixerGroup = bgmAudioMixerGroup;
            _sfxAudioSource.outputAudioMixerGroup = sfxAudioMixerGroup;

            _bgm1AudioSource.loop = true;
            _bgm2AudioSource.loop = true;

            _bgm1Active = false;
        }

        public void PlayBGM(BGMData data, float fadeDuration = 1)
        {
            if (_fadeOutCoroutine != null) StopCoroutine(_fadeOutCoroutine);
            if (_fadeInCoroutine != null) StopCoroutine(_fadeInCoroutine);

            if (_bgm1Active)
            {
                _fadeOutCoroutine = FadeOutAudio(_bgm1AudioSource, fadeDuration);
                _fadeInCoroutine = FadeInAudio(_bgm2AudioSource, data, fadeDuration);

                StartCoroutine(_fadeOutCoroutine);
                StartCoroutine(_fadeInCoroutine);
            }
            else
            {
                _fadeOutCoroutine = FadeOutAudio(_bgm2AudioSource, fadeDuration);
                _fadeInCoroutine = FadeInAudio(_bgm1AudioSource, data, fadeDuration);

                StartCoroutine(_fadeOutCoroutine);
                StartCoroutine(_fadeInCoroutine);
            }

            _bgm1Active = !_bgm1Active;
        }

        IEnumerator FadeInAudio(AudioSource audioSource, BGMData data, float duration)
        {
            var currentTime = 0f;
            var start = 0f;
            
            audioSource.clip = data.loopAudioClip;
            audioSource.PlayOneShot(data.introAudioClip);
            audioSource.PlayScheduled(AudioSettings.dspTime + data.introAudioClip.length);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, 1, currentTime / duration);
                yield return null;
            }
        }

        IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
        {
            var currentTime = 0f;
            var start = audioSource.volume;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, 0, currentTime / duration);
                yield return null;
            }

            audioSource.Stop();
        }
    }
}
