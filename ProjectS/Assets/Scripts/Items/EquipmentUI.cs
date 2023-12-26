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
                if (Input.GetKey(KeyCode.LeftControl) && currentStack > 1)
                {
                    CreateItemUI(transform.parent.parent.parent, 1, true);
                    TakeFromStack(1);
                }
                else if (Input.GetKey(KeyCode.LeftShift) && currentStack > 1)
                {
                    int amount;
                    if (currentStack % 2 == 0)
                        amount = currentStack / 2;
                    else
                        amount = currentStack / 2 + 1;

                    CreateItemUI(transform.parent.parent.parent, amount, true);
                    TakeFromStack(amount);
                }
                else
                {
                    EquipmentManager.instance.UnequipHandItem(this.gameObject);

                    InventoryManager.instance.selectedItem = this;
                    transform.SetParent(transform.parent.parent.parent, true);
                    GetComponent<Image>().raycastTarget = false;
                }

            if (Input.GetMouseButtonDown(1))
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
                        SwapTwoSlots();
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
                    SwapTwoSlots();
            }
        }

    }

    void FollowMouse()
    {
        if (InventoryManager.instance.selectedItem != this)
            return;

        this.gameObject.transform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
            if (CheckIfItIsOverUI())
                return;
            else
            {
                Fire fire = CheckIfItIsOverFire();
                if (fire)
                {

                    if (fuelValue > 0)
                    {
                        PlayerActionManagement.instance.PerformAction(fire.transform.gameObject, PlayerActionManagement.Action.addFuel);
                        PopUpManager.instance.ShowMousePopUp("LMB - Add fuel\nRMB - Cancel");

                    }
                    return;
                }
                CreateItem();
                Destroy(this.gameObject);
                CraftingManager.instance.SetTooltipCraftButton();
            }

        PopUpManager.instance.ShowMousePopUp("LMB - Drop\nRMB - Cancel");

        if (Input.GetMouseButtonDown(1))
        {
            PopUpManager.instance.ShowMousePopUp();
            InventoryManager.instance.SetBackToSlot();
        }
    }

    void CreateItem()
    {
        Item item = Instantiate(ItemsManager.instance.SearchItemsList(type)).GetComponent<Item>();

        item.SetType(type);
        item.AddToStack(currentStack);

        item.gameObject.transform.localPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        item.transform.SetParent(SaveLoadManager.instance.items.transform);

        item.SetTransparent(true);
        PlayerActionManagement.instance.PerformAction(item.gameObject, PlayerActionManagement.Action.drop);

        item.GetComponent<Equipment>().SetDurability(GetComponent<Equipment>().durability);
    }


    void SwapTwoSlots()
    {
        InventoryManager.instance.selectedItem.transform.SetParent(this.transform.parent);
        InventoryManager.instance.selectedItem.transform.localPosition = Vector2.zero;
        InventoryManager.instance.selectedItem.GetComponent<Image>().raycastTarget = true;


        InventoryManager.instance.selectedItem = this;

        InventoryManager.instance.selectedItem.transform.SetParent(this.transform.parent.parent);
        InventoryManager.instance.selectedItem.GetComponent<Image>().raycastTarget = false;
    }

    public void DisplayStack()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = durability.ToString();

    }


    bool CheckIfItIsOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);
        for (int i = 0; i < raycastResultsList.Count; i++)
            if (!raycastResultsList[i].gameObject.GetComponent<CanvasRenderer>())
                raycastResultsList.RemoveAt(i--);

        return raycastResultsList.Count > 0;
    }

    Fire CheckIfItIsOverFire()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);
        for (int i = 0; i < raycastResultsList.Count; i++)
            if (!raycastResultsList[i].gameObject.GetComponent<Fire>())
                raycastResultsList.RemoveAt(i--);

        Fire fire = null;

        if (raycastResultsList.Count == 1)
            fire = raycastResultsList[0].gameObject.GetComponent<Fire>();

        return fire;
    }
}
