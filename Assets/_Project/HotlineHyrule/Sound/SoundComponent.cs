using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

namespace HotlineHyrule.Sound
{
    public class SoundComponent : MonoBehaviour
    {
        [SerializeField] AudioMixerGroup bgmAudioMixerGroup;
        [SerializeField] AudioMixerGroup sfxAudioMixerGroup;

        [SerializeField] BGMData debugData1;
        [SerializeField] BGMData debugData2;
        [SerializeField] InputAction debugFadeBGMAction;

        AudioSource _bgm1AudioSource;
        AudioSource _bgm2AudioSource;
        AudioSource _sfxAudioSource;

        bool _bgm1Active;

        IEnumerator _fadeInCoroutine;
        IEnumerator _fadeOutCoroutine;

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

            debugFadeBGMAction.started += OnDebugAction;
            debugFadeBGMAction.Enable();
        }

        public void PlayBGM(BGMData data, float fadeDuration = 1)
        {
            if (_fadeOutCoroutine != null) StopCoroutine(_fadeOutCoroutine);
            if (_fadeInCoroutine != null) StopCoroutine(_fadeInCoroutine);

            if (_bgm1Active)
            {
                _fadeOutCoroutine = FadeOutAudio(_bgm1AudioSource, fadeDuration);
                _fadeInCoroutine = FadeInAudio(_bgm2AudioSource, data, fadeDuration);

                StartCoroutine(_fadeInCoroutine);
                StartCoroutine(_fadeOutCoroutine);
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

        public void PlaySound(AudioClip clip)
        {
            if (!clip) return;
            
            _sfxAudioSource.PlayOneShot(clip);
        }

        IEnumerator FadeInAudio(AudioSource audioSource, BGMData data, float duration)
        {
            var currentTime = 0f;
            //var start = audioSource.volume;
            var start = 0f;

            if (data == null) yield break;
            
            audioSource.clip = data.loopAudioClip;
            audioSource.PlayOneShot(data.introAudioClip);
            audioSource.PlayScheduled(AudioSettings.dspTime + data.introAudioClip.length);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, 1, currentTime / duration);
                yield break;
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
                yield break;
            }

            audioSource.Stop();
        }

        void OnDebugAction(InputAction.CallbackContext context)
        {
            PlayBGM(_bgm1Active ? debugData2 : debugData1);
        }
    }
}
