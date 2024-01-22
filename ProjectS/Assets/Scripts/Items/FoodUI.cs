using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FoodUI : Food
{
    public override void Start()
    {
        base.Start();
        DisplayStack();
    }

    void Update()
    {
        FollowMouse();

    }

    void FollowMouse()
    {
        if (!InventoryManager.instance.CheckSelecteditem(this))
            return;

        this.gameObject.transform.position = Input.mousePosition;

        if (MyMethods.CheckIfMouseIsOverUI() || MyMethods.CheckIfMouseIsOverItem())
            PopUpManager.instance.ShowMousePopUp("RMB - Cancel");
        else
        {
            PopUpManager.instance.ShowMousePopUp("LMB - Drop\nRMB - Cancel");
            if (Input.GetMouseButtonDown(0))
            {
                CreateItem().Drop((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)); // Create the item and drop it
                InventoryManager.instance.SetSelectedItem(null); // Remove selected item
                Destroy(this.gameObject);// Destroy the Ui item
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            PopUpManager.instance.ShowMousePopUp();
            InventoryManager.instance.SetBackToSlot();
        }
    }

    public override Item CreateItemUI(int amount)
    {

        Item itemUI = base.CreateItemUI(amount);
        InventoryManager.instance.SetSelectedItem(itemUI);

        return itemUI;

    }
    Item CreateItem()
    {
        Item item = Instantiate(ItemsManager.instance.SearchItemsList(name)).GetComponent<Item>();

        item.name = this.name;
        item.AddToStack(currentStack);

        item.gameObject.transform.localPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        item.transform.SetParent(SaveLoadManager.instance.items.transform);

        item.SetTransparent(true);
        PlayerActionManagement.instance.SetTargetAndAction(item.gameObject, PlayerActionManagement.Action.drop);

        return item;
    }

    public void DisplayStack()
    {
        TextMeshProUGUI textStack = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textStack.text = currentStack + "/" + maxStack;
    }

}
