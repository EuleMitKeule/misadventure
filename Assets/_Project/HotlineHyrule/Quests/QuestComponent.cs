using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using HotlineHyrule.Entities;
using HotlineHyrule.Extensions;
using HotlineHyrule.Items;
using HotlineHyrule.Level;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace HotlineHyrule.Quests
{
    public class QuestComponent : MonoBehaviour
    {
        // [HideInInspector] public List<TreasureQuestSettings> treasureQuestSettings; 
        
        QuestData QuestData => LevelComponent ? LevelComponent.levelData.questData : QuestData.Empty;

        List<KillQuestTarget> KillQuestTargets => QuestData.questTargets.OfType<KillQuestTarget>().ToList();
        List<SearchQuestTarget> SearchQuestTargets => QuestData.questTargets.OfType<SearchQuestTarget>().ToList();
        List<TreasureQuestTarget> TreasureQuestTargets => QuestData.questTargets.OfType<TreasureQuestTarget>().ToList();

        public bool IsQuestFinished => QuestData.questTargets.Where(e => e.isRequired).All(IsCompleted);

        public int KilledEnemies { get; set; }
        List<ItemData> FoundItems { get; } = new List<ItemData>();
        List<QuestTarget> ReachedTargets { get; } = new List<QuestTarget>();

        public event EventHandler<QuestTargetEventArgs> QuestTargetReached;
        public event EventHandler<QuestTargetEventArgs> QuestTargetChanged;

        LevelComponent LevelComponent { get; set; }

        void Awake()
        {
            Locator.QuestComponent = this;

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
            if (e.IsMenu) return;
            if (!e.LevelData) return;
            if (!e.LevelData.questData) return;

            LevelComponent = GetComponent<LevelComponent>();

            foreach (var treasureQuestTarget in TreasureQuestTargets)
            {
                if (!treasureQuestTarget.treasureItem) continue;
                if (!treasureQuestTarget.treasureItem.itemPrefab) continue;

                var treasureTilemapObject = transform.Find(treasureQuestTarget.treasureTilemapName);
                if (!treasureTilemapObject)
                {
                    Debug.LogWarning($"{treasureQuestTarget.treasureTilemapName} could not be found.");
                    continue;
                }

                var treasureTilemap = treasureTilemapObject.GetComponent<Tilemap>(); 
                var treasureSpots = new List<Vector3Int>();
                
                foreach (var cellPosition in treasureTilemap.cellBounds.allPositionsWithin)
                {
                    if (!treasureTilemap.HasTile(cellPosition)) continue;
                    treasureSpots.Add(cellPosition);
                }

                var randomIndex = new Random().Next(treasureSpots.Count);
                var treasureSpot = treasureSpots.ElementAt(randomIndex);
                Instantiate(treasureQuestTarget.treasureItem.itemPrefab, treasureSpot.ToWorld(), Quaternion.identity);
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

            foreach (var killQuestTarget in KillQuestTargets)
            {
                QuestTargetChanged?.Invoke(this, new QuestTargetEventArgs(killQuestTarget));
            }

            var questTarget = KillQuestTargets.Find(target => KilledEnemies >= target.killTarget);

            if (ReachedTargets.Contains(questTarget)) return;

            if (questTarget != null)
            {
                ReachedTargets.Add(questTarget);
                QuestTargetReached?.Invoke(this, new QuestTargetEventArgs(questTarget));
            }
        }

        void OnItemConsumed(object sender, ItemEventArgs e)
        {
            if (SearchQuestTargets.All(target => target.item != e.ItemData) &&
                TreasureQuestTargets.All(target => target.treasureItem != e.ItemData)) return;
            
            FoundItems.Add(e.ItemData);

            var searchQuestTarget = SearchQuestTargets.Find(target => target.item == e.ItemData);
            var treasureQuestTarget = TreasureQuestTargets.Find(target => target.treasureItem == e.ItemData);

            if (searchQuestTarget != null)
            {
                if (ReachedTargets.Contains(searchQuestTarget)) return;
            
                ReachedTargets.Add(searchQuestTarget);
                QuestTargetReached?.Invoke(this, new QuestTargetEventArgs(searchQuestTarget));       
            }

            if (treasureQuestTarget != null)
            {
                if (ReachedTargets.Contains(treasureQuestTarget)) return;
            
                ReachedTargets.Add(treasureQuestTarget);
                QuestTargetReached?.Invoke(this, new QuestTargetEventArgs(treasureQuestTarget));       
            }
        }

        bool IsCompleted(QuestTarget questTarget) => ReachedTargets.Contains(questTarget);
    }

    [Serializable]
    public class TreasureQuestSettings
    {
        [SerializeField] public TreasureQuestTarget treasureQuestTarget;
        [SerializeField] public Tilemap treasureTilemap;
    }
}