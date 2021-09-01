using System;
using UnityEngine;

namespace Misadventure.Quests
{
    [Serializable]
    public class KillSpecificQuestTarget : KillQuestTarget
    {
        [SerializeField] public string enemyName;
    }
}