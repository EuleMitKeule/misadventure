using System;
using System.Collections.Generic;
using System.Linq;
using Misadventure.Entities;
using Misadventure.Extensions;
using Misadventure.Items;
using Misadventure.Level;
using Misadventure.Weapons;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace Misadventure.Quests
{
    public class QuestComponent : MonoBehaviour
    {
        QuestData QuestData => LevelComponent ? LevelComponent.levelData.questData : QuestData.Empty;

        List<KillQuestTarget> KillQuestTargets => QuestData.questTargets.OfType<KillQuestTarget>().Where(target => !(target is KillSpecificQuestTarget)).ToList();
        List<KillSpecificQuestTarget> KillSpecificQuestTargets => QuestData.questTargets.OfType<KillSpecificQuestTarget>().ToList();
        List<SearchQuestTarget> SearchQuestTargets => QuestData.questTargets.OfType<SearchQuestTarget>().ToList();
        List<TreasureQuestTarget> TreasureQuestTargets => QuestData.questTargets.OfType<TreasureQuestTarget>().ToList();
        List<UseWeaponQuestTarget> UseWeaponQuestTargets => QuestData.questTargets.OfType<UseWeaponQuestTarget>().ToList();
        List<AreaQuestTarget> AreaQuestTargets => QuestData.questTargets.OfType<AreaQuestTarget>().ToList();
        List<DestroyQuestTarget> DestroyQuestTargets => QuestData.questTargets.OfType<DestroyQuestTarget>().ToList();

        public bool IsQuestFinished => QuestData.questTargets.Where(e => e.IsRequired).All(IsReached);
        public bool IsCompleted => QuestData.questTargets.All(e => ReachedTargets.Contains(e));

        public Dictionary<DestroyQuestTarget, int> QuestToDestroyedObjects { get; } =
            new Dictionary<DestroyQuestTarget, int>();
        public int TotalKilledEnemies { get; set; }
        public Dictionary<string, int> KilledEnemies { get; } = new Dictionary<string, int>();
        List<ItemData> FoundItems { get; } = new List<ItemData>();
        List<WeaponData> UsedWeapons { get; } = new List<WeaponData>();
        List<QuestTarget> ReachedTargets { get; } = new List<QuestTarget>();

        public event EventHandler<QuestTargetEventArgs> QuestTargetReached;
        public event EventHandler<KillQuestTargetEventArgs> KillQuestTargetChanged;
        public event EventHandler<KillQuestTargetEventArgs> SearchQuestTargetChanged;

        LevelComponent LevelComponent { get; set; }

        void Awake()
        {
            Locator.QuestComponent = this;

            QuestTargetReached += OnQuestTargetReached;

            EnemyComponent.EnemyKilled += OnEnemyKilled;
            CaterpillarComponent.CaterpillarKilled += OnEnemyKilled;
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
            
            LevelComponent.FinishLevel(QuestData.finishGameOnCompletion);
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

            foreach (var areaQuestTarget in AreaQuestTargets)
            {
                var colliderObject = GameObject.Find(areaQuestTarget.colliderName);
                if (!colliderObject) continue;

                var questAreaComponent = colliderObject.GetComponent<QuestAreaComponent>();
                if (!questAreaComponent) continue;

                questAreaComponent.PlayerEntered += OnPlayerEntered;
            }

            foreach (var destroyQuestTarget in DestroyQuestTargets) QuestToDestroyedObjects.Add(destroyQuestTarget, 0);

            var objectsToDestroy = FindObjectsOfType<QuestDestroyComponent>().ToList();

            foreach (var objectToDestroy in objectsToDestroy)
            {
                objectToDestroy.Destroyed += OnObjectDestroyed;
            }
        }

        void OnObjectDestroyed(object sender, EventArgs e)
        {
            if (sender is QuestDestroyComponent questDestroyComponent)
            {
                var questTarget = DestroyQuestTargets.Find(element =>
                    questDestroyComponent.name.StartsWith(element.objectName));

                if (questTarget == null) return;

                QuestToDestroyedObjects[questTarget] += 1;

                if (QuestToDestroyedObjects[questTarget] < questTarget.count) return;

                if (!ReachedTargets.Contains(questTarget))
                {
                    ReachedTargets.Add(questTarget);
                    QuestTargetReached?.Invoke(this, new QuestTargetEventArgs(questTarget));
                }
            }
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            EnemyComponent.EnemyKilled -= OnEnemyKilled;
            CaterpillarComponent.CaterpillarKilled -= OnEnemyKilled;
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }

        void OnLevelFinished(object sender, LevelFinishedEventArgs e)
        {
            if (IsCompleted &! e.FinishGame)
            {
                RewardComponent.Rewards = QuestData.questRewards;
            }
            else
            {
                RewardComponent.Rewards = new List<ConsumableItemData>();
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
            // if (SearchQuestTargets.All(target => target.item != e.ItemData) &&
            //     TreasureQuestTargets.All(target => target.treasureItem != e.ItemData)) return;
            
            FoundItems.Add(e.ItemData);

            var treasureQuestTarget = TreasureQuestTargets.Find(target => target.treasureItem == e.ItemData);

            var searchQuestsTargets = SearchQuestTargets.Where(element => element.Items.ContainsKey(e.ItemData)).ToList();

            foreach (var searchQuestTarget in searchQuestsTargets)
            {
                var foundItems = 0;

                foreach ( var pair in searchQuestTarget.Items)
                {
                    foundItems += FoundItems.Count(foundItem => foundItem == pair.Key);
                }
                SearchQuestTargetChanged?.Invoke(this, new KillQuestTargetEventArgs(searchQuestTarget, foundItems));
            }

            foreach (var searchQuestTarget in SearchQuestTargets)
            {
                if (searchQuestTarget.Items == null) continue;
                
                var isCompleted =
                    searchQuestTarget.Items.Keys.All(item => searchQuestTarget.Items[item] <= FoundItems.Count(foundItem => foundItem == item));
                if (!isCompleted) continue;
                
                if (ReachedTargets.Contains(searchQuestTarget)) continue;
            
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

        void OnPlayerEntered(object sender, EventArgs e)
        {
            var questAreaComponent = (QuestAreaComponent)sender;
            var areaQuestTarget = AreaQuestTargets.Find(t => t.colliderName == questAreaComponent.name);
            if (areaQuestTarget == null) return;

            if (ReachedTargets.Contains(areaQuestTarget)) return;
            
            ReachedTargets.Add(areaQuestTarget);
            QuestTargetReached?.Invoke(this, new QuestTargetEventArgs(areaQuestTarget));     
        }

        public bool IsReached(QuestTarget questTarget) => ReachedTargets.Contains(questTarget);
    }
}