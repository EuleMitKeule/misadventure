using System;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    public class UseWeaponQuestTarget : QuestTarget
    {
        [SerializeField] public WeaponData weapon;
    }
}