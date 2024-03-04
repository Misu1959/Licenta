using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    private Vector3 movementDirection;

    private void Update() => Move();

    void FixedUpdate()
    {
        if(!GetComponent<MobActionManagement>().IsRunning())
           GetComponent<Rigidbody>().velocity = movementDirection.normalized * GetComponent<MobStats>().GetSpeed();
        else
           GetComponent<Rigidbody>().velocity = movementDirection.normalized * GetComponent<MobStats>().GetRunSpeed();
    }


    private void Move()
    {
        SetMovementAnimation();

        if (GetComponent<MobActionManagement>().currentActionMode == MobActionManagement.ActionMode.rest)
        {
            movementDirection = Vector3.zero;
            return;
        }

        if (GetComponent<MobActionManagement>().currentTarget)
            movementDirection = GetComponent<MobActionManagement>().currentTarget.transform.position - transform.position;

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

        if (movementDirection == Vector3.zero) // If mob is not moving
        {
            // Seting mob idle state

            if (GetComponent<MobStats>().animator.GetInteger("MovingUpDown") > 1)
                GetComponent<MobStats>().animator.SetInteger("MovingUpDown", 1);
            else if (GetComponent<MobStats>().animator.GetInteger("MovingUpDown") < -1)
                GetComponent<MobStats>().animator.SetInteger("MovingUpDown", -1);

            if (GetComponent<MobStats>().animator.GetInteger("MovingLeftRight") > 1)
                GetComponent<MobStats>().animator.SetInteger("MovingLeftRight", 1);
            else if (GetComponent<MobStats>().animator.GetInteger("MovingLeftRight") < -1)
                GetComponent<MobStats>().animator.SetInteger("MovingLeftRight", -1);

        }
        else // If the mob is moving
        {
            // Set mob moving state
            int val = GetComponent<MobActionManagement>().IsRunning() == false ? 0 : 1;

            if (movementDirection.z < 0) // Check If mob is going downwards
            {
                GetComponent<MobStats>().animator.SetInteger("MovingUpDown", -2 - val);
                GetComponent<MobStats>().animator.SetInteger("MovingLeftRight", 0);
            }
            else if (movementDirection.z > 0) // Check If mob is going upwards
            {
                GetComponent<MobStats>().animator.SetInteger("MovingUpDown", 2 + val);
                GetComponent<MobStats>().animator.SetInteger("MovingLeftRight", 0);
            }
            else // If it's not moving on Z axis check for X axis
            {
                if (movementDirection.x < 0) // Check If mob is going to the left
                {
                    GetComponent<MobStats>().animator.SetInteger("MovingLeftRight", -2 - val);
                    GetComponent<MobStats>().animator.SetInteger("MovingUpDown", 0);
                }
                else if (movementDirection.x > 0) // Check If mob is going to the right
                {
                    GetComponent<MobStats>().animator.SetInteger("MovingLeftRight", 2 + val);
                    GetComponent<MobStats>().animator.SetInteger("MovingUpDown", 0);
                }
            }
        }
    }

}
