using System;
using System.Linq;
using HotlineHyrule.Entities;
using HotlineHyrule.Level;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    public class QuestComponent : MonoBehaviour
    {
        QuestData QuestData => LevelComponent ? LevelComponent.levelData.questData : QuestData.Empty;

        public bool IsFinished => QuestData.questTargets.Where(e => e.isRequired).All(IsCompleted);

        int KilledEnemies { get; set; }

        public event EventHandler<QuestEventArgs> QuestCompleted;

        LevelComponent LevelComponent { get; set; }

        void Awake()
        {
            LevelComponent = GetComponent<LevelComponent>();

            EnemyComponent.EnemyKilled += OnEnemyKilled;
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
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
        }

        bool IsCompleted(QuestTarget questTarget)
        {
            return false;
        }
    }
}