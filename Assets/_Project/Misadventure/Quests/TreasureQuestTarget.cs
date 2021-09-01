using System;
using Misadventure.Items;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Misadventure.Quests
{
    [Serializable]
    public class TreasureQuestTarget : QuestTarget
    {
        [BoxGroup("Specific")]
        [InlineEditor]
        [ShowInInspector] public ItemData treasureItem;
        [BoxGroup("Specific")]
        [ShowInInspector]
        [OdinSerialize]
        public string TreasureTilemapName { get; set; }
    }
}