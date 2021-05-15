using System;
using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Entities;
using HotlineHyrule.Items;
using HotlineHyrule.Level;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    public class QuestComponent : MonoBehaviour
    {
        QuestData QuestData => LevelComponent ? LevelComponent.levelData.questData : QuestData.Empty;

        List<KillQuestTarget> KillQuestTargets => QuestData.questTargets.OfType<KillQuestTarget>().ToList();
        List<SearchQuestTarget> SearchQuestTargets => QuestData.questTargets.OfType<SearchQuestTarget>().ToList();

        public bool IsQuestFinished => QuestData.questTargets.Where(e => e.isRequired).All(IsCompleted);

        int KilledEnemies { get; set; }
        List<ItemData> FoundItems { get; set; } = new List<ItemData>();

        public event EventHandler<QuestEventArgs> QuestCompleted;
        public event EventHandler<QuestTargetEventArgs> QuestTargetReached;

        LevelComponent LevelComponent { get; set; }

        void Awake()
        {
            Locator.QuestComponent = this;

            LevelComponent = GetComponent<LevelComponent>();

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