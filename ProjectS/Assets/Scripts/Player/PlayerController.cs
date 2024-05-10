using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private Vector3 movementDir;
    public bool canMove { get; private set; }
    public void SetCanMove(bool state) => canMove = state;
    public void SetCanMove() => StartCoroutine(SetCanMove(.5f));

    private IEnumerator SetCanMove(float timeToWait)
    {
        canMove = false;
        yield return new WaitForSeconds(timeToWait);
        canMove = true;
    }

    public bool isMovingByKeyboard { get; private set; }

    void Awake() => instance = this;

    private void Start() => SetCanMove(true);

    private void Update() => Move();
    private void FixedUpdate() => PlayerStats.instance.rigidBody.velocity = movementDir.normalized * PlayerStats.instance.speed * ((canMove == false) ? 0 : 1);

    private void Move()
    {
        Vector2 keyboardinput = PlayerStats.instance.inputActions.Player.Move.ReadValue<Vector2>();
        isMovingByKeyboard = (keyboardinput == Vector2.zero) ? false : true;

        if (PlayerBehaviour.instance.currentTarget) // If player has a target
        {
            Vector3 targetWay = PlayerBehaviour.instance.currentTarget.position - transform.position;
            movementDir = PlayerBehaviour.instance.isPerformingAction ? Vector2.zero : targetWay;
        }
        else
            movementDir = new Vector3(keyboardinput.x, 0, keyboardinput.y);

        SetMovementAnimation();
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

