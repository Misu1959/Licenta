using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public GameObject inventory;
    private Transform[] slots = new Transform[15];
    
    [HideInInspector] public Item selectedItem;

    void Start()
    {
        instance = this;
    
        SetSlots();
    }

    void SetSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int j = i;
            slots[i] = inventory.transform.GetChild(i);
            slots[i].GetComponent<Button>().onClick.AddListener(()=> AddItemToEmptySlot(slots[j]));
        }
    }

    Transform FindFreeSlot()
    {
        foreach (Transform slot in slots)
        {
            if (slot.childCount == 0)
                return slot;
        }

        return null;
    }
    public Transform FindSlot(GameObject itemToAdd)
    {
        Transform aux = null;
        foreach (Transform slot in slots)
        {
            if (!aux)
                if (slot.childCount == 0)
                    aux = slot;

            if(slot.childCount>0)
                if(slot.GetChild(0).GetComponent<Item>().type == itemToAdd.GetComponent<Item>().type)
                    if(!slot.GetChild(0).GetComponent<Item>().CheckIfStackIsFull()) 
                        return slot;

        }

        return aux;
    }

    public void AddItemToSlot(GameObject itemToAdd)
    {
        Transform slot = FindSlot(itemToAdd);

        if (slot.childCount == 0)
        {
            itemToAdd.GetComponent<Item>().CreateItemUI(slot, itemToAdd.GetComponent<Item>().currentStack);
            CraftingManager.instance.SetTooltipCraftButton();

            Destroy(itemToAdd);
        }
        else
        {
            Item itemInSlot = slot.GetChild(0).GetComponent<Item>();
            int dif = itemInSlot.maxStack- itemInSlot.currentStack;

            if (itemToAdd.GetComponent<Item>().currentStack >= dif)
            {
                itemToAdd.GetComponent<Item>().TakeFromStack(dif);
                itemInSlot.AddToStack(dif);

                AddItemToSlot(itemToAdd);
            }
            else
            {
                itemInSlot.AddToStack(itemToAdd.GetComponent<Item>().currentStack);
                Destroy(itemToAdd);
            }
            CraftingManager.instance.SetTooltipCraftButton();
        }

    }

    public void SetBackToSlot()
    {
        if (!selectedItem)
            return;

        selectedItem.transform.SetParent(FindFreeSlot());
        selectedItem.transform.localPosition = Vector2.zero;
        selectedItem.GetComponent<Image>().raycastTarget = true;
        selectedItem = null;

        PopUpManager.instance.ShowMousePopUp();
    }

    void AddItemToEmptySlot(Transform slot)
    {
        if ((!selectedItem) || slot.childCount > 0)
            return;

        selectedItem.transform.SetParent(slot);
        selectedItem.transform.localPosition = Vector2.zero;
        selectedItem.GetComponent<Image>().raycastTarget = true;
        selectedItem = null;
    }

    public int AmountOwnedOfType(string _type)
    {
        int totalAmount = 0;

        foreach (Transform slot in slots)
            if (slot.childCount > 0)
                if (slot.GetChild(0).GetComponent<Item>().type == _type)
                    totalAmount += slot.GetChild(0).GetComponent<Item>().currentStack;

        return totalAmount;
    }

    public void SpendResources(string _type, int _amount,bool dontLookForFullStack = true)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].childCount > 0)
                if (slots[i].GetChild(0).GetComponent<Item>().type == _type)
                {
                    int aux = slots[i].GetChild(0).GetComponent<Item>().currentStack;

                    if (dontLookForFullStack)
                        if (slots[i].GetChild(0).GetComponent<Item>().CheckIfStackIsFull())
                            continue;

                    slots[i].GetChild(0).GetComponent<Item>().TakeFromStack(_amount);
                    _amount -= aux;


                    if (_amount <= 0)
                        return;
                }
            if (i == slots.Length - 1)
                if (_amount > 0)
                {
                    dontLookForFullStack = false;
                    i = -1;
                }

        }
    }


    public GameObject FindItem(string _type)
    {
        foreach (Transform slot in slots)
            if (slot.childCount > 0)
                if (slot.GetChild(0).GetComponent<Item>().type == _type)
                    return slot.GetChild(0).gameObject;

        return null;
    }
}
