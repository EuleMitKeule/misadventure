using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Sound
{
    public class SoundComponent : MonoBehaviour
    {
        [SerializeField] AudioClip debugBGM1;
        [SerializeField] AudioClip debugBGM2;
        [SerializeField] InputAction debugFadeBGMAction;

        AudioSource _bgm1AudioSource;
        AudioSource _bgm2AudioSource;
        AudioSource _effectAudioSource;

        bool _bgm1Active;

        IEnumerator _fadeInCoroutine;
        IEnumerator _fadeOutCoroutine;

        void Awake()
        {
            Locator.SoundComponent = this;

            _bgm1AudioSource = gameObject.AddComponent<AudioSource>();
            _bgm2AudioSource = gameObject.AddComponent<AudioSource>();
            _effectAudioSource = gameObject.AddComponent<AudioSource>();

            _bgm1AudioSource.loop = true;
            _bgm2AudioSource.loop = true;

            debugFadeBGMAction.started += OnDebugAction;
            debugFadeBGMAction.Enable();
        }

        public void PlayBGM(AudioClip clip, float fadeDuration = 2)
        {
            if (_fadeOutCoroutine != null) StopCoroutine(_fadeOutCoroutine);
            if (_fadeInCoroutine != null) StopCoroutine(_fadeInCoroutine);

            if (_bgm1Active)
            {
                _bgm2AudioSource.clip = clip;

                _fadeOutCoroutine = FadeOutAudio(_bgm1AudioSource, fadeDuration);
                _fadeInCoroutine = FadeInAudio(_bgm2AudioSource, fadeDuration);

                StartCoroutine(_fadeInCoroutine);
                StartCoroutine(_fadeOutCoroutine);
            }
            else
            {
                _bgm1AudioSource.clip = clip;

                _fadeOutCoroutine = FadeOutAudio(_bgm2AudioSource, fadeDuration);
                _fadeInCoroutine = FadeInAudio(_bgm1AudioSource, fadeDuration);

                StartCoroutine(_fadeOutCoroutine);
                StartCoroutine(_fadeInCoroutine);
            }

            _bgm1Active = !_bgm1Active;
        }

        public void PlaySound(AudioClip clip)
        {
            _effectAudioSource.PlayOneShot(clip);
        }

        IEnumerator FadeInAudio(AudioSource audioSource, float duration)
        {
            var currentTime = 0f;
            var start = audioSource.volume;

            audioSource.Play();

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

        void OnDebugAction(InputAction.CallbackContext context)
        {
            PlayBGM(_bgm1Active ? debugBGM2 : debugBGM1);
        }
    }
}
