using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDistance : MonoBehaviour
{
    private bool setOnce = false;
    private float maxRenderTimer = .1f;
    private float renderTimer;

    private void Start()
    {
        SetOnce();
    }

    private void LateUpdate()
    {
        SetRenderOrder();

    }

    void SetOnce()
    {
        if(GetComponent<Construction>())
            setOnce = true;
        if (GetComponent<Resource>())
            setOnce = true;
        if (GetComponent<Item>())
            setOnce = true;
    }

    void SetRenderOrder()
    {
        if (renderTimer <= 0)
        {
            GetComponent<Renderer>().sortingOrder = (int)(10000 - 10 * (transform.position.y - GetComponent<SpriteRenderer>().bounds.size.y / 2));
            renderTimer = maxRenderTimer;
        }
        else
            renderTimer -= Time.deltaTime;

        if (setOnce)
            Destroy(this);
    }
}
