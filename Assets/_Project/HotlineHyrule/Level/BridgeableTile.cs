using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Level
{
    [CreateAssetMenu(menuName = "2D/Tiles/Bridgeable Tile")]
    public class BridgeableTile : RuleTile
    {
        [SerializeField] public string bridgeTilemapName;
        [SerializeField] public List<Sprite> bridgeSprites;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);

            var bridgeTilemapObject = GameObject.Find(bridgeTilemapName);

            if (bridgeTilemapObject)
            {
                var bridgeTilemap = bridgeTilemapObject.GetComponent<Tilemap>();

                if (bridgeTilemap != null)
                {
                    var bridgeSprite = bridgeTilemap.GetSprite(position);

                    tileData.colliderType =
                        bridgeSprites.Contains(bridgeSprite) ? Tile.ColliderType.None : tileData.colliderType;
                }
            }
        }
    }
}