using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fire : MonoBehaviour
{
    private Animator animator;

    private GameObject lightObject;

    [SerializeField] protected Timer fireTimer;
    [SerializeField] private int rangeHitBox;

    public virtual void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();

        fireTimer.StartTimer();

        lightObject = transform.GetChild(1).gameObject;
        lightObject.SetActive(true);
    }

    public virtual void Update() => SetFireLight();

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



    void SetLightSize()
    {
        lightObject.GetComponent<SphereCollider>().radius = rangeHitBox * fireTimer.RemainedTime() / fireTimer.MaxTime();

        float lightSize = 180 * fireTimer.RemainedTime() / fireTimer.MaxTime();

        lightObject.transform.GetChild(0).GetComponent<Light>().spotAngle = lightSize;
        lightObject.transform.GetChild(0).GetComponent<Light>().innerSpotAngle = lightSize / 3;
    }



    void SetAnim()
    {
        if (!animator) return;


        if (fireTimer.IsElapsed())
            animator.SetInteger("FireLevel", 0);
        else if (fireTimer.IsElapsedPercent(85))
            animator.SetInteger("FireLevel", 1);
        else if (fireTimer.IsElapsedPercent(50))
            animator.SetInteger("FireLevel", 2);
        else if (fireTimer.IsElapsedPercent(15))
            animator.SetInteger("FireLevel", 3);
        else
            animator.SetInteger("FireLevel", 4);

    }
}
