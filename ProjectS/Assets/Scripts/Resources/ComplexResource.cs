using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexResource : Resource
{
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

        if (howToGather == GatherType.chop && canBeGathered)
            popUpText = "LMB - Chop";
        else if (howToGather == GatherType.mine && canBeGathered)
            popUpText = "LMB - Mine";

        PopUpManager.instance.ShowMousePopUp(popUpText);

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
        Debug.Log("x");

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
        for (int i = 0; i < dropTypes.Length; i++)
            DropItemOfType(dropTypes[i]);

        PlayerActionManagement.instance.CompleteAction();
        PopUpManager.instance.ShowMousePopUp();
        Destroy(this.gameObject);
    }

    void DropItemOfType(string typeOfItem)
    {
        Item drop = Instantiate(ItemsManager.instance.SearchItemsList(typeOfItem)).GetComponent<Item>();
        drop.SetType(typeOfItem);
        drop.AddToStack(1);

        drop.transform.position = new Vector2(Random.Range(transform.position.x - 1, transform.position.x + 1),
                                              Random.Range(transform.position.y - 1, transform.position.y + 1));
        drop.transform.SetParent(SaveLoadManager.instance.items.transform);

    }
}
