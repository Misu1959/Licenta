using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fire : MonoBehaviour
{
    private Animator animator;

    private GameObject lightObject;

    [SerializeField] private bool isCampfire;

    [SerializeField] protected Timer fireTimer;
    [SerializeField] private int rangeHitBox;

    public float GetFireTimer_RemainedTime() => fireTimer.RemainedTime();

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        lightObject = transform.GetChild(1).gameObject;
        lightObject.SetActive(true);
    }

    public virtual void Update() => SetFireLight();

    public void SetFireData()
    {
        fireTimer.SetTime(fireTimer.MaxTime());
        fireTimer.StartTimer();
    }
    public void SetFireData(float fireTimer_RemainedTime)
    {
        fireTimer.SetTime(fireTimer_RemainedTime);
        fireTimer.StartTimer();
    }

    void SetFireLight()
    {
        fireTimer.Tick();

        SetLightSize();
        SetAnim();

        if (fireTimer.IsElapsed())
        {
            if (PlayerBehaviour.instance.IsCooking(this.transform))
                PlayerBehaviour.instance.CancelAction(actionInterrupted: true);

            if (!isCampfire)
                Destroy(this.gameObject);

        }
    }


    void SetLightSize()
    {
        lightObject.GetComponent<SphereCollider>().radius = rangeHitBox * fireTimer.RemainedTime() / fireTimer.MaxTime();

        float lightSize = 180 * fireTimer.RemainedTime() / fireTimer.MaxTime();

        lightObject.transform.GetChild(0).GetComponent<Light>().spotAngle = lightSize;
        lightObject.transform.GetChild(0).GetComponent<Light>().innerSpotAngle = lightSize / 3;

    }

    public bool IsFireOn() => fireTimer.RemainedTime() <= 0 ? false : true;

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
