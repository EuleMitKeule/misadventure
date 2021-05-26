using System.Collections.Generic;
using HotlineHyrule.Items;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [CreateAssetMenu(menuName = "Quest/New Quest")]
    public class QuestData : ScriptableObject
    {
        [TextArea] [SerializeField] public string questText;
        [TextArea] [SerializeField] public string questTargetText;
        [SerializeReference] public List<QuestTarget> questTargets;
        [SerializeField] public bool finishLevelOnCompletion;
        [SerializeField] public List<ItemData> questRewards;
        [SerializeField] public int questRewardCount;

        public static QuestData Empty => CreateInstance<QuestData>();
    }
}