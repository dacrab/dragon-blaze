using UnityEngine;

using Cainos.LucidEditor;
using Cainos.Common;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    public class Elevator : MonoBehaviour
    {
        [FoldoutGroup("Params")] public Vector2 lengthRange = new Vector2(2, 5);
        [FoldoutGroup("Params")] public float waitTime = 1.0f;
        [FoldoutGroup("Params")] public float moveSpeed = 3.0f;
        [FoldoutGroup("Params")] public State startState = State.Up;

        [FoldoutGroup("Reference")] public Rigidbody2D platform;
        [FoldoutGroup("Reference")] public SpriteRenderer chainL;
        [FoldoutGroup("Reference")] public SpriteRenderer chainR;

        [FoldoutGroup("Runtime"), ShowInInspector]
        public float Length
        {
            get { return length; }
            set
            {
                if (value < 0) value = 0.0f;
                this.length = value;

                platform.transform.localPosition = new Vector3(0.0f, -value, 0.0f);
                chainL.size = new Vector2(0.09375f, value - 8 * 0.03125f );
                chainR.size = new Vector2(0.09375f, value - 8 * 0.03125f );
            }
        }
        private float length;

        [FoldoutGroup("Runtime"), ShowInInspector]
        public State CurState
        {
            get { return curState; }
            set
            {
                curState = value;
            }
        }
        private State curState;


        [FoldoutGroup("Runtime"), ShowInInspector]
        public bool IsWaiting
        {
            get { return isWaiting; }
            set
            {
                if (isWaiting == value) return;
                isWaiting = value;
                waitTimer = 0.0f;
            }
        }
        private bool isWaiting = false;


        private float waitTimer;
        private float curSpeed;
        private float targetLength;
        private SecondOrderDynamics secondOrderDynamics = new SecondOrderDynamics(4.0f, 0.3f, -0.3f);


        private void Start()
        {
            curState = startState;
            Length = curState == State.Up ? lengthRange.y : lengthRange.x;
            targetLength = Length;

            secondOrderDynamics.Reset(targetLength);
        }

        private void Update()
        {
            if (IsWaiting)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer > waitTime) IsWaiting = false;
                curSpeed = 0.0f;
            }
            else
            {
                if (curState == State.Up)
                {
                    curSpeed = -moveSpeed;
                    if (targetLength < lengthRange.x)
                    {
                        curState = State.Down;
                        IsWaiting = true;
                    }
                }
                else if (curState == State.Down)
                {
                    curSpeed = moveSpeed;
                    if (targetLength > lengthRange.y)
                    {
                        curState = State.Up;
                        IsWaiting = true;
                    }
                }
            }

            targetLength += curSpeed * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            Length = secondOrderDynamics.Update(targetLength, Time.fixedDeltaTime);
        }

        public enum State
        {
            Up,
            Down
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Cainos.LucidEditor;

//namespace Cainos.CustomizablePixelCharacter
//{
//    public class Elevator : MonoBehaviour
//    {
//        [FoldoutGroup("Params")] public Vector2 lengthRange = new Vector2(2, 5);
//        [FoldoutGroup("Params")] public float waitTime = 1.0f;
//        [FoldoutGroup("Params")] public float moveSpeedMax = 3.0f;
//        [FoldoutGroup("Params")] public float moveAcc = 10.0f;
//        [FoldoutGroup("Params")] public State startState = State.Up;

//        [FoldoutGroup("Reference")] public SpriteRenderer platform;
//        [FoldoutGroup("Reference")] public SpriteRenderer chainL;
//        [FoldoutGroup("Reference")] public SpriteRenderer chainR;

//        [FoldoutGroup("Runtime"), ShowInInspector]
//        public float Length
//        {
//            get { return length; }
//            set
//            {
//                if (value < 0) value = 0.0f;
//                this.length = value;

//                platform.transform.localPosition = new Vector3(0.0f, -value, 0.0f);
//                chainL.size = new Vector2(0.09375f, value - 8 * 0.03125f);
//                chainR.size = new Vector2(0.09375f, value - 8 * 0.03125f);
//            }
//        }
//        private float length;

//        [FoldoutGroup("Runtime"), ShowInInspector]
//        public State CurState
//        {
//            get { return curState; }
//            set
//            {
//                curState = value;
//            }
//        }
//        private State curState;


//        [FoldoutGroup("Runtime"), ShowInInspector]
//        public bool IsWaiting
//        {
//            get { return isWaiting; }
//            set
//            {
//                if (isWaiting == value) return;
//                isWaiting = value;
//                waitTimer = 0.0f;
//            }
//        }
//        private bool isWaiting = false;


//        private float waitTimer;
//        private float curSpeed;


//        private void Start()
//        {
//            curState = startState;
//            Length = curState == State.Up ? lengthRange.y : lengthRange.x;
//        }

//        private void Update()
//        {
//            Length += curSpeed * Time.deltaTime;

//            if (IsWaiting)
//            {
//                waitTimer += Time.deltaTime;
//                if (waitTimer > waitTime) IsWaiting = false;

//                curSpeed = Mathf.Lerp(curSpeed, 0.0f, 7.5f * Time.deltaTime);
//                float targetLength = curState == State.Up ? lengthRange.y : lengthRange.x;
//                Length = Mathf.Lerp(Length, targetLength, 7.5f * Time.deltaTime);
//            }
//            else
//            {
//                if (curState == State.Up)
//                {
//                    curSpeed = Mathf.MoveTowards(curSpeed, -moveSpeedMax, moveAcc * Time.deltaTime);
//                    if (Length < lengthRange.x)
//                    {
//                        curState = State.Down;
//                        IsWaiting = true;
//                    }
//                }
//                else if (curState == State.Down)
//                {
//                    curSpeed = Mathf.MoveTowards(curSpeed, moveSpeedMax, moveAcc * Time.deltaTime);
//                    if (Length > lengthRange.y)
//                    {
//                        curState = State.Up;
//                        IsWaiting = true;
//                    }
//                }
//            }
//        }

//        public enum State
//        {
//            Up,
//            Down
//        }
//    }
//}

