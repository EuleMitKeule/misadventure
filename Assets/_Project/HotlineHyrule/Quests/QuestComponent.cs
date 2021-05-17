using System;
using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Entities;
using HotlineHyrule.Items;
using HotlineHyrule.Level;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Quests
{
    public class QuestComponent : MonoBehaviour
    {
        QuestData QuestData => LevelComponent ? LevelComponent.levelData.questData : QuestData.Empty;

        List<KillQuestTarget> KillQuestTargets => QuestData.questTargets.OfType<KillQuestTarget>().ToList();
        List<SearchQuestTarget> SearchQuestTargets => QuestData.questTargets.OfType<SearchQuestTarget>().ToList();
        List<TreasureQuestTarget> TreasureQuestTargets => QuestData.questTargets.OfType<TreasureQuestTarget>().ToList();

        public bool IsQuestFinished => QuestData.questTargets.Where(e => e.isRequired).All(IsCompleted);

        int KilledEnemies { get; set; }
        List<ItemData> FoundItems { get; set; } = new List<ItemData>();

        public event EventHandler<QuestEventArgs> QuestCompleted;
        public event EventHandler<QuestTargetEventArgs> QuestTargetReached;

        LevelComponent LevelComponent { get; set; }
        Tilemap TreasureTilemap { get; set; }

        void Awake()
        {
            Locator.QuestComponent = this;

            LevelComponent = GetComponent<LevelComponent>();
            var treasureTilemapObject = transform.Find("tilemap_treasure");
            if (treasureTilemapObject) TreasureTilemap = treasureTilemapObject.GetComponent<Tilemap>();

            EnemyComponent.EnemyKilled += OnEnemyKilled;
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void Start()
        {
            var itemPickupComponent = Locator.PlayerComponent.GetComponent<ItemPickupComponent>();
            itemPickupComponent.ItemConsumed += OnItemConsumed;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            foreach (var treasureQuestTarget in TreasureQuestTargets)
            {
                if (!treasureQuestTarget.treasurePrefab) continue;

                var treasureSpots = new List<Vector3Int>();
                foreach (var cellPosition in TreasureTilemap.cellBounds.allPositionsWithin)
                {
                    treasureSpots.Add(cellPosition);
                }
            }
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            EnemyComponent.EnemyKilled -= OnEnemyKilled;
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }

        void OnEnemyKilled(object sender, EventArgs e)
        {
            KilledEnemies += 1;

            var questTarget = KillQuestTargets.Find(target => KilledEnemies >= target.killTarget);

            if (questTarget != null)
            {
                QuestTargetReached?.Invoke(this, new QuestTargetEventArgs(questTarget));
            }
        }

        void OnItemConsumed(object sender, ItemEventArgs e)
        {
            if (SearchQuestTargets.All(target => target.item != e.ItemData)) return;

            var questTarget = SearchQuestTargets.Find(target => target.item == e.ItemData);
            QuestTargetReached?.Invoke(this, new QuestTargetEventArgs(questTarget));

            FoundItems.Add(e.ItemData);
        }

        bool IsCompleted(QuestTarget questTarget)
        {
            return false;
        }
    }
}