using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Resource : MonoBehaviour, IPointerDownHandler
{

    protected Animator animator;

    public new ObjectName name;


    [SerializeField] protected Timer gatherTimer;
    [SerializeField] private Timer growTimer;

    protected virtual void Start() => animator = transform.GetChild(0).GetComponent<Animator>();

    private void Update()
    {
        GatherItemOfType();
        Regrow();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;

        if (Input.GetMouseButton(0))
            if (CheckIfCanBeGathered())
                SetToGather();
    }

    public virtual void SetToGather()   =>   PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.gather);

    public virtual void GatherItemOfType()
    {

        // If player isn't harvesting it or if it's not grown
        if (!PlayerBehaviour.instance.IsGathering(this.transform))
        {
            gatherTimer.RestartTimer();
            return;
        }

        gatherTimer.StartTimer();
        gatherTimer.Tick();
        if (!gatherTimer.IsElapsed()) return;


        GetComponent<LootManagement>().CollectLoot();
        
        PlayerBehaviour.instance.CompleteAction(); // Complete the action

        animator.SetTrigger("PlayerInput");
        animator.SetInteger("Stage", -1);

        growTimer.StartTimer();
    }

    void Regrow()
    {
        if (!growTimer.IsOn()) return;
        if (!animator) return;

        growTimer.Tick();
        SetAnim();
    }

    public virtual bool CheckIfCanBeGathered()  => !growTimer.IsOn();

    public virtual void SetAnim()
    {
        if (growTimer.IsElapsedPercent(100))
            animator.SetInteger("Stage", 0);
        else if (growTimer.IsElapsedPercent(66))
            animator.SetInteger("Stage", -3);
        else if (growTimer.IsElapsedPercent(33))
            animator.SetInteger("Stage", -2);
    }

}
