using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : Equipment
{
    private void Start()
    {
        DisplayStack();
    }

    void Update()
    {
        FollowMouse();

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (Time.timeScale == 0) return;

        if (!InventoryManager.instance.selectedItem)
        {
            if (Input.GetMouseButtonDown(0))
            {
                EquipmentManager.instance.UnequipHandItem(this.gameObject);

                InventoryManager.instance.selectedItem = this;
                transform.SetParent(InventoryManager.instance.inventory, true);
                GetComponent<Image>().raycastTarget = false;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (GetComponent<Equipment>())
                    EquipmentManager.instance.SetEquipment(GetComponent<Equipment>());
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (InventoryManager.instance.selectedItem.type == type)
                {
                    if (CheckIfStackIsFull() || InventoryManager.instance.selectedItem.CheckIfStackIsFull())
                        InventoryManager.instance.SwapTwoSlots(this);
                    else
                    {
                        int dif = InventoryManager.instance.selectedItem.GetComponent<ItemUI>().currentStack + currentStack - maxStack;

                        if (dif > 0)
                        {
                            InventoryManager.instance.selectedItem.GetComponent<ItemUI>().TakeFromStack(dif);
                            AddToStack(dif);
                        }
                        else
                        {
                            AddToStack(InventoryManager.instance.selectedItem.currentStack);
                            Destroy(InventoryManager.instance.selectedItem.gameObject);
                        }
                    }
                }
                else
                    InventoryManager.instance.SwapTwoSlots(this);
            }
        }

    }

    void FollowMouse()
    {
        if (InventoryManager.instance.selectedItem != this)
            return;

        this.gameObject.transform.position = Input.mousePosition;

        if (MyMethods.CheckIfMouseIsOverUI())
            return;


        Fire fire = MyMethods.CheckIfMouseIsOverFire();

        if (fire)
        {
            if (fuelValue != 0)
            {
                PopUpManager.instance.ShowMousePopUp("LMB - Add fuel\nRMB - Cancel");

                if (Input.GetMouseButtonDown(0))
                    if (fire.fireType != Fire.FireType.torch)
                    {
                        PlayerActionManagement.instance.SetTargetAndAction(fire.transform.gameObject, PlayerActionManagement.Action.addFuel);
                        return;
                    }
            }
        }
        else
        {
            PopUpManager.instance.ShowMousePopUp("LMB - Drop\nRMB - Cancel");

            if (Input.GetMouseButtonDown(0))
            {
                CreateItem().Drop((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)); // Create the item and drop it
                Destroy(this.gameObject);// Destroy the Ui item
                return;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            PopUpManager.instance.ShowMousePopUp();
            InventoryManager.instance.SetBackToSlot();
        }
    }

    public override Item CreateItemUI(Transform slot, int amount)
    {

        Item itemUI = base.CreateItemUI(slot, amount);
        itemUI.gameObject.GetComponent<Image>().raycastTarget = false;

        InventoryManager.instance.selectedItem = itemUI;

        return itemUI;

    }
    Item CreateItem()
    {
        Item item = Instantiate(ItemsManager.instance.SearchItemsList(type)).GetComponent<Item>();

        item.SetType(type);
        item.AddToStack(currentStack);

        item.gameObject.transform.localPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        item.transform.SetParent(SaveLoadManager.instance.items.transform);

        item.SetTransparent(true);
        PlayerActionManagement.instance.SetTargetAndAction(item.gameObject, PlayerActionManagement.Action.drop);

        item.GetComponent<Equipment>().SetDurability(GetComponent<Equipment>().durability);

        return item;
    }

    public void DisplayStack()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = durability.ToString();

    }

}
