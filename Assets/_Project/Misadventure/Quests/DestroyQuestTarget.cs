using System;
using UnityEngine;

namespace Misadventure.Quests
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