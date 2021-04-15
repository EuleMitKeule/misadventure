using UnityEngine;

namespace HotlineHyrule.Level
{
    /// <summary>
    /// Contains level information for the level (grid) it's attached to.
    /// </summary>
    public class LevelComponent : MonoBehaviour
    {
        /// <summary>
        /// The cell position the player respawns at.
        /// </summary>
        [SerializeField] public Vector2Int playerRespawnPosition;
        
        void Awake()
        {
            Locator.LevelComponent = this;
        }
    }
}