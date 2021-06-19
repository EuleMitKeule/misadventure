using System.Collections.Generic;
using HotlineHyrule.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Tiles
{
    [CreateAssetMenu(menuName = "2D/Tiles/Item Effect Tile")]
    public class ItemEffectTile : RuleTile
    {
        [SerializeField] public List<ConsumableItemData> itemEffects;

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject)
        {
            if (!instantiatedGameObject) return base.StartUp(position, tilemap, instantiatedGameObject);

            var tileEffectComponent = instantiatedGameObject.GetComponent<TileEffectComponent>();
            if (tileEffectComponent) tileEffectComponent.itemEffects = itemEffects;

            return base.StartUp(position, tilemap, instantiatedGameObject);
        }
    }
}