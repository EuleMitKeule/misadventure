using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Extensions;
using HotlineHyrule.Level;
using UnityEngine;
namespace HotlineHyrule.Items
{
    public class FestoonComponent : MonoBehaviour
    {

        ItemComponent activateItem;
        Animator animator;
        [SerializeField] QuestItemData activateItemData;

        // Start is called before the first frame update
        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
            animator = GetComponent<Animator>();
            Debug.Log("sdaasdasd");
        }
        private void Update()
        {
            Debug.Log("asdasdas");
        }

        private void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;            
        }

        private void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            Debug.Log("pooasd");
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
            Debug.Log(e.ItemData);
            if (!e.ItemComponent) return;
            Debug.Log(e.ItemComponent.name);
            
            if (e.ItemComponent != activateItem) return;
            animator.SetTrigger("light");
        }


    }


}
