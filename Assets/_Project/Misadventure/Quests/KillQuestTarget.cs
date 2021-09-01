using System;
using UnityEngine;

namespace Misadventure.Quests
{
    [Serializable]
    public class KillQuestTarget : QuestTarget
    {
        [SerializeField] public int killTarget;
    }
}