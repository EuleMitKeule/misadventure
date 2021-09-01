using System;
using System.Collections.Generic;
using Misadventure.Items;
using Sirenix.Serialization;

namespace Misadventure.Quests
{
    [Serializable]
    public class SearchQuestTarget : QuestTarget
    {
        [OdinSerialize]
        public Dictionary<ItemData, int> Items { get; private set; } = new Dictionary<ItemData, int>();
    }
}