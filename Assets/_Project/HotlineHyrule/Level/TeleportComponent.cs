using UnityEngine;

namespace HotlineHyrule.Level
{
    public class TeleportComponent : MonoBehaviour
    {
        Collider2D Collider { get; set; }

        void Awake()
        {
            Collider = GetComponent<Collider2D>();
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.layer.IsPlayer()) return;
            if (!Locator.QuestComponent.IsQuestFinished) return;

            Collider.enabled = false;

            Locator.LevelComponent.FinishLevel();
        }
    }
}