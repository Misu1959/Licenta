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

    Dictionary<ObjectName, Item>        itemsDictionary         = new Dictionary<ObjectName, Item>();
    Dictionary<ObjectName, Resource>    resourcesDictionary     = new Dictionary<ObjectName, Resource>();
    Dictionary<ObjectName, MobStats>    mobsDictionary          = new Dictionary<ObjectName, MobStats>();
    Dictionary<ObjectName, MobSpawner>  mobSpawnersDictionary   = new Dictionary<ObjectName, MobSpawner>();
    

    private void Start()
    {
        instance = this;
        SetDictionaries();
    }

    private void SetDictionaries()
    {
        foreach (Item item in Resources.LoadAll<Item>("Items"))
            itemsDictionary.Add(item.GetItemData().objectName, item);
        
        foreach (Resource res in Resources.LoadAll<Resource>("Res"))
            resourcesDictionary.Add(res.objectName, res);

        foreach (MobStats mob in Resources.LoadAll<MobStats>("Mobs"))
            mobsDictionary.Add(mob.objectName, mob);

        foreach (MobSpawner mobSpawner in Resources.LoadAll<MobSpawner>("MobSpawners"))
            mobSpawnersDictionary.Add(mobSpawner.objectName, mobSpawner);

    }

    public Item GetOriginalItem(ObjectName nameOfItem) => itemsDictionary.ContainsKey(nameOfItem) ? itemsDictionary[nameOfItem] : null;
    public Resource GetOriginalResource(ObjectName nameOfResource) => resourcesDictionary.ContainsKey(nameOfResource) ? resourcesDictionary[nameOfResource] : null;

    public MobStats GetOriginalMob(ObjectName nameOfMob) => mobsDictionary.ContainsKey(nameOfMob) ? mobsDictionary[nameOfMob] : null;


    public MobSpawner GetOriginalMobSpawner(ObjectName nameOfMobSpawner) => mobSpawnersDictionary.ContainsKey(nameOfMobSpawner) ? mobSpawnersDictionary[nameOfMobSpawner] : null;
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
        Item newItem = Instantiate(GetOriginalItem(item.GetItemData().objectName));
        newItem.SetItemData(item.GetItemData());

        if (item.GetComponent<Storage>())
            newItem.GetComponent<Storage>().SetStorageData(item.GetComponent<Storage>().GetStorageData());

        return newItem;
    } 

    public Item CreateItem(ObjectName newItemName) // Create a totaly new item
    {
        Item oldItem = GetOriginalItem(newItemName);
        Item newItem = Instantiate(oldItem).GetComponent<Item>();
        newItem.SetItemData(oldItem.GetItemData());

        if(newItem.GetComponent<Storage>())
            newItem.GetComponent<Storage>().SetStorageData(oldItem.GetComponent<Storage>().GetStorageData());

        return newItem;
    }

}
