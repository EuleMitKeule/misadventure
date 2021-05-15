using System.Collections.Generic;
using HotlineHyrule.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Level
{
    public class TilemapObjectMapperComponent : MonoBehaviour
    {
        [SerializeField] List<TileObject> tileObjectMap;
        
        Tilemap[] Tilemaps { get; set; }

        [ContextMenu("Generate Mapped Objects")]
        public void GenerateMappedObjects()
        {
            Tilemaps = GetComponentsInChildren<Tilemap>();
            
            foreach (var tileObject in tileObjectMap)
            {
                foreach (var tilemap in Tilemaps)
                {
                    foreach (var cellPosition in tilemap.cellBounds.allPositionsWithin)
                    {
                        var tile = tilemap.GetTile(cellPosition);
                        if (tileObject.tile != tile) continue;

                        var newObject = Instantiate(tileObject.prefab, tilemap.transform);
                        newObject.transform.position = cellPosition.ToWorld();
                    }
                }
            }
        }
    }
}