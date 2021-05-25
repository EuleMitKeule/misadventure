using System.Collections.Generic;
using HotlineHyrule.Level;
using HotlineHyrule.Quests;
using TMPro;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class QuestInterfaceComponent : MonoBehaviour
    {
        [SerializeField] GameObject questTargetPrefab;
        
        QuestData QuestData { get; set; }
        Dictionary<QuestTarget, GameObject> TargetToTargetObject { get; set; }

        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                Destroy(child.gameObject);
            }

            if (!e.LevelData) return;
            if (!e.LevelData.questData) return;
            if (!questTargetPrefab) return;

            QuestData = e.LevelData.questData;
            Locator.QuestComponent.QuestTargetReached += OnQuestTargetReached;
            Locator.QuestComponent.QuestTargetChanged += OnQuestTargetChanged;

            TargetToTargetObject = new Dictionary<QuestTarget, GameObject>();

            foreach (var questTarget in e.LevelData.questData.questTargets)
            {
                var questTargetObject = Instantiate(questTargetPrefab, transform);
                var label = questTargetObject.GetComponentInChildren<TextMeshProUGUI>();
                var parsedTargetText = questTarget.shortTargetText;

                if (questTarget is KillQuestTarget killQuestTarget &&
                    parsedTargetText.Contains("{x}"))
                {
                    var remainingKillTarget =
                        Mathf.Max(killQuestTarget.killTarget - Locator.QuestComponent.KilledEnemies, 0);
                    parsedTargetText = parsedTargetText.Replace("{x}", remainingKillTarget.ToString());
                }

                var questTargetText =
                    $"{(questTarget.isRequired ? "" : "(")}{parsedTargetText}{(questTarget.isRequired ? "" : ")")}";
                label.text = $"{questTargetText} ~";
                
                TargetToTargetObject.Add(questTarget, questTargetObject);
            }
        }

        void OnQuestTargetChanged(object sender, QuestTargetEventArgs e)
        {
            var targetObject = TargetToTargetObject[e.QuestTarget];
            var label = targetObject.GetComponentInChildren<TextMeshProUGUI>();
            var parsedTargetText = e.QuestTarget.shortTargetText;

            if (e.QuestTarget is KillQuestTarget killQuestTarget &&
                parsedTargetText.Contains("{x}"))
            {
                var remainingKillTarget =
                    Mathf.Max(killQuestTarget.killTarget - Locator.QuestComponent.KilledEnemies, 0);
                parsedTargetText = parsedTargetText.Replace("{x}", remainingKillTarget.ToString());
            }

            var questTargetText =
                $"{(e.QuestTarget.isRequired ? "" : "(")}{parsedTargetText}{(e.QuestTarget.isRequired ? "" : ")")}";
            label.text = $"{questTargetText} ~";
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;

            Locator.QuestComponent.QuestTargetReached -= OnQuestTargetReached;
        }

        void OnQuestTargetReached(object sender, QuestTargetEventArgs e)
        {
            if (!QuestData.questTargets.Contains(e.QuestTarget)) return;

            var questTargetObject = TargetToTargetObject[e.QuestTarget];
            var label = questTargetObject.GetComponentInChildren<TextMeshProUGUI>();
            label.fontStyle = FontStyles.Strikethrough;
        }
    }
}