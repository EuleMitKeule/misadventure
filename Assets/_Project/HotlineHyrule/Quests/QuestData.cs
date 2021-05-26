using System.Collections.Generic;
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

        public static QuestData Empty => CreateInstance<QuestData>();
    }
}