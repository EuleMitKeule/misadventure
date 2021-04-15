using HotlineHyrule.Entities;
using NUnit.Framework;
using UnityEngine;

namespace HotlineHyruleTests.Entities
{
    public class HealthComponentTests
    {
        bool _wasSuccessful;

        [Test]
        public void ShouldSetHealth()
        {
            var healthComponent = GetDummy();
            
            healthComponent.Health = 69;
            
            Assert.AreEqual(69, healthComponent.Health);
        }

        [Test]
        public void ShouldClampHealthToMax()
        {
            var healthComponent = GetDummy(123);

            healthComponent.Health = 1337;
            
            Assert.AreEqual(123, healthComponent.Health); 
        }

        [Test]
        public void ShouldClampHealthToZero()
        {
            var healthComponent = GetDummy();

            healthComponent.Health = -1337;
            
            Assert.AreEqual(0, healthComponent.Health); 
        }

        [Test]
        public void ShouldInvokeHealthChanged()
        {
            var healthComponent = GetDummy();
            healthComponent.HealthChanged += OnHealthChanged;

            healthComponent.Health = 50;
            healthComponent.Health = 75;
            
            Assert.AreEqual(true, _wasSuccessful);
            _wasSuccessful = false;
        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            _wasSuccessful = e.NewHealth == 75 && e.HealthDifference == 25;
        }

        [Test]
        public void ShouldNotClampHealthDifferenceInHealthChangedEvent()
        {
            var healthComponent = GetDummy(60);
            healthComponent.HealthChanged += OnHealthChangedClamp;

            healthComponent.Health = 50;
            healthComponent.Health += 25;
            
            Assert.AreEqual(true, _wasSuccessful);
            _wasSuccessful = false;
        }

        void OnHealthChangedClamp(object sender, HealthEventArgs e)
        {
            _wasSuccessful = e.NewHealth == 60 && e.HealthDifference == 25;
        }

        static HealthComponent GetDummy(int maxHealth = 100, int startHealth = 100)
        {
            var testEntity = new GameObject();
            var healthComponent = testEntity.AddComponent<HealthComponent>();
            healthComponent.maxHealth = maxHealth;

            return healthComponent;
        }
    }
}