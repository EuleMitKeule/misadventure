using HotlineHyrule.Entities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrulePlayTests.Entities
{
    public class HealthComponentPlayTests
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

        // [Test]
        // public void ShouldResetHealthOnRespawn()
        // {
        //     var healthComponent = GetDummy(500, 500);
        //     var testEntity = healthComponent.gameObject;
        //     testEntity.AddComponent<Rigidbody2D>();
        //     var respawnComponent = testEntity.AddComponent<RespawnComponent>();
        //     healthComponent.HealthChanged += respawnComponent.OnHealthChanged;
        //     respawnComponent.Respawned += healthComponent.OnRespawned;
        //     var context = new InputAction.CallbackContext();
        //
        //     healthComponent.Health -= 500;
        //      respawnComponent.OnButtonRespawn(context);
        //     var health = healthComponent.Health;
        //     
        //     Assert.AreEqual(500, health);
        // }

        static HealthComponent GetDummy(int maxHealth = 100, int startHealth = 100)
        {
            var testEntity = new GameObject();
            testEntity.SetActive(false);
            var healthComponent = testEntity.AddComponent<HealthComponent>();
            healthComponent.maxHealth = maxHealth;
            healthComponent.startHealth = startHealth;
            testEntity.SetActive(true);

            return healthComponent;
        }
    }
}