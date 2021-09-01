using System;
using System.Collections;
using System.Collections.Generic;
using Misadventure.Items;
using Misadventure.Level;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Misadventure.UserInterface
{
    public class ItemEffectInterfaceComponent : MonoBehaviour
    {
        /// <summary>
        /// Combined set of Consumable Item data and its fitting image
        /// </summary>
        [Serializable]
        public struct ItemDataImage
        {
            /// <summary>
            /// The Consumable Item data
            /// </summary>
            public ConsumableItemData data;
            /// <summary>
            /// The icon that shall be shown in the UI when an item with the corresponding data was consumed
            /// </summary>
            public Image image;
        }

        /// <summary>
        /// List of ItemData that shall be mapped on their fitting icon image
        /// </summary>
        [SerializeField] List<ItemDataImage> itemDataImages;

        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }
        
        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            if (!Locator.PlayerComponent) return;

            var itemPickupComponent = Locator.PlayerComponent.GetComponent<ItemPickupComponent>();
            itemPickupComponent.ItemConsumed += OnItemConsumed;
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            if (!Locator.PlayerComponent) return;
            
            var itemPickupComponent = Locator.PlayerComponent.GetComponent<ItemPickupComponent>();
            itemPickupComponent.ItemConsumed -= OnItemConsumed;
        }

        void OnItemConsumed(object sender, ItemEventArgs e)
        {
            foreach (var itemDataImage in itemDataImages)
            {
                
                if (itemDataImage.data == e.ItemData)
                {
                    
                    var imageObj = itemDataImage.image.gameObject;
                    var instance = Instantiate(imageObj, imageObj.transform.position, imageObj.transform.rotation);
                    instance.transform.SetParent(gameObject.transform, false);

                    if (itemDataImage.data.GetType() == typeof(AttackItemData))
                    {
                        var castedItemData =  itemDataImage.data as AttackItemData;
                        StartCoroutine(RemoveImageObject(instance, castedItemData.duration));
                    }
                    else if (itemDataImage.data.GetType() == typeof(MovementItemData))
                    {
                        var castedItemData =  itemDataImage.data as MovementItemData;
                        StartCoroutine(RemoveImageObject(instance, castedItemData.duration));
                    }
                    else if (itemDataImage.data.GetType() == typeof(HealthItemData))
                    {
                        var castedItemData = itemDataImage.data as HealthItemData;
                        if (castedItemData.HealRate == 0) return;
                        var cycles = (int)(castedItemData.HealTotal / castedItemData.HealAmount);
                        var duration = cycles * castedItemData.HealRate;
                        StartCoroutine(RemoveImageObject(instance, duration));
                    }
                }
            }
        }

        static IEnumerator RemoveImageObject(Object obj, float duration)
        {
            yield return new WaitForSeconds(duration);
            Destroy(obj);
        }
    }
}