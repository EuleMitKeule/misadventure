using System;
using HotlineHyrule.Items;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class SearchQuestTarget : QuestTarget
    {
        [SerializeField] public ItemData item;
    }
}