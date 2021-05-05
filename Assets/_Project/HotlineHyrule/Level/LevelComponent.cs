using UnityEngine;

namespace HotlineHyrule.Level
{
    /// <summary>
    /// Contains level information for the level (grid) it's attached to.
    /// </summary>
    [RequireComponent(typeof(Grid))]
    public class LevelComponent : MonoBehaviour
    {
        /// <summary>
        /// The cell position the player respawns at.
        /// </summary>
        [SerializeField] public Vector2Int playerRespawnPosition;
        
        public Grid Grid { get; private set; }
        
        void Awake()
        {
            Locator.LevelComponent = this;
            Grid = GetComponent<Grid>();
        }
    }
}