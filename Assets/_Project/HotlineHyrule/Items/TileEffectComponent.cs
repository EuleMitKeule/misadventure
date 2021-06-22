using System;
using System.Collections;
using System.Collections.Generic;
using HotlineHyrule.Entities;
using HotlineHyrule.Level;
using HotlineHyrule.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Items
{
    public class TileEffectComponent : MonoBehaviour
    {
        [SerializeField] float tileDuration;
        [SerializeField] public List<ConsumableItemData> itemEffects;
        Tilemap Tilemap { get; set; }

        void Awake()
        {
            Tilemap = GetComponent<Tilemap>();
           
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        private void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            Tilemap.tilemapTileChanged -= OnTileChanged;
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }

        private void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            Tilemap.tilemapTileChanged += OnTileChanged;
        }

        private void OnTileChanged(Tilemap tilemap, Tilemap.SyncTile[] tiles)
        {
            if (Tilemap != tilemap) return;
            foreach (var tile in tiles)
            {
                StartCoroutine(DestroyTile(tile.position));
            }            
        }

        IEnumerator DestroyTile(Vector3Int pos)
        {
            yield return new WaitForSeconds(tileDuration);
            Tilemap.SetTile(pos, null);
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