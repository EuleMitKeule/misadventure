using HotlineHyrule.Entities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace HotlineHyrulePlayTests.Entities
{
    public class HealthComponentPlayTests : InputTestFixture
    {
        [Test]
        public void ShouldSetStartHealth()
        {
            var healthComponent = GetDummy(maxHealth: 50);

            var health = healthComponent.Health;

            Assert.AreEqual(50, health);
        }

        [Test]
        public void ShouldClampStartHealth()
        {
            var healthComponent = GetDummy(75, 100);

            var health = healthComponent.Health;

            Assert.AreEqual(75, health);
        }

        [Test]
        public void ShouldResetHealthOnRespawn()
        {
            var keyboard = InputSystem.AddDevice<Keyboard>();
            var healthComponent = GetDummy(500, 500, true);

            healthComponent.Health -= 500; 
            PressAndRelease(keyboard.rKey);
            
            var health = healthComponent.Health;
            
            Assert.AreEqual(500, health);
        }

        static HealthComponent GetDummy(int maxHealth = 100, int startHealth = 100, bool withRespawnComponent = false)
        {
            var testEntity = new GameObject();
            testEntity.SetActive(false);
            var healthComponent = testEntity.AddComponent<HealthComponent>();
            healthComponent.maxHealth = maxHealth;
            healthComponent.startHealth = startHealth;

            if (withRespawnComponent)
            {
                testEntity.AddComponent<Rigidbody2D>();
            }
            
            testEntity.SetActive(true);

            return healthComponent;
        }
    }
}