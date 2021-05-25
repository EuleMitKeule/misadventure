using System;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class QuestTarget
    {
        [SerializeField] public bool isRequired;
        [TextArea] [SerializeField] public string targetText;
        [TextArea] [SerializeField] public string shortTargetText;
    }
}