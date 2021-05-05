using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    public class SoundManagerInitializationComponent : MonoBehaviour
    {
        [SerializeField] GameObject soundPlayerPrefab;
        [SerializeField] GameObject soundPlayerContainerObject;

        [SerializeField] List<SoundManager.SoundMapping> soundMappings;

        void Awake()
        {
            SoundManager.soundPlayerPrefab = soundPlayerPrefab;
            SoundManager.soundPlayerContainerObject = soundPlayerContainerObject;
            
            SoundManager.InitializeDictionary(soundMappings);
        }
    }
}
