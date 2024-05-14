using System.Collections.Generic;
using UnityEngine;
using Cainos.Common;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    public class BoundingPlatform : MonoBehaviour
    {
        public Transform platform;

        public float waitTime = 1.0f;
        public float retrieveSpeed = 1.0f;
        public float pushSpeed = 10.0f;

        private float platformYPosDown;
        private float platformYPosUp;
        private float platformYPos;
        private float waitTimer;
        private State curState = State.Down;

        private Vector3 platformPrevPos;
        private Vector2 platformVel;

        private SecondOrderDynamics secondOrderDynamics = new SecondOrderDynamics(4.0f, 0.5f, -0.3f);

        private List<Rigidbody2D> onPlatformRigidbodies;

        private void Push()
        {
            foreach ( Rigidbody2D rb2d in onPlatformRigidbodies)
            {
                rb2d.velocity += pushSpeed * Vector2.up;
            }
            onPlatformRigidbodies.Clear();
        }

        private void Start()
        {
            platformYPosDown = platform.transform.localPosition.y;
            platformYPosUp = 0.0f;
            onPlatformRigidbodies = new List<Rigidbody2D>();

            platformPrevPos = platform.transform.position;

            secondOrderDynamics.Reset(platformYPosDown);
        }

        private void FixedUpdate()
        {
            platformVel = (platform.transform.position - platformPrevPos) / Time.fixedDeltaTime;
            platformPrevPos = platform.transform.position;

            waitTimer += Time.fixedDeltaTime;
            if ( waitTimer > waitTime )
            {
                //to up
                if (curState == State.Down)
                {
                    waitTimer = 0.0f;
                    curState = State.Up;
                    platformYPos = platformYPosUp;

                    Push();
                }
                //to down
                else
                {
                    if (platformYPos > platformYPosDown)
                    {
                        platformYPos -= retrieveSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        waitTimer = 0.0f;
                        platformYPos = platformYPosDown;
                        curState = State.Down;
                    }
                }
            }

            platform.transform.localPosition = Vector3.up * secondOrderDynamics.Update(platformYPos, Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.attachedRigidbody && collision.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic)
            {
                if (onPlatformRigidbodies.Contains(collision.attachedRigidbody)) return;

                onPlatformRigidbodies.Add(collision.attachedRigidbody);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.attachedRigidbody && collision.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic)
            {
                if (onPlatformRigidbodies.Contains(collision.attachedRigidbody) == false) return;
                onPlatformRigidbodies.Remove(collision.attachedRigidbody);
            }
        }

        public enum State
        {
            Up,
            Down
        }
    }
}
