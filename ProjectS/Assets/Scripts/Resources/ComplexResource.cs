using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexResource : Resource
{
    [SerializeField] EquipmentActionType howToGather;
    [SerializeField] private float hp;

    public override void SetToGather()
    {
        if (howToGather == EquipmentActionType.chop)
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.chop);
        else if (howToGather == EquipmentActionType.mine)
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.mine);
    }

    public override void GatherItemOfType()
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

        TakeDmg();
        EquipmentManager.instance.GetHandItem()?.GetComponent<EquipmentUI>().UseTool();

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
        foreach(ObjectName loot in drops)
            DropItemOfName(loot); // Drop the loot

        PlayerActionManagement.instance.CompleteAction(); // Complete the action
        Destroy(this.gameObject); // Destroy this object

    }

    void DropItemOfName(ObjectName nameOfItem)
    {
        Item drop = ItemsManager.instance.CreateItem(nameOfItem);

        // Set loot position
        drop.transform.position = new Vector2(Random.Range(transform.position.x - 1, transform.position.x + 1),
                                              Random.Range(transform.position.y - 1, transform.position.y + 1));
        drop.transform.SetParent(WorldManager.instance.items.transform); // Set loot parent object

    }

    public override bool CheckIfCanBeGathered()
    {
        // If the item equiped in hand matches the action requirement return true else return false
        return EquipmentManager.instance.GetHandItem()?.GetEquipmentData().actionType != howToGather ? false : true;
    }
}
