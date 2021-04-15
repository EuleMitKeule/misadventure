using UnityEngine;

namespace HotlineHyrule.Level
{
    public class LevelComponent : MonoBehaviour
    {
        [SerializeField] public Vector2Int playerRespawnPosition;
        
        void Awake()
        {
            Locator.LevelComponent = this;
        }
    }
}