using System;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class MoveToAreaQuestTarget : QuestTarget
    {
        public Collider2D targetArea;
    }
}