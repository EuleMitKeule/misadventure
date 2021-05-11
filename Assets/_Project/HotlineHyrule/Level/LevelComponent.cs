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
        [Header("General")]
        [SerializeField] public Vector2Int playerRespawnPosition;
        /// <summary>
        /// Whether to enable the rain effect.
        /// </summary>
        [Header("Effects")]
        [SerializeField] bool isRaining;
        /// /// <summary>
        /// Whether to enable the snow effect.
        /// </summary>
        [SerializeField] bool isSnowing;
        /// <summary>
        /// The prefab of the rain effect.
        /// </summary>
        [SerializeField] GameObject rainEffectPrefab;
        /// <summary>
        /// The prefab of the snow effect.
        /// </summary>
        [SerializeField] GameObject snowEffectPrefab;

        public Grid Grid { get; private set; }
        
        void Awake()
        {
            Locator.LevelComponent = this;
            Grid = GetComponent<Grid>();

            var mainCamera = Camera.main;

            if (mainCamera)
            {
                if (isRaining) Instantiate(rainEffectPrefab, mainCamera.transform);
                if (isSnowing) Instantiate(snowEffectPrefab, mainCamera.transform);
            }
        }
    }
}