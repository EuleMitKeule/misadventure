using System.Collections.Generic;
using HotlineHyrule.Items;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    public class PlayerStateData : ScriptableObject
    {
        [SerializeField] public List<LoadoutSlot> loadoutSlots;
        [SerializeField] public LoadoutSlot currentLoadoutSlot;
        [SerializeField] public int currentHealth;

        public static PlayerStateData Empty => CreateInstance<PlayerStateData>();
    }
}