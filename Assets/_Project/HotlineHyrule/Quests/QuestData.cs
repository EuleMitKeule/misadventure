using System.Collections.Generic;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [CreateAssetMenu(menuName = "Quest/New Quest")]
    public class QuestData : ScriptableObject
    {
        [TextArea] [SerializeField] public string questText;
        [SerializeReference] public List<QuestTarget> questTargets;

        public static QuestData Empty => CreateInstance<QuestData>();
    }
}