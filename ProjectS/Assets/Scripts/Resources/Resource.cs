using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Resource : MonoBehaviour, IPointerDownHandler
{
    public enum Name
    {
        berryBush,
        grassBush,
        sappling,
        rock,
        tree,
        redShroom,
        greenShroom,
        blueShroom
    };

    private Animator animator;


    public new Name name;

    [SerializeField] protected ItemData.Name[] drops;

    [SerializeField] protected float timeToGather;
    protected Timer timerGather;

    [SerializeField] private float maxTimeToGrow;
    private float timeToGrow;
    private Timer timerGrow;

    private void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        timerGather = new Timer(timeToGather);
        timerGrow = new Timer(maxTimeToGrow, timeToGrow);
    }

    private void Update()
    {
        GatherItemOfType();
        Regrow();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;

        if (CheckIfCanBeGathered())
            SetToGather();
    }

    public virtual void OnMouseOver()
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false))
        {
            PopUpManager.instance.ShowMousePopUp();
            return;
        }

        if (!timerGrow.IsOn())
            PopUpManager.instance.ShowMousePopUp("LMB - Gather");
    }

    private void OnMouseExit()  {   PopUpManager.instance.ShowMousePopUp(); }

    public virtual void SetToGather()   {   PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.gather);  }

    public virtual void GatherItemOfType()
    {

        // If player isn't harvesting it or if it's not grown
        if (!PlayerActionManagement.instance.IsGathering(this.gameObject))
        {
            timerGather.RestartTimer();
            return;
        }

        timerGather.StartTimer();
        timerGather.Tick();
        if (!timerGather.IsElapsed())
            return;

        // Next lines adds the loot to the inventory
        Item item = ItemsManager.instance.CreateItem(drops[0]);
        InventoryManager.instance.AddItemToInventory(item);
        
        timerGrow.StartTimer();
        PlayerActionManagement.instance.CompleteAction(); // Complete the action
    }

    void Regrow()
    {
        if (!timerGrow.IsOn()) return;
        if (!animator) return;


        timerGrow.Tick();

        if (timerGrow.IsElapsedPercent(100))
            animator.SetInteger("GrowthStage", 3);
        else if (timerGrow.IsElapsedPercent(66))
            animator.SetInteger("GrowthStage", 2);
        else if (timerGrow.IsElapsedPercent(34))
            animator.SetInteger("GrowthStage", 1);
        else
            animator.SetInteger("GrowthStage", -1);

    }

    public virtual bool CheckIfCanBeGathered()  {   return !timerGrow.IsOn();   }
}
