using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public Transform inventory;
    public Transform backpack;
    public Transform chest;
    private InventorySlot[] inventorySlots = new InventorySlot[15];
    private InventorySlot[] backpackSlots = new InventorySlot[12];
    private InventorySlot[] chestSlots = new InventorySlot[9];
    
    public InventorySlot selectedItemSlot { get; private set; }

    void Start()
    {
        instance = this;
    
        SetSlots();
    }

    private void Update() {   MoveSelectedItem(); }

    private void SetSlots()
    {
        selectedItemSlot = inventory.GetChild(0).GetComponent<InventorySlot>();
        selectedItemSlot.transform.SetParent(inventory.parent);

        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i] = inventory.GetChild(i).GetComponent<InventorySlot>();

        for (int i = 0; i < backpackSlots.Length; i++)
            backpackSlots[i] = backpack.GetChild(i).GetComponent<InventorySlot>();

        for (int i = 0; i < chestSlots.Length; i++)
            chestSlots[i] = chest.GetChild(i).GetComponent<InventorySlot>();
    }

    private InventorySlot FindFreeSlot()
    {
        foreach (InventorySlot slot in inventorySlots)
            if (!slot.CheckIfItHasItem()) // If the slot is empty
                return slot; // Return an empty slot

        if (backpack.gameObject.activeInHierarchy) // If player has backpack check it
            foreach (InventorySlot slot in backpackSlots)
                if (!slot.CheckIfItHasItem()) // If the slot is empty
                    return slot; // Return an empty slot

        if (chest.gameObject.activeInHierarchy) // If player is searching chest check it
            foreach (InventorySlot slot in chestSlots)
                if (!slot.CheckIfItHasItem()) // If the slot is empty
                    return slot; // Return an empty slot

        return null;
    }
    public InventorySlot FindSlot(ItemData.Name nameOfItemToAdd)
    {
        foreach (InventorySlot slot in inventorySlots)
            if (slot.CheckIfItHasItem())   // Check if there is an item in slot
                if (slot.CheckMatchingName(nameOfItemToAdd)) // Check if the items have the same type
                    if(!slot.GetItemInSlot().CheckIfStackIsFull())  // If the item in slot does't have a full stack return it
                        return slot;


        if (backpack.gameObject.activeInHierarchy) // If player has backpack check it
            foreach (InventorySlot slot in backpackSlots)
                if (slot.CheckIfItHasItem())   // Check if there is an item in slot
                    if (slot.CheckMatchingName(nameOfItemToAdd)) // Check if the items have the same type
                        if (!slot.GetItemInSlot().CheckIfStackIsFull())  // If the item in slot does't have a full stack return it
                            return slot;

        if (chest.gameObject.activeInHierarchy) // If player is searching chest check it
            foreach (InventorySlot slot in chestSlots)
                if (slot.CheckIfItHasItem())   // Check if there is an item in slot
                    if (slot.CheckMatchingName(nameOfItemToAdd)) // Check if the items have the same type
                        if (!slot.GetItemInSlot().CheckIfStackIsFull())  // If the item in slot does't have a full stack return it
                            return slot;

        return FindFreeSlot(); // If we haven't found a slot that is partialy full return an empty slot if this exist
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
        StartCoroutine(CraftingManager.instance.RefreshCraftingManager());

    }

    public void RemoveItemFromSlot(InventorySlot slot) 
    {
        if (slot.IsBackpackSlot())
            EquipmentManager.instance.BackpackStorage().RemoveData(slot.transform.GetSiblingIndex());

        if (slot.IsChestSlot())
            PlayerActionManagement.instance.currentTarget?.GetComponent<Storage>()?.RemoveData(slot.transform.GetSiblingIndex());
        
        slot.SetItemInSlot(null);
    }

    public void RemoveItemFromSlot(ItemUI itemToBeRemove)
    {
        if (itemToBeRemove?.transform.parent?.GetComponent<InventorySlot>())
        {
            InventorySlot slotToRemoveFrom = itemToBeRemove.transform.parent.GetComponent<InventorySlot>();
            RemoveItemFromSlot(slotToRemoveFrom);
        }
    }


    public void SwapTwoSlots(InventorySlot slot)
    {
        ItemUI aux1     = slot.GetItemInSlot();
        ItemUI aux2     = selectedItemSlot.GetItemInSlot();

        aux1.transform.SetParent(aux1.transform.parent.parent);

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

    public ItemUI FindSpecificItem(ItemData.Name nameOfItemType)
    {
        foreach (InventorySlot slot in inventorySlots)
            if (slot.CheckIfItHasItem())
                if (slot.CheckMatchingName(nameOfItemType))
                    return slot.GetItemInSlot();

        if (backpack.gameObject.activeInHierarchy) // If player has backpack check it
            foreach (InventorySlot slot in backpackSlots)
                if (slot.CheckIfItHasItem())
                    if (slot.CheckMatchingName(nameOfItemType))
                        return slot.GetItemInSlot();

        if (chest.gameObject.activeInHierarchy) // If player is searching chest check it
            foreach (InventorySlot slot in chestSlots)
                if (slot.CheckIfItHasItem())
                    if (slot.CheckMatchingName(nameOfItemType))
                        return slot.GetItemInSlot();

        return null;
    }

    public int AmountOwned(ItemData.Name nameOfItemType)
    {
        int totalAmount = 0;

        foreach (InventorySlot slot in inventorySlots)
            if (slot.CheckIfItHasItem())
                if (slot.CheckMatchingName(nameOfItemType))
                    totalAmount += slot.GetItemInSlot().GetItemData().currentStack;

        if (backpack.gameObject.activeInHierarchy) // If player has backpack check it
            foreach (InventorySlot slot in backpackSlots)
                if (slot.CheckIfItHasItem())
                    if (slot.CheckMatchingName(nameOfItemType))
                        totalAmount += slot.GetItemInSlot().GetItemData().currentStack;

        if (chest.gameObject.activeInHierarchy) // If player is searching chest check it
            foreach (InventorySlot slot in chestSlots)
                if (slot.CheckIfItHasItem())
                    if (slot.CheckMatchingName(nameOfItemType))
                        totalAmount += slot.GetItemInSlot().GetItemData().currentStack;

        return totalAmount;
    }

    public void SpendResources(ItemData.Name _name, int _amount, bool dontLookForFullStack = true) 
    {
        // First look for partialy full stacks
        foreach (InventorySlot slot in inventorySlots)
            if (slot.CheckIfItHasItem())
                if (slot.CheckMatchingName(_name))
                {
                    if (dontLookForFullStack) // If I'm cheking only partially full stacks
                        if (slot.GetItemInSlot().CheckIfStackIsFull()) // If the stack is full skip it
                            continue;

                    int aux = _amount;
                    _amount -= slot.GetItemInSlot().GetItemData().currentStack; // Substract the amount of resources spent form ccurent stack
                    slot.GetItemInSlot().TakeFromStack(aux); // Take the resources from the slot

                    if (_amount <= 0) // if enough resources have been taken return
                        return;
                }

        if (backpack.gameObject.activeInHierarchy) // If player has backpack check it
            foreach (InventorySlot slot in backpackSlots)
                if (slot.CheckIfItHasItem())
                    if (slot.CheckMatchingName(_name))
                    {
                        if (dontLookForFullStack) // If I'm cheking only partially full stacks
                            if (slot.GetItemInSlot().CheckIfStackIsFull()) // If the stack is full skip it
                                continue;

                        int aux = _amount;
                        _amount -= slot.GetItemInSlot().GetItemData().currentStack; // Substract the amount of resources spent form ccurent stack
                        slot.GetItemInSlot().TakeFromStack(aux); // Take the resources from the slot

                        if (_amount <= 0) // if enough resources have been taken return
                            return;
                    }

        if (chest.gameObject.activeInHierarchy) // If player has backpack check it
            foreach (InventorySlot slot in chestSlots)
                if (slot.CheckIfItHasItem())
                    if (slot.CheckMatchingName(_name))
                    {
                        if (dontLookForFullStack) // If I'm cheking only partially full stacks
                            if (slot.GetItemInSlot().CheckIfStackIsFull()) // If the stack is full skip it
                                continue;

                        int aux = _amount;
                        _amount -= slot.GetItemInSlot().GetItemData().currentStack; // Substract the amount of resources spent form ccurent stack
                        slot.GetItemInSlot().TakeFromStack(aux); // Take the resources from the slot

                        if (_amount <= 0) // if enough resources have been taken return
                            return;
                    }

        if (dontLookForFullStack)
            SpendResources(_name, _amount, false);
    }
    
    public void DisplayBackpack(Storage backpackStorage = null)
    {
        if (backpackStorage == null)
        {
            backpack.gameObject.SetActive(false);

            for (int i = 0; i < backpackSlots.Length; i++)
                if (backpackSlots[i].CheckIfItHasItem())
                    Destroy(backpackSlots[i].GetItemInSlot().gameObject);

            StartCoroutine(CraftingManager.instance.RefreshCraftingManager());
            PopUpManager.instance.ShowMousePopUp();
        }
        else
        {
            backpack.gameObject.SetActive(true);

            StorageData storageData = backpackStorage.GetStorageData();
            if (storageData.size > 8)
            {
                backpack.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 400);
                for (int i = 8; i < backpack.childCount; i++)
                    backpackSlots[i].gameObject.SetActive(true);


            }
            else
            {
                backpack.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 300);
                for (int i = 8; i < backpack.childCount; i++)
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
            chest.gameObject.SetActive(false);

            for (int i = 0; i < chestSlots.Length; i++)
                if (chestSlots[i].CheckIfItHasItem())
                    Destroy(chestSlots[i].GetItemInSlot().gameObject);

            StartCoroutine(CraftingManager.instance.RefreshCraftingManager());
        }
        else
        {
            chest.gameObject.SetActive(true);

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
