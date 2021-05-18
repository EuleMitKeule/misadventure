using System;
using HotlineHyrule.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class TreasureQuestTarget : QuestTarget
    {
        [SerializeField] public ItemData treasureItem;
        [SerializeField] public string treasureTilemapName;
    }
}