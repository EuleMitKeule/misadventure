using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Pathfinding
{
    public class NavComponent : MonoBehaviour
    {
        /// <summary>
        /// Tilemaps that entities can walk on.
        /// </summary>
        [SerializeField] public List<Tilemap> walkableTilemaps;
        /// <summary>
        /// Tilemaps that entities can not walk on.
        /// </summary>
        [SerializeField] public List<Tilemap> unwalkableTilemaps;

        /// <summary>
        /// Cell positions that entities can walk on.
        /// </summary>
        public List<Vector3Int> NavMap { get; set; }
        bool IsGenerated { get; set; } = false;

        void Awake()
        {
            Locator.NavComponent = this;
            GenerateNavMap();
            
            Pathfinder.InitializeNavMap(NavMap);
        }

        void Update()
        {
            if (!IsGenerated)
            {
                GenerateNavMap();
            
                Pathfinder.InitializeNavMap(NavMap);

                IsGenerated = true;
            }
        }

        /// <summary>
        /// Generates a nav map containing walkable cell positions.
        /// </summary>
        [ContextMenu("Generate Nav Map")]
        void GenerateNavMap()
        {
            var cells = new List<Vector3Int>();

            //Iterate all walkable not ignored child tilemaps and add each unique cell to the set
            foreach (var tilemap in walkableTilemaps)
            {
                foreach (var cell in tilemap.cellBounds.allPositionsWithin)
                {
                    if (!tilemap.HasTile(cell)) continue;
                    if (cells.Contains(cell)) continue;

                    cells.Add(cell);
                }
            }

            //Iterate all not walkable not ignored child tilemaps and remove each cell from the set
            foreach (var tilemap in unwalkableTilemaps)
            {
                foreach (var cell in tilemap.cellBounds.allPositionsWithin)
                {
                    if (!tilemap.HasTile(cell)) continue;
                    if (!cells.Contains(cell)) continue;

                    var hasTileCollider = tilemap.GetColliderType(cell) != Tile.ColliderType.None;
                    
                    var boxcastResults = new RaycastHit2D[10];
                    Physics2D.BoxCastNonAlloc(cell.ToWorld(), Vector2.one * 0.9f, 0f, Vector2.zero, boxcastResults);
                    
                    var tilePrefab = tilemap.GetInstantiatedObject(cell);
                    var prefabCollider = tilePrefab ? tilePrefab.GetComponent<Collider2D>() : null;
                    var hasPrefabCollider = prefabCollider && !prefabCollider.isTrigger;
                    
                    var hasCollider = boxcastResults.Where(e => e.collider != null).Any(e => e.collider.gameObject == tilemap.gameObject);
                    
                    if (!hasTileCollider && !hasCollider && !hasPrefabCollider) continue;

                    cells.Remove(cell);
                }
            }

            NavMap = cells;
        }
    }
}
