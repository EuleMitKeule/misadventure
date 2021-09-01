using System;
using System.Collections.Generic;
using Misadventure.Entities;
using Misadventure.Level;
using UnityEngine;
using UnityEngine.UI;

namespace Misadventure.UserInterface
{
    public class PlayerHealthInterfaceComponent : MonoBehaviour
    {
        [SerializeField] float scalingFactor = 0.1f;
        [SerializeField] GameObject healthContainerObject;
        [SerializeField] GameObject heartPrefab;
        public Sprite fullHeartSprite;
        public Sprite halfHeartSprite;

        int MaxHealth { get; set; }
        int CurrentHealth { get; set; }

        List<GameObject> HealthIcons { get; } = new List<GameObject>();

        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            if (!Locator.PlayerComponent) return;

            var healthComponent = Locator.PlayerComponent.GetComponent<HealthComponent>();
            MaxHealth = healthComponent.maxHealth;
            SetHealthTo(healthComponent.Health);

            healthComponent.HealthChanged += OnPlayerHealthChanged;
        }

        void SetHealthTo(int amount)
        {
            if (MaxHealth == 0) return;

            var wantedHeartCount = Mathf.RoundToInt(amount * scalingFactor);

            var realHeartCount = wantedHeartCount / 2f;
            var roundedHeartCount = (int)Math.Round(realHeartCount, 0, MidpointRounding.AwayFromZero);
            var remainder = realHeartCount - (int) realHeartCount;

            var isLastHalf = remainder >= 0.5f;

            foreach (var icon in HealthIcons)
            {
                Destroy(icon);
            }

            for (var i = 0; i < roundedHeartCount; i++)
            {
                var heartObject = Instantiate(heartPrefab, healthContainerObject.transform);
                HealthIcons.Add(heartObject);
            }

            if (isLastHalf && HealthIcons.Count != 0)
            {
                var heartImage = HealthIcons[HealthIcons.Count - 1].GetComponent<Image>();
                heartImage.sprite = halfHeartSprite;
            }
        }

        void OnPlayerHealthChanged(object sender, HealthEventArgs e)
        {
            SetHealthTo(e.NewHealth);
        }
    }
}