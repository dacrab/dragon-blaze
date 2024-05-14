using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    //moving platform
    //used on a moving platform so that objects fallen on this platform will perfectly follow to it
    //otherwise due to physical simulation precision problem object will not follow the moving platform as expected

    public class MovingPlatform : MonoBehaviour
    {
        public float velocityInheritPercent = 0.8f;     

        private List<Transform> onPlatformObjects;

        private Vector3 prevPos;
        private Vector2 velocity;

        private void Start()
        {
            onPlatformObjects = new List<Transform>();
            prevPos = transform.position;
        }

        private void FixedUpdate()
        {
            velocity = (transform.position - prevPos) / Time.fixedDeltaTime;
            prevPos = transform.position;

            foreach (Transform t in onPlatformObjects)
            {
                t.Translate(velocity * Time.fixedDeltaTime);
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.attachedRigidbody && collision.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic)
            {
                if (onPlatformObjects.Contains(collision.transform)) return;

                onPlatformObjects.Add(collision.transform);
                if (collision.attachedRigidbody) collision.attachedRigidbody.velocity -= velocity * velocityInheritPercent;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.attachedRigidbody && collision.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic)
            {
                if (onPlatformObjects.Contains(collision.transform) == false) return;
                onPlatformObjects.Remove(collision.transform);

                if (collision.attachedRigidbody) collision.attachedRigidbody.velocity += velocity * velocityInheritPercent;
            }
        }
    }
}
