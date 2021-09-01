using Misadventure.Sound;
using UnityEngine;

namespace Misadventure.Menu
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
