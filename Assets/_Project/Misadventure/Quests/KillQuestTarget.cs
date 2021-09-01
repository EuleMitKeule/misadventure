using System;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class KillQuestTarget : QuestTarget
    {
        [SerializeField] public int killTarget;
    }
}