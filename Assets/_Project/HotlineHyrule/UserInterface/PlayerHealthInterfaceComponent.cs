using System.Collections.Generic;
using HotlineHyrule.Entities;
using HotlineHyrule.Level;
using UnityEngine;
using UnityEngine.UI;

namespace HotlineHyrule.UserInterface
{
    public class PlayerHealthInterfaceComponent : MonoBehaviour
    {
        [SerializeField] GameObject healthContainerObject;
        [SerializeField] GameObject heartPrefab;
        public Sprite fullHeartSprite;
        public Sprite halfHeartSprite;

        int _health = 0;

        List<GameObject> _healthIcons = new List<GameObject>();

        void Awake()
        {
            Locator.GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            var healthComponent = Locator.PlayerComponent.GetComponent<HealthComponent>();
            SetHealthTo(healthComponent.Health);

            healthComponent.HealthChanged += OnPlayerHealthChanged;
        }

        void SetHealthTo(int amount)
        {
            if (_health < amount) AddHealth(amount - _health);
            else SubtractHealth(_health - amount);
        }

        void AddHealth(int amount)
        {
            if (amount <= 0) return;

            var remaining = amount;

            if (_health % 2 == 1)
            {
                _healthIcons[^1].GetComponent<Image>().sprite = fullHeartSprite;
                remaining -= 1;
            }

            for (var i = 0; i < (remaining + 1) / 2; ++i)
            {
                var heartObject = Instantiate(heartPrefab, healthContainerObject.transform);

                _healthIcons.Add(heartObject);
            }

            _health += amount;

            if (_health % 2 == 1)
            {
                var heartImage = _healthIcons[^1].GetComponent<Image>();
                heartImage.sprite = halfHeartSprite;
            }
        }

        void SubtractHealth(int amount)
        {
            if (amount <= 0) return;

            var cappedAmount = _health < amount ? _health : amount;
            var remaining = cappedAmount;

            if (_health % 2 == 1)
            {
                Destroy(_healthIcons[^1]);
                _healthIcons.RemoveAt(_healthIcons.Count - 1);
                remaining -= 1;
            }

            for (var i = 0; i < remaining / 2; ++i)
            {
                Destroy(_healthIcons[^1]);
                _healthIcons.RemoveAt(_healthIcons.Count - 1);
            }

            _health -= cappedAmount;

            if (_health % 2 == 1)
            {
                var heartImage = _healthIcons[^1].GetComponent<Image>();
                heartImage.sprite = halfHeartSprite;
            }
        }

        void OnPlayerHealthChanged(object sender, HealthEventArgs e)
        {
            SetHealthTo(e.NewHealth);
        }
    }
}