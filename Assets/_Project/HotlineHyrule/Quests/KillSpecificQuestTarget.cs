using System;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class KillSpecificQuestTarget : KillQuestTarget
    {
        [SerializeField] public string enemyName;
    }
}