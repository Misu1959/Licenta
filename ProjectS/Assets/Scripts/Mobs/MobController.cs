using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    public Transform spawner { private get; set; }
    private Transform restObject;

    private Vector3 targetPosition;
    private Vector3 movementDirection;

    [HideInInspector] public bool isWaiting;

    private void Start()
    {
        SetRestObject();
        SetTargetPosition();
    }
    private void Update() => Move();

    void FixedUpdate() => GetComponent<Rigidbody>().velocity = movementDirection.normalized * GetComponent<MobStats>().GetSpeed();


    private void Move()
    {
        SetMovementAnimation();

        if (isWaiting)
        {
            movementDirection = Vector3.zero;
            return;
        }

        if (GetComponent<MobActionManagement>().currentTarget)
            targetPosition = GetComponent<MobActionManagement>().currentTarget.transform.position;

        movementDirection = targetPosition - transform.position;
    }

    private void SetRestObject()
    {
        restObject = transform.GetChild(1);
        restObject.SetParent(null);
    }

    public void SetTargetPosition()
    {
        if(!spawner)
            targetPosition = new Vector3(Random.Range(spawner.position.x - 10, spawner.position.x + 10), 0,
                                         Random.Range(spawner.position.z - 10, spawner.position.z + 10));
        else
            targetPosition = new Vector3(Random.Range(transform.position.x - 10, transform.position.x + 10), 0,
                                         Random.Range(transform.position.z - 10, transform.position.z + 10));

        restObject.transform.position = targetPosition;
        GetComponent<MobActionManagement>().SetTargetAndAction(restObject.gameObject, MobActionManagement.Action.rest);
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
            if (GetComponent<MobActionManagement>().currentTarget)
                if (GetComponent<MobActionManagement>().currentTarget != restObject.gameObject)
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
