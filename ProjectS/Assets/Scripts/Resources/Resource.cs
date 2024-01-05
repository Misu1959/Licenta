using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Resource : MonoBehaviour, IPointerDownHandler
{
    protected enum GatherType
    {
        gather = 0,
        chop = 1,
        mine = 2
    };

    public string type { get; private set; }

    [SerializeField] protected GatherType howToGather;
    [SerializeField] protected  string[] dropTypes;

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


    public void SetType(string  _type)
    {
        type = _type;
    }
    public void SetToGather()
    {
        if (howToGather == GatherType.gather)
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.gather);
        else if (howToGather == GatherType.chop)
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.chop);
        else if (howToGather == GatherType.mine)
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.mine);
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
        Item item = Instantiate(ItemsManager.instance.SearchItemsList(dropTypes[0])).GetComponent<Item>();
        item.SetType(dropTypes[0]);
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


    public bool CheckIfCanBeGathered()
    {

        if (howToGather == GatherType.gather)
        {
            if (!timerGrow.IsOn())
                return true;
            return false;
        }
        else
        {
            //if ((int)EquipmentManager.instance.GetHandItem()?.equipmentType == (int)howToGather) // If the item in hand matches the action requirement
               // return true;
            return false;
        }
    }
}
