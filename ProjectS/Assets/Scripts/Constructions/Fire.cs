using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fire : MonoBehaviour
{
    private GameObject lightObject;
    public int fireType;

    [SerializeField] float maxFireSize;
    [SerializeField] private float maxLifetime;
    private float lifetime;


    void Start()
    {
        lifetime = maxLifetime;
        
        transform.GetChild(0).gameObject.SetActive(true);
        
        lightObject = transform.GetChild(1).gameObject;
        lightObject.SetActive(true);

    }

    void Update()
    {
        if (lifetime < 0)
        {
            if (fireType != 100)
                Destroy(this.gameObject);
            
            lifetime = 0;
        }
        else if (lifetime > 0)
        {
            SetLightSize();
            SetAnim();
        }

    }

    void SetAnim()
    {
        if (lifetime == 0)
            GetComponent<Animator>().SetInteger("FireLevel", 0);
        else if (lifetime < .25f * maxLifetime)
            GetComponent<Animator>().SetInteger("FireLevel", 1);
        else if (lifetime < .50f * maxLifetime)
            GetComponent<Animator>().SetInteger("FireLevel", 2);
        else if (lifetime < .75f * maxLifetime)
            GetComponent<Animator>().SetInteger("FireLevel", 3);
        else if (lifetime < 1 * maxLifetime)
            GetComponent<Animator>().SetInteger("FireLevel", 4);

    }

    public void AddFuel(Item item)
    {

        lifetime = Mathf.Clamp(lifetime + item.fuelValue, 0, maxLifetime);

        item.TakeFromStack(1);
    }

    void SetLightSize()
    {
        lifetime -= Time.deltaTime;

        lightObject.transform.localScale = Vector2.Lerp(Vector2.zero, new Vector2(maxFireSize / transform.lossyScale.x, maxFireSize / transform.lossyScale.x), lifetime / maxLifetime);
        lightObject.GetComponent<Light2D>().pointLightOuterRadius = Vector2.Lerp(Vector2.zero, new Vector2(maxFireSize, maxFireSize), lifetime / maxLifetime).x;
    }



}
