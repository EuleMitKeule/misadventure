using System;
using HotlineHyrule.Entities;
using UnityEngine;

namespace HotlineHyrule.Level
{
    public class QuestComponent : MonoBehaviour
    {
        QuestData QuestData => LevelComponent ? LevelComponent.levelData.questData : QuestData.Empty;

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

            if (QuestData is KillQuestData killQuestData)
            {
                if (KilledEnemies < killQuestData.killTarget) return;

                QuestCompleted?.Invoke(this, new QuestEventArgs(QuestData));
                Debug.Log("Completed Kill Quest!");
            }
        }
    }

    public class QuestEventArgs : EventArgs
    {
        public QuestData QuestData { get; }

        public QuestEventArgs(QuestData questData) => QuestData = questData;
    }
}