using HotlineHyrule.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Tiles
{
    [CreateAssetMenu(menuName = "2D/Tiles/Prefab Tile")]
    public class PrefabTile : TileBase
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Sprite sprite;

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            if (go) go.transform.position = position.ToWorld();
            
            return base.StartUp(position, tilemap, go);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            if (prefab) tileData.gameObject = prefab;
            tileData.sprite = sprite;
        }
    }
}