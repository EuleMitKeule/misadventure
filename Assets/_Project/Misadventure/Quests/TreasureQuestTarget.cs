using System;
using HotlineHyrule.Items;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Quests
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