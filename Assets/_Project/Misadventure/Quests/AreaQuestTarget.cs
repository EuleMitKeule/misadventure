using System;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class AreaQuestTarget : QuestTarget
    {
        [SerializeField]
        public string colliderName;
    }
}