using Misadventure.Level;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Misadventure.Items
{
    public class ItemEnablerComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        ItemData TriggerItem { get; set; }
        Collider2D ItemCollider { get; set; }
        
        void Awake()
        {
            ItemCollider = GetComponent<Collider2D>();
            
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            
            var playerComponent = Locator.PlayerComponent;
            if (!playerComponent) return;

            var itemPickupComponent = playerComponent.GetComponent<ItemPickupComponent>();
            if (!itemPickupComponent) return;

            itemPickupComponent.ItemConsumed += OnItemConsumed;
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }

        void OnItemConsumed(object sender, ItemEventArgs e)
        {
            if (e.ItemData != TriggerItem) return;

            ItemCollider.enabled = true;
        }
    }
}