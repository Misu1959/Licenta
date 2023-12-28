using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexResource : Resource
{
    [SerializeField] private float hp;

    public override void OnMouseEnter()
    {
        string popUpText = "";
        bool canBeGathered = CheckIfCanBeGathered();

        if (howToGather == 1 && canBeGathered)
            popUpText = "LMB - Chop";
        else if (howToGather == 2 && canBeGathered)
            popUpText = "LMB - Mine";

        PopUpManager.instance.ShowMousePopUp(popUpText);

    }

    public override void GatherItemOfType()
    {
        // If player isn't harvesting it or if it's not grown
        if (!isBeingGathered || !isGrown)
            return;

        if(!PlayerActionManagement.instance.isPerformingAction)
        {
            SetIsBeingGathered(false);
            return;
        }

        if (timeToGather <= 0) // Gather
        {
            timeToGather = maxTimeToGather;

            EquipmentManager.instance.GetHandItem().UseTool();
            TakeDmg();

            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) // If player holds space or LMB
                if (EquipmentManager.instance.GetHandItem()?.durability > 0) // If item still has durability continue
                {
                    SetIsBeingGathered(true);
                    return;
                }
            PlayerActionManagement.instance.CompleteAction();
            SetIsBeingGathered(false);
        }
        else
            timeToGather -= Time.deltaTime;

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
