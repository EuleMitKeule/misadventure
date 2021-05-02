using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotlineHyrule.Entities
{
    public class UIComponent : MonoBehaviour
    {
        [SerializeField] GameObject healthContainerObject;

        public Sprite fullHeartSprite;
        public Sprite halfHeartSprite;

        int health = 0;

        List<GameObject> healthIconList;

        void Awake()
        {
            healthIconList = new List<GameObject>();
        }

        void Start()
        {
            HealthComponent healthComponent = Locator.PlayerComponent.gameObject.GetComponent<HealthComponent>();
            SetHealthTo(healthComponent.Health);

            healthComponent.HealthChanged += OnPlayerHealthChanged;
        }

        void SetHealthTo(int amount)
        {
            if (health < amount)
                AddHealth(amount - health);
            else
                SubtractHealth(health - amount);
        }

        void AddHealth(int amount)
        {
            if (amount <= 0)
                return;

            int remaining = amount;

            if (health % 2 == 1)
            {
                healthIconList[healthIconList.Count - 1].GetComponent<Image>().sprite = fullHeartSprite;
                --remaining;
            }

            for (int i = 0; i < (remaining + 1) / 2; ++i)
            {
                GameObject icon = new GameObject();
                icon.transform.SetParent(healthContainerObject.transform);

                RectTransform rectTransform = icon.AddComponent<RectTransform>();
                Image image = icon.AddComponent<Image>();

                rectTransform.localPosition = new Vector3(-900 + healthIconList.Count % 12 * 31, 400 - healthIconList.Count / 12 * 31, 0);
                rectTransform.sizeDelta = new Vector2(32, 32);

                image.sprite = fullHeartSprite;

                healthIconList.Add(icon);
            }

            health += amount;

            if (health % 2 == 1)
                healthIconList[healthIconList.Count - 1].GetComponent<Image>().sprite = halfHeartSprite;
        }

        void SubtractHealth(int amount)
        {
            if (amount <= 0)
                return;

            int cappedAmount = health < amount ? health : amount;
            int remaining = cappedAmount;

            if (health % 2 == 1)
            {
                Destroy(healthIconList[healthIconList.Count - 1]);
                healthIconList.RemoveAt(healthIconList.Count - 1);
                --remaining;
            }

            for (int i = 0; i < remaining / 2; ++i)
            {
                Destroy(healthIconList[healthIconList.Count - 1]);
                healthIconList.RemoveAt(healthIconList.Count - 1);
            }

            health -= cappedAmount;

            if (health % 2 == 1)
                healthIconList[healthIconList.Count - 1].GetComponent<Image>().sprite = halfHeartSprite;

        }

        void OnPlayerHealthChanged(object sender, HealthEventArgs e)
        {
            SetHealthTo(e.NewHealth);
        }
    }
}