using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class Fire : MonoBehaviour
{

    private GameObject lightObject;

    [SerializeField] float maxFireSize;

    [SerializeField] protected float maxLifetime;
    protected float lifetime;

    protected Timer timer;

    void Start()
    {
        lifetime = maxLifetime;

        timer = new Timer(maxLifetime,lifetime);
        timer.StartTimer();

        lightObject = transform.GetChild(1).gameObject;
        lightObject.SetActive(true);

    }

    void Update()
    {
        SetFireLight();
    }

    void SetFireLight()
    {
        Debug.Log("X");
        timer.Tick();
        if (timer.IsElapsed())
        {
            if(GetComponent<Fireplace>()?.isCampfire == false)
                Destroy(this.gameObject);

            lifetime = 0;
        }
        else
        {
            Debug.Log("y");
            SetLightSize();
            SetAnim();
        }
    }

    void SetAnim()
    {
        if(timer.IsElapsed())
            GetComponent<Animator>().SetInteger("FireLevel", 0);
        else if (timer.IsElapsedPercent(85))
            GetComponent<Animator>().SetInteger("FireLevel", 1);
        else if (timer.IsElapsedPercent(50))
            GetComponent<Animator>().SetInteger("FireLevel", 2);
        else if (timer.IsElapsedPercent(15))
            GetComponent<Animator>().SetInteger("FireLevel", 3);
        else
            GetComponent<Animator>().SetInteger("FireLevel", 4);

    }

    void SetLightSize()
    {

        lightObject.transform.localScale = Vector2.Lerp(Vector2.zero, new Vector2(maxFireSize / transform.lossyScale.x, maxFireSize / transform.lossyScale.x), lifetime / maxLifetime);
        lightObject.GetComponent<Light2D>().pointLightOuterRadius = Vector2.Lerp(Vector2.zero, new Vector2(maxFireSize, maxFireSize), lifetime / maxLifetime).x;
    }



}
