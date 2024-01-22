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

    public new Name name;

    [SerializeField] protected Item.Name[] drops;

    [SerializeField] protected float timeToGather;
    protected Timer timerGather;

    [SerializeField] private float maxTimeToGrow;
    private float timeToGrow;
    private Timer timerGrow;

    private void Start()
    {
        //        yield return null; // Wait a frame
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
        if (!InteractionManager.canInteract || InventoryManager.instance.selectedItem)
            return;

        if (CheckIfCanBeGathered())
            SetToGather();
    }

    public virtual void OnMouseOver()
    {
        if (!InteractionManager.canInteract || InventoryManager.instance.selectedItem)
        {
            PopUpManager.instance.ShowMousePopUp();
            return;
        }


        if (!timerGrow.IsOn())
            PopUpManager.instance.ShowMousePopUp("LMB - Gather");
    }

    private void OnMouseExit()
    {
        PopUpManager.instance.ShowMousePopUp();
    }

    public virtual void SetToGather()
    {
        PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.gather);
    }

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
        Item item = Instantiate(ItemsManager.instance.SearchItemsList(drops[0])).GetComponent<Item>();
        item.name = drops[0];
        item.AddToStack(1);
        InventoryManager.instance.AddItemToSlot(item);

        timerGrow.StartTimer();
        PlayerActionManagement.instance.CompleteAction(); // Complete the action
    }

    void Regrow()
    {
        if (!timerGrow.IsOn())
            return;

        if (!GetComponent<Animator>())
            return;

        timerGrow.Tick();

        if (timerGrow.IsElapsedPercent(100))
            GetComponent<Animator>().SetInteger("GrowthStage", 3);
        else if (timerGrow.IsElapsedPercent(66))
            GetComponent<Animator>().SetInteger("GrowthStage", 2);
        else if (timerGrow.IsElapsedPercent(34))
            GetComponent<Animator>().SetInteger("GrowthStage", 1);
        else
            GetComponent<Animator>().SetInteger("GrowthStage", -1);

    }

    public virtual bool CheckIfCanBeGathered()
    {
        return !timerGrow.IsOn();
        //if ((int)EquipmentManager.instance.GetHandItem()?.equipmentType == (int)howToGather) // If the item in hand matches the action requirement
        //return true;
    }
}
