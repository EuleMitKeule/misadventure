using System.Collections.Generic;
using HotlineHyrule.Entities;
using HotlineHyrule.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Items
{
    public class TileEffectComponent : MonoBehaviour
    {
        [SerializeField] public List<ConsumableItemData> itemEffects;
        Tilemap Tilemap { get; set; }

        void Awake()
        {
            Tilemap = GetComponent<Tilemap>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.layer.IsPlayer()) return;

            var itemPickupComponent = other.GetComponent<ItemPickupComponent>();

            if (!itemPickupComponent) return;

            foreach (var itemEffect in itemEffects)
            {
                itemPickupComponent.ConsumeItem(itemEffect);
            }
        }
    }
}