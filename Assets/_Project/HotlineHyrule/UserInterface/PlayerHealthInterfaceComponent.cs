using System.Collections.Generic;
using HotlineHyrule.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace HotlineHyrule.UserInterface
{
    public class PlayerHealthInterfaceComponent : MonoBehaviour
    {
        [SerializeField] GameObject healthContainerObject;
        public Sprite fullHeartSprite;
        public Sprite halfHeartSprite;

        int _health = 0;

        List<GameObject> _healthIcons = new List<GameObject>();

        void Start()
        {
            var healthComponent = Locator.PlayerComponent.GetComponent<HealthComponent>();
            SetHealthTo(healthComponent.Health);

            healthComponent.HealthChanged += OnPlayerHealthChanged;
        }

        void SetHealthTo(int amount)
        {
            if (_health < amount)
                AddHealth(amount - _health);
            else
                SubtractHealth(_health - amount);
        }

        void AddHealth(int amount)
        {
            if (amount <= 0)
                return;

            var remaining = amount;

            if (_health % 2 == 1)
            {
                _healthIcons[^1].GetComponent<Image>().sprite = fullHeartSprite;
                remaining -= 1;
            }

            for (int i = 0; i < (remaining + 1) / 2; ++i)
            {
                GameObject icon = new GameObject();
                icon.transform.SetParent(healthContainerObject.transform);

                RectTransform rectTransform = icon.AddComponent<RectTransform>();
                Image image = icon.AddComponent<Image>();

                rectTransform.localPosition = new Vector3(-900 + _healthIcons.Count % 12 * 31, 400 - _healthIcons.Count / 12 * 31, 0);
                rectTransform.sizeDelta = new Vector2(32, 32);

                image.sprite = fullHeartSprite;

                _healthIcons.Add(icon);
            }

            _health += amount;

            if (_health % 2 == 1)
                _healthIcons[^1].GetComponent<Image>().sprite = halfHeartSprite;
        }

        void SubtractHealth(int amount)
        {
            if (amount <= 0)
                return;

            int cappedAmount = _health < amount ? _health : amount;
            int remaining = cappedAmount;

            if (_health % 2 == 1)
            {
                Destroy(_healthIcons[^1]);
                _healthIcons.RemoveAt(_healthIcons.Count - 1);
                --remaining;
            }

            for (int i = 0; i < remaining / 2; ++i)
            {
                Destroy(_healthIcons[^1]);
                _healthIcons.RemoveAt(_healthIcons.Count - 1);
            }

            _health -= cappedAmount;

            if (_health % 2 == 1)
                _healthIcons[^1].GetComponent<Image>().sprite = halfHeartSprite;

        }

        void OnPlayerHealthChanged(object sender, HealthEventArgs e)
        {
            SetHealthTo(e.NewHealth);
        }
    }
}