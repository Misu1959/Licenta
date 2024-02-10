using System;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public Transform inventoryPanel;
    public Transform backpackPanel;
    public Transform chestPanel;
    private InventorySlot[] inventorySlots;
    private InventorySlot[] backpackSlots;
    private InventorySlot[] chestSlots;
    public InventorySlot selectedItemSlot { get; private set; }

    void Start()
    {
        instance = this;
        SetSlots();
    }

    private void Update() { MoveSelectedItem(); }

    private void SetSlots()
    {
        selectedItemSlot = inventoryPanel.GetChild(0).GetComponent<InventorySlot>();
        selectedItemSlot.transform.SetParent(inventoryPanel.parent);

        inventorySlots = new InventorySlot[inventoryPanel.childCount];
        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i] = inventoryPanel.GetChild(i).GetComponent<InventorySlot>();

        backpackSlots = new InventorySlot[backpackPanel.childCount];
        for (int i = 0; i < backpackSlots.Length; i++)
            backpackSlots[i] = backpackPanel.GetChild(i).GetComponent<InventorySlot>();

        chestSlots = new InventorySlot[chestPanel.childCount];
        for (int i = 0; i < chestSlots.Length; i++)
            chestSlots[i] = chestPanel.GetChild(i).GetComponent<InventorySlot>();
    }

    private bool CheckSlot(InventorySlot slot, ItemData.Name itemName, bool fullStack)
    {
        if (itemName == ItemData.Name.empty) return !slot.CheckIfItHasItem();
        if (!slot.CheckIfItHasItem()) return false;
        if (!slot.CheckMatchingName(itemName)) return false;
        
        if (!fullStack)
            return !slot.GetItemInSlot().CheckIfStackIsFull();
        else
            return slot.GetItemInSlot().CheckIfStackIsFull();
    }

    private InventorySlot[] GetInventory(InventorySlot[] inventoryToCheck, ItemData.Name itemName, bool fullStack)
    {
        if (inventoryToCheck.Where(slot => CheckSlot(slot, itemName, fullStack)).ToArray().Length == 0) return null;

        return inventoryToCheck.Where(slot => CheckSlot(slot, itemName, fullStack)).ToArray();
    }

    private InventorySlot FirstSlotInInventory(InventorySlot[] inventory, ItemData.Name itemName, bool fullStack) 
    {
        InventorySlot[] listOfSlots = GetInventory(inventory, itemName, fullStack);
        return (listOfSlots == null) ? null : listOfSlots[0];  
    }

    private InventorySlot FirstEmptySlotInInventory(InventorySlot[] inventory)
    {
        InventorySlot[] listOfSlots = GetInventory(inventory, ItemData.Name.empty, false);
        return (listOfSlots == null) ? null : listOfSlots[0];
    }

    public InventorySlot FindSlot(ItemData.Name itemName)
    {
        InventorySlot slotToReturn = FirstSlotInInventory(inventorySlots, itemName, false);
        if (slotToReturn) return slotToReturn;

        if (backpackPanel.gameObject.activeInHierarchy)
        {
            slotToReturn = FirstSlotInInventory(inventorySlots, itemName, false);
            if (slotToReturn) return slotToReturn;
        }

        if (chestPanel.gameObject.activeInHierarchy)
        {
            slotToReturn = FirstSlotInInventory(inventorySlots, itemName, false);
            if (slotToReturn) return slotToReturn;
        }

        slotToReturn = FirstEmptySlotInInventory(inventorySlots);
        if (slotToReturn) return slotToReturn;

        if (backpackPanel.gameObject.activeInHierarchy)
        {
            slotToReturn = FirstEmptySlotInInventory(backpackSlots);
            if (slotToReturn) return slotToReturn;
        }

        if (chestPanel.gameObject.activeInHierarchy)
        {
            slotToReturn = FirstEmptySlotInInventory(chestSlots);
            if (slotToReturn) return slotToReturn;
        }

        return null;
    }

    public void AddItemToInventory(Item_Base itemToAdd)
    {
        ItemUI itemUIToAdd = itemToAdd.GetComponent<ItemUI>();
        if (itemToAdd.GetComponent<Item>()) // If not an UI item
        {
            itemUIToAdd = ItemsManager.instance.CreateItemUI(itemToAdd.GetComponent<Item>());
            Destroy(itemToAdd.gameObject);
        }

        if(itemUIToAdd.GetComponent<EquipmentUI>())
        {
            bool itemAddedToEquipmentSlot;
            
            if(!itemUIToAdd.GetComponent<Storage>())
                itemAddedToEquipmentSlot = EquipmentManager.instance.SetEquipment(itemUIToAdd, false, false);
            else
                itemAddedToEquipmentSlot = EquipmentManager.instance.SetEquipment(itemUIToAdd, true, false);

            if (itemAddedToEquipmentSlot) return;
        }

        InventorySlot slot = FindSlot(itemUIToAdd.GetItemData().name);
        if (slot == null) // If I haven't found an available slot
        {
            DropItem(itemUIToAdd, PlayerStats.instance.transform.position);
        }
        else if (!slot.CheckIfItHasItem()) // if the slot is empty
        {
            AddItemToSlot(slot,itemUIToAdd); //Add the item to a free slot
        }
        else if(slot.CheckIfItHasItem())// If the slot is partially full
        {
            ItemUI itemInSlot = slot.GetItemInSlot(); // I get the item that is in slot
            int dif = itemInSlot.GetItemData().maxStack - itemInSlot.GetItemData().currentStack; // I calculate how much can I add to the item stack
            
            if (itemUIToAdd.GetItemData().currentStack < dif) // If I can add both's item's stack together in one
            {
                itemInSlot.AddToStack(itemUIToAdd.GetItemData().currentStack);
                itemUIToAdd.TakeFromStack(itemUIToAdd.GetItemData().currentStack);
            }
            else // If not I add them until the item that is in stock has a full stack
            {
                itemInSlot.AddToStack(dif);
                itemUIToAdd.TakeFromStack(dif);

                AddItemToInventory(itemUIToAdd); // I call the function again so that I look for a new slot
            }
        }
        

    }

    public void AddItemToSlot(InventorySlot slot, ItemUI itemToAdd)
    {
        RemoveItemFromSlot(itemToAdd); // Remove item from old slot
        slot.SetItemInSlot(itemToAdd); // Add it to the new slot

        itemToAdd.transform.SetParent(slot.transform);
        itemToAdd.transform.localPosition = Vector2.zero;

        if (slot.IsBackpackSlot())
            EquipmentManager.instance.BackpackStorage()?.AddData(itemToAdd.GetItemData(), slot.transform.GetSiblingIndex());

        if (slot.IsChestSlot())
            PlayerActionManagement.instance.currentTarget?.GetComponent<Storage>()?.AddData(itemToAdd.GetItemData(), slot.transform.GetSiblingIndex());
        
        itemToAdd.DisplayItem();
        StartCoroutine(CraftingManager.instance.RefreshCraftingMenu());

    }

    public void RemoveItemFromSlot(InventorySlot slot) 
    {
        if (slot.IsBackpackSlot())
            EquipmentManager.instance.BackpackStorage().RemoveData(slot.transform.GetSiblingIndex());

        if (slot.IsChestSlot())
            PlayerActionManagement.instance.currentTarget?.GetComponent<Storage>()?.RemoveData(slot.transform.GetSiblingIndex());
        
        slot.SetItemInSlot(null);
    }

    public void RemoveItemFromSlot(ItemUI itemToBeRemoved)
    {
        if (itemToBeRemoved?.transform.parent?.GetComponent<InventorySlot>())
        {
            InventorySlot slotToRemoveFrom = itemToBeRemoved.transform.parent.GetComponent<InventorySlot>();

            if (slotToRemoveFrom.GetItemInSlot() == itemToBeRemoved)
                RemoveItemFromSlot(slotToRemoveFrom);
        }
    }


    public void SwapTwoSlots(InventorySlot slot)
    {
        ItemUI aux1     = slot.GetItemInSlot();
        ItemUI aux2     = selectedItemSlot.GetItemInSlot();

        AddItemToSlot(slot, aux2);
        SetSelectedItem(aux1);
    }

    public void SetBackToSlot()
    {
        if (!selectedItemSlot.CheckIfItHasItem()) return;


        if (selectedItemSlot.GetItemInSlot().GetComponent<Storage>())
            EquipmentManager.instance.SetEquipment(selectedItemSlot.GetItemInSlot(), false, false);
        else
            AddItemToInventory(selectedItemSlot.GetItemInSlot());
    }

    public void SetSelectedItem(ItemUI itemToSelect)
    {
        AddItemToSlot(selectedItemSlot, itemToSelect); // Set the selected item

        // If player is going to add fuel or to cook and I change the selected item or unselect it cancel the action
        if (PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.addFuel ||
            PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.cook)
            PlayerActionManagement.instance.CancelAction();
    }

    private void MoveSelectedItem()
    {
        if (!selectedItemSlot.CheckIfItHasItem()) return;

        selectedItemSlot.transform.position = Input.mousePosition;

        if (MyMethods.CheckIfMouseIsOverUI() || MyMethods.CheckIfMouseIsOverItem())
            PopUpManager.instance.ShowMousePopUp("RMB - Cancel");
        else
        {
            PopUpManager.instance.ShowMousePopUp("LMB - Drop\nRMB - Cancel");
            if (Input.GetMouseButtonDown(0))
            {
                DropItem(selectedItemSlot.GetItemInSlot(), Camera.main.ScreenToWorldPoint(Input.mousePosition));
                PopUpManager.instance.ShowMousePopUp();
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            SetBackToSlot();
            PopUpManager.instance.ShowMousePopUp();
        }
    }

    public void DropItem(ItemUI itemToDrop, Vector2 positionToDrop)
    {
        
        Item item = ItemsManager.instance.CreateItem(itemToDrop);
        
        item.transform.SetParent(SaveLoadManager.instance.items.transform);
        item.transform.localPosition = positionToDrop;
        item.SetTransparent(true);

        PlayerActionManagement.instance.SetTargetAndAction(item.gameObject, PlayerActionManagement.Action.drop);

        Destroy(itemToDrop.gameObject,.01f);

    }

    public ItemUI FindSpecificItem(ItemData.Name itemName)
    {
        ItemUI itemUI = FirstSlotInInventory(inventorySlots, itemName, true)?.GetItemInSlot();
        if (itemUI) return itemUI;

        if (backpackPanel.gameObject.activeInHierarchy) // If player has backpackPanel check it
        {
            itemUI = FirstSlotInInventory(backpackSlots, itemName, true)?.GetItemInSlot();
            if (itemUI) return itemUI;
        }
        
        if (chestPanel.gameObject.activeInHierarchy) // If player is searching chestPanel check it
        {
            itemUI = FirstSlotInInventory(chestSlots, itemName, true)?.GetItemInSlot();
            if (itemUI) return itemUI;
        }
        return null;
    }


    public int AmountOwned(ItemData.Name itemName)
    {
        int totalAmount = 0;

        totalAmount += AddAmountToInventory(inventorySlots, itemName, false);
        totalAmount += AddAmountToInventory(inventorySlots, itemName, true);

        if (backpackPanel.gameObject.activeInHierarchy)
        {
            totalAmount += AddAmountToInventory(backpackSlots, itemName, false);
            totalAmount += AddAmountToInventory(backpackSlots, itemName, true);

        }

        if (chestPanel.gameObject.activeInHierarchy)
        {
            totalAmount += AddAmountToInventory(chestSlots, itemName, false);
            totalAmount += AddAmountToInventory(chestSlots, itemName, true);
        }

        return totalAmount;
    }

    private int AddAmountToInventory(InventorySlot[] inventory, ItemData.Name itemName, bool fullStack)
    {
        InventorySlot[] listOfSlots = GetInventory(inventory, itemName, fullStack);
        int amount = 0;

        if (listOfSlots == null) return amount;

        foreach (InventorySlot slot in listOfSlots)
            amount += slot.GetItemInSlot().GetItemData().currentStack;

        return amount;
    }

    public void SpendResources(ItemData.Name itemName, int itemAmount) 
    {

        if (TakeAmountFromInventory(inventorySlots, itemName, false, itemAmount)) return;

        if (backpackPanel.gameObject.activeInHierarchy) // If player has backpackPanel check it
            if (TakeAmountFromInventory(backpackSlots, itemName, false, itemAmount)) return;

        if (chestPanel.gameObject.activeInHierarchy) // If player is checking chestPanel check it
            if (TakeAmountFromInventory(chestSlots, itemName, false, itemAmount)) return;

        if (TakeAmountFromInventory(inventorySlots, itemName, true, itemAmount)) return;

        if (backpackPanel.gameObject.activeInHierarchy) // If player has backpackPanel check it
            if (TakeAmountFromInventory(backpackSlots, itemName, true, itemAmount)) return;

        if (chestPanel.gameObject.activeInHierarchy) // If player is checking chestPanel check it
            if (TakeAmountFromInventory(chestSlots, itemName, true, itemAmount)) return;

    }

    private bool TakeAmountFromInventory(InventorySlot[] inventory, ItemData.Name itemName, bool fullStack, int itemAmount)
    {
        InventorySlot[] listOfSlots = GetInventory(inventory, itemName, fullStack);

        if (listOfSlots == null)   return false;

        foreach (InventorySlot slot in listOfSlots)
        {
            int aux = itemAmount;
            itemAmount -= slot.GetItemInSlot().GetItemData().currentStack;
            slot.GetItemInSlot().TakeFromStack(aux);
            if (itemAmount <= 0) return true;
        }
        return false;
    }


    
    public void DisplayBackpack(Storage backpackStorage = null)
    {
        if (backpackStorage == null)
        {
            backpackPanel.gameObject.SetActive(false);

            for (int i = 0; i < backpackSlots.Length; i++)
                if (backpackSlots[i].CheckIfItHasItem())
                    Destroy(backpackSlots[i].GetItemInSlot().gameObject);

            StartCoroutine(CraftingManager.instance.RefreshCraftingMenu());
            PopUpManager.instance.ShowMousePopUp();
        }
        else
        {
            backpackPanel.gameObject.SetActive(true);

            StorageData storageData = backpackStorage.GetStorageData();
            if (storageData.size > 8)
            {
                backpackPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 400);
                for (int i = 8; i < backpackPanel.childCount; i++)
                    backpackSlots[i].gameObject.SetActive(true);


            }
            else
            {
                backpackPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 300);
                for (int i = 8; i < backpackPanel.childCount; i++)
                    backpackSlots[i].gameObject.SetActive(false);

            }

            for (int i = 0; i < storageData.size; i++)
                if (storageData.items[i].name != ItemData.Name.empty)
                {
                    ItemUI itemUI = ItemsManager.instance.CreateItemUI(storageData.items[i]);
                    AddItemToSlot(backpackSlots[i], itemUI);
                }
        }
    }

    public void DisplayChest(Storage chestStorage = null)
    {
        if (chestStorage == null)
        {
            chestPanel.gameObject.SetActive(false);

            for (int i = 0; i < chestSlots.Length; i++)
                if (chestSlots[i].CheckIfItHasItem())
                    Destroy(chestSlots[i].GetItemInSlot().gameObject);

            StartCoroutine(CraftingManager.instance.RefreshCraftingMenu());
            PopUpManager.instance.ShowMousePopUp();
        }
        else
        {
            chestPanel.gameObject.SetActive(true);

            StorageData storageData = chestStorage.GetStorageData();
            for (int i = 0; i < storageData.size; i++)
                if (storageData.items[i]?.name != ItemData.Name.empty)
                {
                    ItemUI itemUI = ItemsManager.instance.CreateItemUI(storageData.items[i]);
                    AddItemToSlot(chestSlots[i], itemUI);
                }
        }
    }

}
