using UnityEngine;
using UnityEngine.Tilemaps;

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
        [SerializeField] public Tilemap wallTilemap;
        [SerializeField] public Tilemap groundTilemap;

        void Awake()
        {
            Locator.LevelComponent = this;           

            if(!wallTilemap)
            {
                wallTilemap = GameObject.Find("tilemap_wall").GetComponent<Tilemap>();
            }
            if (!groundTilemap)
            {
                groundTilemap = GameObject.Find("tilemap_ground").GetComponent<Tilemap>();
            }

        }


        public bool IsWall(Vector3Int position) => wallTilemap.HasTile(position);
        public bool IsWall(Vector3 position) => wallTilemap.HasTile(wallTilemap.WorldToCell(position));

        public BoundsInt LevelBounds() => groundTilemap.cellBounds;
    }
}