using System.Collections.Generic;
using HotlineHyrule.Extensions;
using HotlineHyrule.Items;
using HotlineHyrule.Level;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Quests
{
    public class RewardComponent : MonoBehaviour
    {
        [SerializeField] public Tilemap rewardTilemap;
        [SerializeField] public TileBase chestTile;

        public static List<ItemData> Rewards { get; set; }

        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            if (Rewards == null) return;
            if (Rewards.Count == 0) return;

            var i = 0;
            foreach (var cellPosition in rewardTilemap.cellBounds.allPositionsWithin)
            {
                if (!rewardTilemap.HasTile(cellPosition)) continue;

                if (chestTile) rewardTilemap.SetTile(cellPosition, chestTile);
                Instantiate(Rewards[i].ItemPrefab, cellPosition.ToWorld(), Quaternion.identity);

                i += 1;
                if (i == 2) break;
            }
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }
    }
}