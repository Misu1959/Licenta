using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Resource : MonoBehaviour, IPointerDownHandler
{
    public enum GrowthStages
    {
        dead        = -10,
        empty       = -1,
        barrelyFull = -2,
        almostFull  = -3,

        full        = 0,
        hide        = 1,
        show        = 2

    };

    protected Animator animator;

    public ObjectName objectName;

    [SerializeField] protected TimeManager.DayState harvestPeriod;
    public TimeManager.DayState GetHarvestPeriod() => harvestPeriod;


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
        animator.SetInteger("Stage", (int)GrowthStages.empty);

        growTimer.StartTimer();
    }

    void Regrow()
    {
        if (!growTimer.IsOn()) return;
        if (!animator) return;

        growTimer.Tick();
        SetAnim();
    }

    public virtual bool CheckIfCanBeGathered()
    {
        if (growTimer.IsOn())
            return false;

        if (harvestPeriod == TimeManager.DayState.allDay)
            return true;
        else if (harvestPeriod == TimeManager.instance.dayState)
            return true;

        return false;

    }

    public virtual void SetAnim()
    {
        if (growTimer.IsElapsedPercent(100))
        {
            if(harvestPeriod == TimeManager.instance.dayState)
                animator.SetInteger("Stage", (int)GrowthStages.show);
            else
                animator.SetInteger("Stage", (int)GrowthStages.hide);

        }
        else if (growTimer.IsElapsedPercent(66))
            animator.SetInteger("Stage", (int)GrowthStages.almostFull);
        else if (growTimer.IsElapsedPercent(33))
            animator.SetInteger("Stage", (int)GrowthStages.barrelyFull);
    }

}
