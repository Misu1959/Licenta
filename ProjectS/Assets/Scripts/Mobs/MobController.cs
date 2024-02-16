using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    private GameObject currentTarget;

    private Vector3 targetPosition;
    private Vector3 movementDirection;

    private Timer targetTimer;
    private bool isWaiting;
    private bool isFallowingPlayer;

    private void Start()
    {
        targetTimer = new Timer(10);
    }

    private void Update()
    {
        Move();

        if (currentTarget) return;

        targetTimer.StartTimer();
        targetTimer.Tick();

        if (HasArrivedOnPosition() && !isWaiting)
        {
            targetTimer.SetTime(3);
            movementDirection = Vector3.zero;
            isWaiting = true;
        }

        if (targetTimer.IsElapsed())
        {
            SetNewTargetPosition();
            
            if(isWaiting)
                isWaiting = false;
        }
    }

    void FixedUpdate() => GetComponent<Rigidbody>().velocity = movementDirection.normalized * GetComponent<MobStats>().GetSpeed();


    private void Move()
    {
        SetMovementAnimation();
        if (isWaiting) return;

        if(currentTarget)
            targetPosition = currentTarget.transform.position;
            
        movementDirection = targetPosition - transform.position;
    }

    private void SetNewTargetPosition()
    {
        targetPosition = new Vector3(Random.Range(transform.position.x - 10, transform.position.x + 10), 0,
                                     Random.Range(transform.position.z - 10, transform.position.z + 10));
    
    }


    private bool HasArrivedOnPosition() => Vector3.Distance(transform.position, targetPosition) > 1 ? false : true;

    private void SetMovementAnimation()
    {
        /*
            For player animation movement
                0 - nothing
                1 - idle 
                2 - moving
                3 - runing
         */

        if (movementDirection == Vector3.zero) // If mob is not moving
        {
            // Seting mob idle state

            if (GetComponent<MobStats>().animator.GetInteger("GoingUpDown") > 1)
                GetComponent<MobStats>().animator.SetInteger("GoingUpDown", 1);
            else if (GetComponent<MobStats>().animator.GetInteger("GoingUpDown") < -1)
                GetComponent<MobStats>().animator.SetInteger("GoingUpDown", -1);

            if (GetComponent<MobStats>().animator.GetInteger("GoingLeftRight") > 1)
                GetComponent<MobStats>().animator.SetInteger("GoingLeftRight", 1);
            else if (GetComponent<MobStats>().animator.GetInteger("GoingLeftRight") < -1)
                GetComponent<MobStats>().animator.SetInteger("GoingLeftRight", -1);

        }
        else // If the mob is moving
        {
            // Set mob moving state

            int val = 0;
            if (isFallowingPlayer)
                val = 1;

            if (movementDirection.z < 0) // Check If mob is going downwards
            {
                GetComponent<MobStats>().animator.SetInteger("GoingUpDown", -2 - val);
                GetComponent<MobStats>().animator.SetInteger("GoingLeftRight", 0);
            }
            else if (movementDirection.z > 0) // Check If mob is going upwards
            {
                GetComponent<MobStats>().animator.SetInteger("GoingUpDown", 2 + val);
                GetComponent<MobStats>().animator.SetInteger("GoingLeftRight", 0);
            }
            else // If it's not moving on Z axis check for X axis
            {
                if (movementDirection.x < 0) // Check If mob is going to the left
                {
                    GetComponent<MobStats>().animator.SetInteger("GoingLeftRight", -2 - val);
                    GetComponent<MobStats>().animator.SetInteger("GoingUpDown", 0);
                }
                else if (movementDirection.x > 0) // Check If mob is going to the right
                {
                    GetComponent<MobStats>().animator.SetInteger("GoingLeftRight", 2 + val);
                    GetComponent<MobStats>().animator.SetInteger("GoingUpDown", 0);
                }
            }
        }
    }

}
