using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Resource : MonoBehaviour, IPointerDownHandler
{

    private Animator animator;

    public new ObjectName name;

    [SerializeField] protected ObjectName[] drops;

    [SerializeField] protected Timer gatherTimer;
    [SerializeField] private Timer growTimer;

    private void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Update()
    {
        GatherItemOfType();
        Regrow();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;

        if (Input.GetMouseButton(0))
        {
            if (CheckIfCanBeGathered())
                SetToGather();
        }
    }

    public virtual void SetToGather()   =>   PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.gather);

    public virtual void GatherItemOfType()
    {

        // If player isn't harvesting it or if it's not grown
        if (!PlayerActionManagement.instance.IsGathering(this.gameObject))
        {
            gatherTimer.RestartTimer();
            return;
        }

        gatherTimer.StartTimer();
        gatherTimer.Tick();
        if (!gatherTimer.IsElapsed())
            return;

        // Next lines adds the loot to the inventory
        Item item = ItemsManager.instance.CreateItem(drops[0]);
        InventoryManager.instance.AddItemToInventory(item);
        
        growTimer.StartTimer();
        PlayerActionManagement.instance.CompleteAction(); // Complete the action
    }

    void Regrow()
    {
        if (!growTimer.IsOn()) return;
        if (!animator) return;


        growTimer.Tick();

        if (growTimer.IsElapsedPercent(100))
            animator.SetInteger("GrowthStage", 3);
        else if (growTimer.IsElapsedPercent(66))
            animator.SetInteger("GrowthStage", 2);
        else if (growTimer.IsElapsedPercent(34))
            animator.SetInteger("GrowthStage", 1);
        else
            animator.SetInteger("GrowthStage", -1);

    }

    public virtual bool CheckIfCanBeGathered()  => !growTimer.IsOn();
}
