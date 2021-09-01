using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Extensions;
using HotlineHyrule.Level;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace HotlineHyrule.Items
{
    public class FestoonComponent : MonoBehaviour
    {

        ItemComponent activateItem;
        Light2D light2d;
        [SerializeField] QuestItemData activateItemData;

        // Start is called before the first frame update
        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
            light2d = GetComponent<Light2D>();
        }

        private void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;            
        }

        private void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;

            var itemComponents = GameObject.FindObjectsOfType<ItemComponent>();

            var questItemComponents = itemComponents.Where(element => element.itemDatas.Contains(activateItemData)).ToList();

            activateItem = questItemComponents.OrderBy(element => element.transform.position.DistanceTo(transform.position)).First();

            if (!Locator.PlayerComponent) return;
            var pickupComponent = Locator.PlayerComponent.GetComponent<ItemPickupComponent>();

            pickupComponent.ItemConsumed += OnItemConsumed;
        }

        private void OnItemConsumed(object sender, ItemEventArgs e)
        {            
            if (!e.ItemComponent) return;                     
            if (e.ItemComponent != activateItem) return;
            light2d.enabled = true;            
        }


    }


}
