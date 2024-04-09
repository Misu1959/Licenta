using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private Vector3 movementDir;
    public  Vector3 keyboardMovement { get; private set; }
    public bool canMove { get; private set; }
    public void SetCanMove(bool state) => canMove = state;

    public void SetKeyboardMovement()  
    {
        if (!canMove)
            keyboardMovement = Vector3.zero;
        else
            keyboardMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    void Start()
    {
        instance = this;

        SetCanMove(true);
    }

    void Update() => Move();

    private void FixedUpdate()  => PlayerStats.instance.rigidBody.velocity = movementDir.normalized * PlayerStats.instance.speed;

    private void Move()
    {
        SetKeyboardMovement();
        SetMovementAnimation();

        if (PlayerBehaviour.instance.currentTarget) // If player has a target
        {
            if (PlayerBehaviour.instance.isPerformingAction)
                movementDir = Vector2.zero;
            else if (canMove)
                movementDir = PlayerBehaviour.instance.currentTarget.transform.position - transform.position;
        }
        else
            movementDir = keyboardMovement;
    }

    private void SetMovementAnimation()
    {
        /*
        For player animation movement
            0 - nothing
            1 - idle 
            2 - moving
         */

        if(movementDir == Vector3.zero) // If player is not moving
        {
            // Seting player idle state

            if (PlayerStats.instance.animator.GetInteger("MovingUpDown") == 2)
                PlayerStats.instance.animator.SetInteger("MovingUpDown", 1);
            else if (PlayerStats.instance.animator.GetInteger("MovingUpDown") == -2)
                PlayerStats.instance.animator.SetInteger("MovingUpDown", -1);

            if (PlayerStats.instance.animator.GetInteger("MovingLeftRight") == 2)
                PlayerStats.instance.animator.SetInteger("MovingLeftRight", 1);
            else if (PlayerStats.instance.animator.GetInteger("MovingLeftRight") == -2)
                PlayerStats.instance.animator.SetInteger("MovingLeftRight", -1);

        }
        else // If the player is moving
        {
            // Set player moving state

            if (Mathf.Abs(movementDir.z) >= Mathf.Abs(movementDir.x))
            {
                if (movementDir.z < 0)
                {
                    PlayerStats.instance.animator.SetInteger("MovingUpDown", -2);
                    PlayerStats.instance.animator.SetInteger("MovingLeftRight", 0);
                }
                else if (movementDir.z > 0)
                {
                    PlayerStats.instance.animator.SetInteger("MovingUpDown", 2);
                    PlayerStats.instance.animator.SetInteger("MovingLeftRight", 0);
                }
            }
            else
            {
                if (movementDir.x < 0) // Check If player is going to the left
                {
                    PlayerStats.instance.animator.SetInteger("MovingUpDown", 0);
                    PlayerStats.instance.animator.SetInteger("MovingLeftRight", -2);
                }
                else if (movementDir.x > 0) // Check If player is going to the right
                {
                    PlayerStats.instance.animator.SetInteger("MovingUpDown", 0);
                    PlayerStats.instance.animator.SetInteger("MovingLeftRight", 2);
                }
            }
        }
    }
}

