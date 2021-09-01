using Misadventure.Level;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Misadventure.UserInterface
{
    public class MenuInterfaceComponent : MonoBehaviour
    {
        CanvasGroup CanvasGroup { get; set; }

        [SerializeField] CanvasGroup mainGroup;
        [SerializeField] CanvasGroup optionsGroup;

        [SerializeField] AudioMixer audioMixer;

        [SerializeField] Slider masterSlider;
        [SerializeField] Slider bgmSlider;
        [SerializeField] Slider sfxSlider;

        [SerializeField] Toggle FullscreenToggle;

        void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();

            GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu)
            {
                OnSliderMaster();
                OnSliderBGM();
                OnSliderSFX();

                CanvasGroup.alpha = 1f;
                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;

                mainGroup.alpha = 1f;
                mainGroup.interactable = true;
                mainGroup.blocksRaycasts = true;

                return;
            }

            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

        public void OnButtonOptions()
        {
            mainGroup.alpha = 0f;
            mainGroup.interactable = false;
            mainGroup.blocksRaycasts = false;

            optionsGroup.alpha = 1f;
            optionsGroup.interactable = true;
            optionsGroup.blocksRaycasts = true;
        }
        public void OnButtonStart()
        {
            Locator.GameComponent.LoadNextScene();
        }

        public void OnButtonQuit()
        {
            Application.Quit();
        }

        public void OnButtonBack()
        {
            mainGroup.alpha = 1f;
            mainGroup.interactable = true;
            mainGroup.blocksRaycasts = true;

            optionsGroup.alpha = 0f;
            optionsGroup.interactable = false;
            optionsGroup.blocksRaycasts = false;
        }

        public void OnSliderMaster()
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterSlider.value) * 20 - 25);
        }

        public void OnSliderBGM()
        {
            audioMixer.SetFloat("BGMVolume", Mathf.Log10(bgmSlider.value) * 20 - 15);
        }

        public void OnSliderSFX()
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxSlider.value) * 20 - 20);
        }

        public void OnToggleFullscreen()
        {
            Screen.fullScreen = FullscreenToggle.isOn;
        }
    }
}