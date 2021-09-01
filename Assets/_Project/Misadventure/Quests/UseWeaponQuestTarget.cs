using System;
using Misadventure.Weapons;
using UnityEngine;

namespace Misadventure.Quests
{
    [Serializable]
    public class UseWeaponQuestTarget : QuestTarget
    {
        [SerializeField] public WeaponData weapon;
    }
}