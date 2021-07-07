using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using HotlineHyrule.Entities;
using HotlineHyrule.Extensions;
using HotlineHyrule.Items;
using HotlineHyrule.Level;
using HotlineHyrule.Weapons;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace HotlineHyrule.Quests
{
    public class QuestComponent : MonoBehaviour
    {
        QuestData QuestData => LevelComponent ? LevelComponent.levelData.questData : QuestData.Empty;

        List<KillQuestTarget> KillQuestTargets => QuestData.questTargets.OfType<KillQuestTarget>().Where(target => !(target is KillSpecificQuestTarget)).ToList();
        List<KillSpecificQuestTarget> KillSpecificQuestTargets => QuestData.questTargets.OfType<KillSpecificQuestTarget>().ToList();
        List<SearchQuestTarget> SearchQuestTargets => QuestData.questTargets.OfType<SearchQuestTarget>().ToList();
        List<TreasureQuestTarget> TreasureQuestTargets => QuestData.questTargets.OfType<TreasureQuestTarget>().ToList();
        List<UseWeaponQuestTarget> UseWeaponQuestTargets => QuestData.questTargets.OfType<UseWeaponQuestTarget>().ToList();

        public bool IsQuestFinished => QuestData.questTargets.Where(e => e.IsRequired).All(IsReached);
        public bool IsCompleted => QuestData.questTargets.All(e => ReachedTargets.Contains(e));

        public int TotalKilledEnemies { get; set; }
        public Dictionary<string, int> KilledEnemies { get; } = new Dictionary<string, int>();
        List<ItemData> FoundItems { get; } = new List<ItemData>();
        List<WeaponData> UsedWeapons { get; } = new List<WeaponData>();
        List<QuestTarget> ReachedTargets { get; } = new List<QuestTarget>();

        public event EventHandler<QuestTargetEventArgs> QuestTargetReached;
        public event EventHandler<KillQuestTargetEventArgs> KillQuestTargetChanged;

        LevelComponent LevelComponent { get; set; }

        void Awake()
        {
            Locator.QuestComponent = this;

            QuestTargetReached += OnQuestTargetReached;

            EnemyComponent.EnemyKilled += OnEnemyKilled;
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void Start()
        {
            if (Locator.PlayerComponent)
            {
                var itemPickupComponent = Locator.PlayerComponent.GetComponent<ItemPickupComponent>();
                itemPickupComponent.ItemConsumed += OnItemConsumed;
                var weaponComponent = Locator.PlayerComponent.GetComponent<WeaponComponent>();
                weaponComponent.AttackStarted += OnAttackStarted;
            }

            Locator.LevelComponent.LevelFinished += OnLevelFinished;
        }

        void OnQuestTargetReached(object sender, QuestTargetEventArgs e)
        {
            if (!IsQuestFinished) return;
            if (!QuestData.finishLevelOnCompletion) return;

            LevelComponent.FinishLevel();
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
                if (!treasureQuestTarget.treasureItem.ItemPrefab) continue;

                var treasureTilemapObject = transform.Find(treasureQuestTarget.TreasureTilemapName);
                if (!treasureTilemapObject)
                {
                    Logging.LogWarning($"{treasureQuestTarget.TreasureTilemapName} could not be found.");
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
                Instantiate(treasureQuestTarget.treasureItem.ItemPrefab, treasureSpot.ToWorld(), Quaternion.identity);
            }
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            EnemyComponent.EnemyKilled -= OnEnemyKilled;
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }

        void OnLevelFinished(object sender, EventArgs e)
        {
            if (IsCompleted)
            {
                var items = QuestData.questRewards.OrderBy(x => Guid.NewGuid()).ToList();
                var rewards = items.Take(QuestData.questRewardCount).ToList();

                RewardComponent.Rewards = rewards;
            }
            else
            {
                RewardComponent.Rewards = new List<ItemData>();
            }
        }

        void OnEnemyKilled(object sender, EntityEventArgs e)
        {
            TotalKilledEnemies += 1;

            var killSpecificQuestTarget =
                KillSpecificQuestTargets.Find(target => e.EntityObject.name.Contains(target.enemyName));

            if (killSpecificQuestTarget != null)
            {
                if (!KilledEnemies.ContainsKey(killSpecificQuestTarget.enemyName))
                {
                    KilledEnemies.Add(killSpecificQuestTarget.enemyName, 0);
                }

                KilledEnemies[killSpecificQuestTarget.enemyName] += 1;
                var killCount = KilledEnemies[killSpecificQuestTarget.enemyName];
                KillQuestTargetChanged?.Invoke(this, new KillQuestTargetEventArgs(killSpecificQuestTarget, killCount));

                if (!IsReached(killSpecificQuestTarget) &&
                    KilledEnemies[killSpecificQuestTarget.enemyName] >= killSpecificQuestTarget.killTarget)
                {
                    ReachedTargets.Add(killSpecificQuestTarget);
                    QuestTargetReached?.Invoke(this, new QuestTargetEventArgs(killSpecificQuestTarget));
                }
            }

            foreach (var target in KillQuestTargets)
            {
                KillQuestTargetChanged?.Invoke(this, new KillQuestTargetEventArgs(target, TotalKilledEnemies));
            }

            var questTarget = KillQuestTargets.Find(target => TotalKilledEnemies >= target.killTarget);

            if (!ReachedTargets.Contains(questTarget) && questTarget != null)
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

        void OnAttackStarted(object sender, WeaponEventArgs e)
        {
            if (UsedWeapons.Contains(e.Weapon)) return;

            UsedWeapons.Add(e.Weapon);

            var useWeaponQuestTarget = UseWeaponQuestTargets.Find(target => target.weapon == e.Weapon);

            if (useWeaponQuestTarget == null) return;
            if (IsReached(useWeaponQuestTarget)) return;

            ReachedTargets.Add(useWeaponQuestTarget);
            QuestTargetReached?.Invoke(this, new QuestTargetEventArgs(useWeaponQuestTarget));
        }

        public bool IsReached(QuestTarget questTarget) => ReachedTargets.Contains(questTarget);
    }
}