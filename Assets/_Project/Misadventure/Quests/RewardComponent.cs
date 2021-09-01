using System.Collections.Generic;
using Misadventure.Items;
using Misadventure.Level;
using UnityEngine;

namespace Misadventure.Quests
{
    public class RewardComponent : MonoBehaviour
    {
        public static List<ConsumableItemData> Rewards { get; set; }

        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            if (Rewards == null) return;
            if (Rewards.Count == 0) return;
            if (!Locator.PlayerComponent) return;

            var itemPickupComponent = Locator.PlayerComponent.GetComponent<ItemPickupComponent>();
            if (!itemPickupComponent) return;

            foreach (var reward in Rewards)
            {
                itemPickupComponent.ConsumeItem(reward);
            }
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }
    }
}