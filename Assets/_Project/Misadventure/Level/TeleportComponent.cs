using UnityEngine;

namespace HotlineHyrule.Level
{
    public class TeleportComponent : MonoBehaviour
    {
        /// <summary>
        /// If true, level finish will only be triggered if all required quests are done
        /// If false, they will be ignored (useful when only required quest is to reach the finish)
        /// </summary>
        [SerializeField] bool otherFinishedQuestsRequired = true;
        Collider2D Collider { get; set; }

        void Awake()
        {
            Collider = GetComponent<Collider2D>();
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.layer.IsPlayer()) return;
            if (!Locator.QuestComponent.IsQuestFinished && otherFinishedQuestsRequired) return;

            Collider.enabled = false;

            Locator.LevelComponent.FinishLevel();
        }
    }
}