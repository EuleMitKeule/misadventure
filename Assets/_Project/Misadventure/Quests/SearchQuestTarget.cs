using System;
using System.Collections.Generic;
using HotlineHyrule.Items;
using Sirenix.Serialization;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class SearchQuestTarget : QuestTarget
    {
        [OdinSerialize]
        public Dictionary<ItemData, int> Items { get; private set; } = new Dictionary<ItemData, int>();
    }
}