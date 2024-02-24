using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fire : MonoBehaviour
{

    private GameObject lightObject;

    [SerializeField] float fireSize;

    [SerializeField] protected Timer fireTimer;

    public virtual void Start()
    {
        fireTimer.StartTimer();

        lightObject = transform.GetChild(1).gameObject;
        lightObject.SetActive(true);
    }

    public virtual void Update()
    {
        SetFireLight();
    }

    void SetFireLight()
    {
        fireTimer.Tick();
        if (fireTimer.IsElapsed())
        {
            if(GetComponent<Fireplace>()?.isCampfire == false)
                Destroy(this.gameObject);

        }
        else
        {
            SetLightSize();
            SetAnim();
        }
    }

    void SetAnim()
    {
        if (!GetComponent<Animator>()) return;


        if (fireTimer.IsElapsed())
            GetComponent<Animator>().SetInteger("FireLevel", 0);
        else if (fireTimer.IsElapsedPercent(85))
            GetComponent<Animator>().SetInteger("FireLevel", 1);
        else if (fireTimer.IsElapsedPercent(50))
            GetComponent<Animator>().SetInteger("FireLevel", 2);
        else if (fireTimer.IsElapsedPercent(15))
            GetComponent<Animator>().SetInteger("FireLevel", 3);
        else
            GetComponent<Animator>().SetInteger("FireLevel", 4);

    }

    void SetLightSize()
    {

    }



}
