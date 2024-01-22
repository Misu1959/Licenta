using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexResource : Resource
{
    [SerializeField] Equipment.ActionType howToGather;
    [SerializeField] private float hp;

    public override void OnMouseOver()
    {
        if (!InteractionManager.canInteract || InventoryManager.instance.selectedItem )
        {
            PopUpManager.instance.ShowMousePopUp();
            return;
        }

        string popUpText = "";
        bool canBeGathered = CheckIfCanBeGathered();

        if (howToGather == Equipment.ActionType.chop && canBeGathered)
            popUpText = "LMB - Chop";
        else if (howToGather == Equipment.ActionType.mine && canBeGathered)
            popUpText = "LMB - Mine";

        PopUpManager.instance.ShowMousePopUp(popUpText);

    }

    public override void SetToGather()
    {
        if (howToGather == Equipment.ActionType.chop)
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.chop);
        else if (howToGather == Equipment.ActionType.mine)
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.mine);
    }

    public override void GatherItemOfType()
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

        //timerGather.StartTimer();

        TakeDmg();
        EquipmentManager.instance.GetHandItem()?.UseTool();

        if(!EquipmentManager.instance.GetHandItem()) // If there is no equipment stop action
            PlayerActionManagement.instance.CompleteAction();

        if (!Input.GetKey(KeyCode.Space) && !Input.GetMouseButton(0)) // If player don't hold space or LMB
            PlayerActionManagement.instance.CompleteAction();

    }

    void TakeDmg()
    {
        hp--;

        if (hp <= 0)
            DestroyResource();
    }

    void DestroyResource()
    {
        foreach(Item.Name loot in drops)
            DropItemOfName(loot); // Drop the loot

        PlayerActionManagement.instance.CompleteAction(); // Complete the action
        Destroy(this.gameObject); // Destroy this object

        PopUpManager.instance.ShowMousePopUp();
    }

    void DropItemOfName(Item.Name nameOfItem)
    {
        Item drop = Instantiate(ItemsManager.instance.SearchItemsList(nameOfItem)).GetComponent<Item>();
        drop.name = nameOfItem;
        drop.AddToStack(1); 


        // Set loot position
        drop.transform.position = new Vector2(Random.Range(transform.position.x - 1, transform.position.x + 1),
                                              Random.Range(transform.position.y - 1, transform.position.y + 1));
        drop.transform.SetParent(SaveLoadManager.instance.items.transform); // Set loot parent object

    }

    public override bool CheckIfCanBeGathered()
    {
        // If the item equiped in hand matches the action requirement return true else return false
        return EquipmentManager.instance.GetHandItem()?.actionType != howToGather ? false : true;
    }
}
