using System;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class TreasureQuestTarget : QuestTarget
    {
        [SerializeField] public GameObject treasurePrefab;
    }
}