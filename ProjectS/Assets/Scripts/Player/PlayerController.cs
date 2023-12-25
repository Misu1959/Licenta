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
    private Vector2 targetPos;

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
        if (targetPos == Vector2.zero)
            movementDir = new Vector2(
                                     Input.GetAxisRaw("Horizontal"),
                                     Input.GetAxisRaw("Vertical")
                                     );
        else
            transform.position = Vector2.MoveTowards(transform.position, targetPos, PlayerStats.instance.speed * Time.deltaTime);
    }

    private void SetAnim()
    {
        cam.transform.position = transform.position - new Vector3(0, 0, 10);

        if(movementDir != Vector2.zero)
        {
            if(movementDir.x<0)
                transform.eulerAngles = new Vector3(0, 180, 0);
            else
                transform.eulerAngles = new Vector3(0, 0, 0);

            if (movementDir.y < 0)
                anim.SetInteger("GoingUpDown", -2);
            else if (movementDir.y > 0)
                anim.SetInteger("GoingUpDown", 2);
            else
                anim.SetInteger("GoingUpDown", 0);

            if (movementDir.x != 0)
                anim.SetInteger("GoingSide", 2);
            else
                anim.SetInteger("GoingSide", 0);
        }
        else
        {
            if (anim.GetInteger("GoingUpDown") == 2)
                anim.SetInteger("GoingUpDown", 1);
            else if (anim.GetInteger("GoingUpDown") == -2)
                anim.SetInteger("GoingUpDown", -1);

            if (anim.GetInteger("GoingUpDown") != -1 && anim.GetInteger("GoingUpDown") != 1)
            {
                if (anim.GetInteger("GoingSide") == 2)
                    anim.SetInteger("GoingSide", 1);
            }
            else if (anim.GetInteger("GoingSide") == 2)
                    anim.SetInteger("GoingSide", 0);


        }
    }

    public void SetTargetPos(GameObject target)
    {
        if (target)
            targetPos = new Vector2(
                                    target.transform.position.x,
                                    target.transform.position.y - GetComponent<SpriteRenderer>().bounds.size.y / 2
                                    );
        else
            targetPos = Vector2.zero;

        movementDir = Vector2.zero;
    }
}
