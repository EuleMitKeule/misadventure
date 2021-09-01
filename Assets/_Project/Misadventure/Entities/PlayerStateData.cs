using System.Collections.Generic;
using Misadventure.Items;
using UnityEngine;

namespace Misadventure.Entities
{
    public class PlayerStateData : ScriptableObject
    {
        [SerializeField] public List<LoadoutSlot> loadoutSlots;
        [SerializeField] public LoadoutSlot currentLoadoutSlot;
        [SerializeField] public int currentHealth;

        public static PlayerStateData Empty => CreateInstance<PlayerStateData>();
    }
}