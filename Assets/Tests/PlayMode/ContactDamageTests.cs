using NUnit.Framework;
using UnityEngine;
using Gameplay.Combat;

namespace DragonBlaze.Tests
{
    public class ContactDamageTests
    {
        GameObject hazard = null!;
        GameObject player = null!;
        Health health = null!;

        [SetUp]
        public void Setup()
        {
            // Step physics by hand so the trigger callbacks are deterministic inside a single test.
            Physics2D.simulationMode = SimulationMode2D.Script;

            hazard = new GameObject("Hazard");
            hazard.transform.position = Vector3.zero;
            var hazardBody = hazard.AddComponent<Rigidbody2D>();
            hazardBody.bodyType = RigidbodyType2D.Kinematic;
            var hazardCollider = hazard.AddComponent<BoxCollider2D>();
            hazardCollider.isTrigger = true;
            hazardCollider.size = Vector2.one;
            hazard.AddComponent<ContactDamage>();

            player = new GameObject("Player") { tag = "Player" };
            player.AddComponent<SpriteRenderer>();
            player.AddComponent<Animator>();
            player.transform.position = Vector3.zero;
            player.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            player.AddComponent<BoxCollider2D>().size = Vector2.one;
            health = player.AddComponent<Health>();
        }

        [TearDown]
        public void Teardown()
        {
            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
            Object.DestroyImmediate(hazard);
            Object.DestroyImmediate(player);
        }

        [Test]
        public void ContactDamage_DamagesPlayerInsideTrigger()
        {
            Physics2D.SyncTransforms();
            Physics2D.Simulate(Time.fixedDeltaTime);
            Physics2D.Simulate(Time.fixedDeltaTime);

            Assert.Less(health.CurrentHealth, health.MaxHealth, "touching the hazard must cost health");
        }

        [Test]
        public void ContactDamage_IgnoresObjectsWithoutHealth()
        {
            Object.DestroyImmediate(health);

            Physics2D.SyncTransforms();
            Physics2D.Simulate(Time.fixedDeltaTime);

            Assert.Pass("no exception when the player has no Health component");
        }
    }
}
