using System.Collections.Generic;
using HotlineHyrule.Level;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace HotlineHyrule.Items
{
    public class BombCoverComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        List<ItemData> ItemsToFind { get; set; }

        [OdinSerialize]
        Collider2D ItemCollider { get; set; }

        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;

            if (!ItemCollider) ItemCollider = GetComponent<Collider2D>();
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (!Locator.PlayerComponent) return;
            var itemPickupComponent = Locator.PlayerComponent.GetComponent<ItemPickupComponent>();
            if (!itemPickupComponent) return;
            itemPickupComponent.ItemConsumed += OnItemConsumed;
        }

        void OnItemConsumed(object sender, ItemEventArgs e)
        {
            if (!ItemsToFind.Contains(e.ItemData)) return;

            ItemsToFind.Remove(e.ItemData);

            if (ItemsToFind.Count == 0)
            {
                ItemCollider.enabled = true;
            }
        }
    }
}