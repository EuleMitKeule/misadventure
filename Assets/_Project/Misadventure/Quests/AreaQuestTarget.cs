using System;
using UnityEngine;

namespace Misadventure.Quests
{
    [Serializable]
    public class AreaQuestTarget : QuestTarget
    {
        [SerializeField]
        public string colliderName;
    }
}