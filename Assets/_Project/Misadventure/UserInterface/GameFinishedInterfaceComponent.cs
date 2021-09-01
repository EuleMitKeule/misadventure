using Misadventure.Level;
using TMPro;
using UnityEngine;

namespace Misadventure.UserInterface
{
    public class GameFinishedInterfaceComponent : MonoBehaviour
    {
        [SerializeField] Transform questTargetReachedParent;
        [SerializeField] GameObject questTargetReachedTextPrefab;
        
        TextMeshProUGUI TimeLabel { get; set; }

        void Awake()
        {
            if (!questTargetReachedParent) questTargetReachedParent = transform.Find("parent_quest_target_reached");

            GameComponent.LevelLoaded += OnLevelLoaded;
            
            var timeLabelObject = transform.Find("label_time");
            if (!timeLabelObject) return;

            TimeLabel = timeLabelObject.GetComponent<TextMeshProUGUI>();
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            
            Locator.LevelComponent.LevelFinished += OnLevelFinished;
        }

        void OnLevelFinished(object sender, LevelFinishedEventArgs e)
        {
            if (!e.FinishGame) return;
            
            var elapsedTime = (int)Locator.GameComponent.ElapsedTime;
            var elapsedSeconds = elapsedTime % 60;
            var elapsedMinutes = elapsedTime / 60 % 60;
            var elapsedHours = elapsedTime / 60 / 60;

            TimeLabel.text =
                $"{elapsedHours:D2}:{elapsedMinutes:D2}:{elapsedSeconds:D2}";
            
            for (var i = 0; i < questTargetReachedParent.childCount; i++)
            {
                var child = questTargetReachedParent.GetChild(i);
                Destroy(child.gameObject);
            }

            foreach (var questTarget in Locator.LevelComponent.levelData.questData.questTargets)
            {
                var targetText = Locator.QuestComponent.IsReached(questTarget)
                    ? questTarget.targetReachedText
                    : questTarget.targetNotReachedText;

                var labelObject = Instantiate(questTargetReachedTextPrefab, questTargetReachedParent);
                var label = labelObject.GetComponent<TextMeshProUGUI>();
                label.text = targetText;
            }
        }
    }
}