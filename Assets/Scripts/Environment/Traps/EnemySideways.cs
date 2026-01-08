using UnityEngine;
using Core.State;

namespace Environment.Traps;

public sealed class EnemySideways : TrapBase
{
    [SerializeField] float movementDistance = 3f, speed = 2f;

    bool movingLeft;
    float leftEdge, rightEdge;

    void Awake()
    {
        leftEdge = transform.position.x - movementDistance;
        rightEdge = transform.position.x + movementDistance;
    }

    void Update()
    {
        if (!GameStateManager.IsCurrentlyPlaying) return;
        
        float dir = movingLeft ? -1f : 1f;
        float newX = transform.position.x + dir * speed * Time.deltaTime;
        
        if (movingLeft && newX <= leftEdge) { newX = leftEdge; movingLeft = false; }
        else if (!movingLeft && newX >= rightEdge) { newX = rightEdge; movingLeft = true; }
        
        transform.position = new(newX, transform.position.y, transform.position.z);
    }
}
