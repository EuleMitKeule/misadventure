using System;
using System.Collections;
using System.Collections.Generic;
using HotlineHyrule.Entities;
using HotlineHyrule.Level;
using HotlineHyrule.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

namespace HotlineHyrule.Items
{
    public class TileEffectComponent : MonoBehaviour
    {
        [SerializeField] float tileDuration;
        [SerializeField] [MinMaxSlider(0, "tileDuration")] Vector2 tileDurationOffset;
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
            // Tilemap.tilemapTileChanged -= OnTileChanged;
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }

        private void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            // Tilemap.tilemapTileChanged += OnTileChanged;
        }

        // void OnTileChanged(Tilemap tilemap, Tilemap.SyncTile[] tiles)
        // {
        //     if (Tilemap != tilemap) return;
        //     foreach (var tile in tiles)
        //     {
        //         StartCoroutine(DestroyTile(tile.position));
        //     }
        // }

        public void SetTile(Vector3Int position, TileBase tile)
        {
            Tilemap.SetTile(position, tile);
            StartCoroutine(DestroyTile(position));
        }

        IEnumerator DestroyTile(Vector3Int pos)
        {
            var durationOffset = UnityEngine.Random.Range(tileDurationOffset.x, tileDurationOffset.y);
            var duration = tileDuration + (UnityEngine.Random.Range(0, 1) < 0.5f ? 1 : -1) * durationOffset;
            yield return new WaitForSeconds(duration);
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