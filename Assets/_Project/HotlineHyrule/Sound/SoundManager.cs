using UnityEngine;
using System.Collections.Generic;

namespace HotlineHyrule
{
    public enum Sound
    {
        // Enemies
        EnemyHit,
        Explosion,

        // Items
        Diamond1Pickup,
        Diamond5Pickup,
        Diamond10Pickup,
        Diamond20Pickup,
        HeartPickup,
        SaveFountain,

        // Player
        ArrowHitWall,
        Bow,
        ChangeArmor,
        GroundFall,
        HeartBeat,
        Hit1,
        Hit2,
        SwordSwing,

        // UI
        ButtonHover,
        ButtonSelect,
        ButtonTitle,
        CreateSaveCharacterSwitch
    }

    /// <summary>
    /// Provides access to sound related services.
    /// </summary>
    public static class SoundManager
    {
        public static GameObject soundPlayerPrefab;
        public static GameObject soundPlayerContainerObject;

        private static Dictionary<Sound, AudioClip> dictionary = new Dictionary<Sound, AudioClip>();

        [System.Serializable]
        public class SoundMapping : System.Object
        {
            public Sound name;
            public AudioClip clip;
        }

        public static void InitializeDictionary(List<SoundMapping> list)
        {
            foreach (var item in list)
                dictionary[item.name] = item.clip;
        }

        public static void PlaySound(Sound sound, Vector3 position)
        {
            GameObject soundPlayer = GameObject.Instantiate(soundPlayerPrefab, soundPlayerContainerObject.transform);
            soundPlayer.transform.position = position;

            AudioSource audioSource = soundPlayer.GetComponent<AudioSource>();
            audioSource.clip = dictionary[sound];
            audioSource.Play();

            SoundPlayerComponent soundPlayerComponent = soundPlayer.GetComponent<SoundPlayerComponent>();
            soundPlayerComponent.StartCoroutine(soundPlayerComponent.DestroyWhenFinished(audioSource));
        }
    }
}
