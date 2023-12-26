using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    Camera cam;
    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 movementDir;

    private GameObject currentTarget;

    void Start()
    {

        instance= this;

        cam  = Camera.main;
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        SetAnim();
    }

    private void FixedUpdate()
    {
        rb.velocity = movementDir.normalized * PlayerStats.instance.speed;
    }

    private void Move()
    {
        if (currentTarget) // If player has a target
            //transform.position = Vector2.MoveTowards(transform.position, currentTarget.transform.position, PlayerStats.instance.speed * Time.deltaTime); Old way
            movementDir =  currentTarget.transform.position - transform.position;
        else // Move by WASD
            movementDir = new Vector2(
                                     Input.GetAxisRaw("Horizontal"),
                                     Input.GetAxisRaw("Vertical")
                                     );
    }

    public void SetTarget(GameObject target = null)
    {
        currentTarget = target;

        if (!currentTarget)
            movementDir = Vector2.zero;

    }


    private void SetAnim()
    {
        /*
        For player animation movement
            0 - nothing
            1 - idle 
            2 - moving
         */


        cam.transform.position = transform.position - new Vector3(0, 0, 10); // Set camera on player

        if(movementDir == Vector2.zero) // If player is not moving
        {
            // Seting player idle state

            if (anim.GetInteger("GoingUpDown") == 2)
                anim.SetInteger("GoingUpDown", 1);
            else if (anim.GetInteger("GoingUpDown") == -2)
                anim.SetInteger("GoingUpDown", -1);

            if (anim.GetInteger("GoingLeftRight") == 2)
                anim.SetInteger("GoingLeftRight", 1);
            else if (anim.GetInteger("GoingLeftRight") == -2)
                anim.SetInteger("GoingLeftRight", -1);

        }
        else // If the player is moving
        {
            // Set player moving state

            if (movementDir.y < 0) // Check If player is going downwards
            {
                anim.SetInteger("GoingUpDown", -2);
                anim.SetInteger("GoingLeftRight", 0);
            }
            else if (movementDir.y > 0) // Check If player is going upwards
            {
                anim.SetInteger("GoingUpDown", 2);
                anim.SetInteger("GoingLeftRight", 0);
            }
            else // If it;s not moving on Y axis check for X axis
            {
                if (movementDir.x < 0) // Check If player is going to the left
                {
                    anim.SetInteger("GoingLeftRight", -2);
                    anim.SetInteger("GoingUpDown", 0);
                }
                else if (movementDir.x > 0) // Check If player is going to the right
                {
                    anim.SetInteger("GoingLeftRight", 2);
                    anim.SetInteger("GoingUpDown", 0);
                }
            }
        }
    }

}