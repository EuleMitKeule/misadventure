using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class DestroyQuestTarget : QuestTarget
    {
        [SerializeField]
        public string objectName;
        [SerializeField]
        public int count;
    }
}