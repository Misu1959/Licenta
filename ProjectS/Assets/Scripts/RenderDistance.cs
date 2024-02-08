using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDistance : MonoBehaviour
{
    private SpriteRenderer sr;
    private Timer renderTimer;
    private bool setOnce = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        renderTimer = new Timer(.1f);
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
        if (GetComponent<Material>())
            setOnce = true;
    }

    void SetRenderOrder()
    {
        renderTimer.StartTimer();
        renderTimer.Tick();

        if (renderTimer.IsElapsed())
        {
            sr.sortingOrder = (int)(10000 - 100 * (transform.position.y - sr.sprite.bounds.center.normalized.y));

            if (setOnce)
                Destroy(this);
        }
    }
}
