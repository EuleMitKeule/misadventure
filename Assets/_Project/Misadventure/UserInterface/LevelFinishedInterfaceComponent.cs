using Misadventure.Level;
using TMPro;
using UnityEngine;

namespace Misadventure.UserInterface
{
    public class LevelFinishedInterfaceComponent : MonoBehaviour
    {
        [SerializeField] Transform questTargetReachedParent;
        [SerializeField] Transform rewardsParent;
        [SerializeField] GameObject questTargetReachedTextPrefab;

        Animator Animator { get; set; }

        void Awake()
        {
            if (!questTargetReachedParent) questTargetReachedParent = transform.Find("parent_quest_target_reached");
            if (!rewardsParent) rewardsParent = transform.Find("parent_rewards");

            Animator = GetComponent<Animator>();

            GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;

            Locator.LevelComponent.LevelFinished += OnLevelFinished;

            var labels = GetComponentsInChildren<TextMeshProUGUI>();
            labels[2].text = e.LevelData.areaFinishedText;
        }

        void OnLevelFinished(object sender, LevelFinishedEventArgs e)
        {
            if (!Locator.QuestComponent) return;
            if (e.FinishGame) return;

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

                if (targetText == "") continue;

                var labelObject = Instantiate(questTargetReachedTextPrefab, questTargetReachedParent);
                var label = labelObject.GetComponent<TextMeshProUGUI>();
                label.text = targetText;
            }

            if (Locator.QuestComponent.IsCompleted)
            {
                foreach (var reward in Locator.LevelComponent.levelData.questData.questRewards)
                {
                    var rewardsTextObject = Instantiate(questTargetReachedTextPrefab, rewardsParent);
                    var label = rewardsTextObject.GetComponent<TextMeshProUGUI>();
                    label.text = reward.InfoText;
                }
            }
        }
    }
}