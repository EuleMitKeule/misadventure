using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule
{
    public class SoundComponent : MonoBehaviour
    {

        AudioSource bgm1AudioSource;
        AudioSource bgm2AudioSource;
        AudioSource effectAudioSource;

        bool bgm1Active = false;

        IEnumerator fadeInCoroutine;
        IEnumerator fadeOutCoroutine;

        [SerializeField] InputAction debugFadeBGMAction;
        [SerializeField] AudioClip debugBGM1;
        [SerializeField] AudioClip debugBGM2;

        void Awake()
        {
            Locator.SoundComponent = this;

            bgm1AudioSource = gameObject.AddComponent<AudioSource>();
            bgm2AudioSource = gameObject.AddComponent<AudioSource>();
            effectAudioSource = gameObject.AddComponent<AudioSource>();

            bgm1AudioSource.loop = true;
            bgm2AudioSource.loop = true;

            debugFadeBGMAction.started += OnDebugAction;
            debugFadeBGMAction.Enable();
        }

        public void PlayBGM(AudioClip clip, float fadeDuration = 2)
        {
            if (fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);
            if (fadeInCoroutine != null)
                StopCoroutine(fadeInCoroutine);

            if (bgm1Active)
            {
                bgm2AudioSource.clip = clip;

                fadeOutCoroutine = FadeOutAudio(bgm1AudioSource, fadeDuration);
                fadeInCoroutine = FadeInAudio(bgm2AudioSource, fadeDuration);

                StartCoroutine(fadeInCoroutine);
                StartCoroutine(fadeOutCoroutine);
            }
            else
            {
                bgm1AudioSource.clip = clip;

                fadeOutCoroutine = FadeOutAudio(bgm2AudioSource, fadeDuration);
                fadeInCoroutine = FadeInAudio(bgm1AudioSource, fadeDuration);

                StartCoroutine(fadeOutCoroutine);
                StartCoroutine(fadeInCoroutine);
            }

            bgm1Active = !bgm1Active;
        }

        public void PlaySound(AudioClip clip)
        {
            effectAudioSource.PlayOneShot(clip);
        }

        IEnumerator FadeInAudio(AudioSource audioSource, float duration)
        {
            float currentTime = 0;
            float start = audioSource.volume;

            audioSource.Play();

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, 1, currentTime / duration);
                yield return null;
            }

            yield break;
        }

        IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
        {
            float currentTime = 0;
            float start = audioSource.volume;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, 0, currentTime / duration);
                yield return null;
            }

            audioSource.Stop();
            yield break;
        }

        void OnDebugAction(InputAction.CallbackContext context)
        {
            if (bgm1Active)
                PlayBGM(debugBGM2);
            else
                PlayBGM(debugBGM1);
        }
    }
}
