using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;
    [SerializeField] private GameObject itemUIPrefab;

    private Item[] items;
    private Resource[] resources;
    private MobStats[] mobs;
    
    private void Start()
    {
        instance = this;
        SetPrefabs();
    }

    private void SetPrefabs()
    {
        items     = Resources.LoadAll<Item>("Items");
        resources = Resources.LoadAll<Resource>("Res");
        mobs = Resources.LoadAll<MobStats>("Mobs");
    }

    public Item SearchItemsList(ObjectName nameOfItemToFind)
    {
        foreach (Item item in items) 
            if (item.GetItemData().name == nameOfItemToFind)
                return item;

        return null;
    }
    public Resource SearchResourcesList(ObjectName nameOfResourceToFind)
    {
        foreach (Resource resource in resources)
            if (resource.name == nameOfResourceToFind)
                return resource;

        return null;
    }

    public MobStats SearchMobsList(ObjectName nameOfMobToFind)
    {
        foreach (MobStats mob in mobs)
            if (mob.name == nameOfMobToFind)
                return mob;

        return null;
    }

    public ItemUI CreateItemUI(Item item) // Create a new itemUI based on an Item
    {
        GameObject newItemUI = Instantiate(itemUIPrefab);

        switch (item.GetItemData().GetItemType())
        {
            case ItemType.material:
            {
                newItemUI.AddComponent<ItemMaterialUI>();
                break;
            }
            case ItemType.food:
            {
                newItemUI.AddComponent<FoodUI>();
                break;
            }
            case ItemType.equipment:
            {
                newItemUI.AddComponent<EquipmentUI>();

                if (item.GetEquipmentData().actionType == EquipmentActionType.torch)
                    newItemUI.AddComponent<TorchUI>();

                break;
            }
        }
        newItemUI.GetComponent<ItemUI>().SetItemData(item.GetItemData());

        if (item.GetComponent<Storage>())
        {
            newItemUI.AddComponent<Storage>();
            newItemUI.GetComponent<Storage>().SetStorageData(item.GetComponent<Storage>().GetStorageData());
        }

        return newItemUI.GetComponent<ItemUI>();
    }

    public ItemUI CreateItemUI(ItemData newData, int amount = -1) // Create a new itemUI based on an ItemUI data
    {
        GameObject newItemUI = Instantiate(itemUIPrefab);

        switch (newData.GetItemType())
        {
            case ItemType.material:
                {
                    newItemUI.AddComponent<ItemMaterialUI>();
                    break;
                }
            case ItemType.food:
                {
                    newItemUI.AddComponent<FoodUI>();
                    break;
                }
            case ItemType.equipment:
                {
                    newItemUI.AddComponent<EquipmentUI>();
                    break;
                }
        }
        newItemUI.GetComponent<ItemUI>().SetItemData(newData);

        if(amount != -1)
            newItemUI.GetComponent<ItemUI>().GetItemData().currentStack = amount;

        return newItemUI.GetComponent<ItemUI>();

    }

    public Item CreateItem(ItemUI item) // Create a new item based on an ui Item
    {
        Item newItem = Instantiate(SearchItemsList(item.GetItemData().name)).GetComponent<Item>();
        newItem.SetItemData(item.GetItemData());

        if (item.GetComponent<Storage>())
            newItem.GetComponent<Storage>().SetStorageData(item.GetComponent<Storage>().GetStorageData());

        return newItem;
    } 

    public Item CreateItem(ObjectName newItemName) // Create a totaly new item
    {
        Item oldItem = SearchItemsList(newItemName);
        Item newItem = Instantiate(oldItem).GetComponent<Item>();
        newItem.SetItemData(oldItem.GetItemData());

        if(newItem.GetComponent<Storage>())
            newItem.GetComponent<Storage>().SetStorageData(oldItem.GetComponent<Storage>().GetStorageData());

        return newItem;
    }

}
