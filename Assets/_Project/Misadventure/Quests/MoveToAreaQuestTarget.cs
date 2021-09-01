using System;
using UnityEngine;

namespace Misadventure.Quests
{
    [Serializable]
    public class MoveToAreaQuestTarget : QuestTarget
    {
        public Collider2D targetArea;
    }
}