using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Gameplay.Combat;

namespace DragonBlaze.Tests
{
    public class HealthTests
    {
        GameObject go;
        Health health;

        [SetUp]
        public void Setup()
        {
            go = new GameObject("TestEntity");
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Animator>();
            health = go.AddComponent<Health>();
        }

        [TearDown]
        public void Teardown() => Object.DestroyImmediate(go);

        [Test]
        public void Health_StartsAtMax()
        {
            Assert.AreEqual(health.MaxHealth, health.CurrentHealth);
            Assert.IsTrue(health.IsAlive);
        }

        [Test]
        public void TakeDamage_ReducesHealth()
        {
            health.TakeDamage(30f);
            Assert.AreEqual(health.MaxHealth - 30f, health.CurrentHealth);
        }

        [Test]
        public void TakeDamage_ClampsToZero()
        {
            health.TakeDamage(9999f);
            Assert.AreEqual(0f, health.CurrentHealth);
            Assert.IsFalse(health.IsAlive);
        }

        [Test]
        public void Heal_RestoresHealth()
        {
            health.TakeDamage(50f);
            health.Heal(25f);
            Assert.AreEqual(health.MaxHealth - 25f, health.CurrentHealth);
        }

        [Test]
        public void Heal_ClampsToMax()
        {
            health.TakeDamage(10f);
            health.Heal(9999f);
            Assert.AreEqual(health.MaxHealth, health.CurrentHealth);
        }
    }
}
