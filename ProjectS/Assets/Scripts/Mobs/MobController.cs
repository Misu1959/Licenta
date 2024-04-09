using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    private MobStats mobStats;
    private MobBehaviour mobBehaviour;

    public Transform personalTarget { get; private set; }
    public Transform currentTarget { get; set; }

    private Vector3 movementDirection;

    public bool canMove { get; private set; }
    public void SetCanMove(bool state) => canMove = state;


    private void Start()
    {
        mobStats = GetComponent<MobStats>();
        mobBehaviour = GetComponent<MobBehaviour>();

        personalTarget = transform.GetChild(1);
        personalTarget.SetParent(null);

        SetCanMove(true);
    }
    void Update() => Move();

    private void FixedUpdate() => mobStats.rigidBody.velocity = movementDirection;

    private void Move()
    {
        SetMovementAnimation();

        if (CurrentSpeed() == 0)
        {
            movementDirection = Vector3.zero;
            return;
        }

        if (mobBehaviour.action == MobBehaviour.Action.runAway)
            movementDirection = transform.position - currentTarget.position;
        else if(currentTarget)
            movementDirection = currentTarget.position - transform.position;

        movementDirection = movementDirection.normalized;
        movementDirection *= CurrentSpeed();
    }

    private float CurrentSpeed()
    {
        if (!canMove)
            return 0;

        if (mobBehaviour.action == MobBehaviour.Action.nothing ||
            mobBehaviour.action == MobBehaviour.Action.sleep)
            return 0;
        else if (mobBehaviour.behaviour == MobBehaviour.Behaviour.walk)
            return mobStats.GetWalkSpeed();
        else 
            return mobStats.GetRunSpeed();

    }

    private void SetMovementAnimation()
    {
        /*
            For player animation movement
                0 - nothing
                1 - idle 
                2 - moving
                3 - runing
         */

        if (CurrentSpeed() == 0) // If mob is not moving
        {
            // Seting mob idle state

            if (mobStats.animator.GetInteger("MovingUpDown") > 1)
                mobStats.animator.SetInteger("MovingUpDown", 1);
            else if (mobStats.animator.GetInteger("MovingUpDown") < -1)
                mobStats.animator.SetInteger("MovingUpDown", -1);

            if (mobStats.animator.GetInteger("MovingLeftRight") > 1)
                mobStats.animator.SetInteger("MovingLeftRight", 1);
            else if (mobStats.animator.GetInteger("MovingLeftRight") < -1)
                mobStats.animator.SetInteger("MovingLeftRight", -1);

        }
        else // If the mob is moving
        {
            // Set mob moving state

            int val = CurrentSpeed() == mobStats.GetWalkSpeed() ? 0 : 1;

            if (Mathf.Abs(movementDirection.z) >= Mathf.Abs(movementDirection.x))
            {
                if (movementDirection.z < 0) // Check If mob is going downwards
                {
                    mobStats.animator.SetInteger("MovingUpDown", -2 - val);
                    mobStats.animator.SetInteger("MovingLeftRight", 0);
                }
                else if (movementDirection.z > 0) // Check If mob is going upwards
                {
                    mobStats.animator.SetInteger("MovingUpDown", 2 + val);
                    mobStats.animator.SetInteger("MovingLeftRight", 0);
                }
            }
            else 
            {
                if (movementDirection.x < 0) // Check If mob is going to the left
                {
                    mobStats.animator.SetInteger("MovingLeftRight", -2 - val);
                    mobStats.animator.SetInteger("MovingUpDown", 0);
                }
                else if (movementDirection.x > 0) // Check If mob is going to the right
                {
                    mobStats.animator.SetInteger("MovingLeftRight", 2 + val);
                    mobStats.animator.SetInteger("MovingUpDown", 0);
                }
            }
        }
    }

}
