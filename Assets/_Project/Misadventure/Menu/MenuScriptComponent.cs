using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotlineHyrule.Menu
{
    public class MenuScriptComponent : MonoBehaviour
    {
        [SerializeField] BGMData bgmData;
        
        void Start()
        {
            Locator.SoundComponent.PlayBGM(bgmData);
        }
    }
}
