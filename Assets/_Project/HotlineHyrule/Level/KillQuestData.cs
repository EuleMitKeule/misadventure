using UnityEngine;

namespace HotlineHyrule.Level
{
    [CreateAssetMenu(menuName = "Quests/New Kill Quest")]
    public class KillQuestData : QuestData
    {
        [SerializeField] public int killTarget;
    }
}