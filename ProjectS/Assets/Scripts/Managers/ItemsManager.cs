using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;
    [SerializeField] private GameObject itemUIPrefab;

    Dictionary<ObjectName, Item>            itemsDictionary         = new Dictionary<ObjectName, Item>();
    Dictionary<ObjectName, Resource>        resourcesDictionary     = new Dictionary<ObjectName, Resource>();
    Dictionary<ObjectName, Construction>    constructionsDictionary = new Dictionary<ObjectName, Construction>();
    Dictionary<ObjectName, MobStats>        mobsDictionary          = new Dictionary<ObjectName, MobStats>();
    Dictionary<ObjectName, MobSpawner>      mobSpawnersDictionary   = new Dictionary<ObjectName, MobSpawner>();
    

    private void Awake()
    {
        instance = this;
        SetDictionaries();

        this.gameObject.SetActive(false);
    }

    private void SetDictionaries()
    {
        foreach (Item item in Resources.LoadAll<Item>("Items"))
            itemsDictionary.Add(item.GetItemData().objectName, item);
        
        foreach (Resource res in Resources.LoadAll<Resource>("Res"))
            resourcesDictionary.Add(res.objectName, res);

        foreach (Construction constr in Resources.LoadAll<Construction>("Constructions"))
            constructionsDictionary.Add(constr.objectName, constr);

        foreach (MobStats mob in Resources.LoadAll<MobStats>("Mobs"))
            mobsDictionary.Add(mob.objectName, mob);

        foreach (MobSpawner mobSpawner in Resources.LoadAll<MobSpawner>("MobSpawners"))
            mobSpawnersDictionary.Add(mobSpawner.objectName, mobSpawner);

    }

    public Item GetOriginalItem(ObjectName nameOfItem) => itemsDictionary.ContainsKey(nameOfItem) ? itemsDictionary[nameOfItem] : null;
    public Resource GetOriginalResource(ObjectName nameOfResource) => resourcesDictionary.ContainsKey(nameOfResource) ? resourcesDictionary[nameOfResource] : null;

    public Construction GetOriginalConstruction(ObjectName nameofConstruction) => constructionsDictionary.ContainsKey(nameofConstruction) ? constructionsDictionary[nameofConstruction] : null;

    public MobStats GetOriginalMob(ObjectName nameOfMob) => mobsDictionary.ContainsKey(nameOfMob) ? mobsDictionary[nameOfMob] : null;


    public MobSpawner GetOriginalMobSpawner(ObjectName nameOfMobSpawner) => mobSpawnersDictionary.ContainsKey(nameOfMobSpawner) ? mobSpawnersDictionary[nameOfMobSpawner] : null;

    public ItemUI CreateItemUI(ObjectName itemName) // Create a completey new itemUI
    {
        GameObject newItemUI = Instantiate(itemUIPrefab);

        Item originalItem = GetOriginalItem(itemName);

        if (originalItem.GetComponent<ItemMaterial>())
            newItemUI.AddComponent<ItemMaterialUI>();
        else if(originalItem.GetComponent<Food>())
            newItemUI.AddComponent<FoodUI>();
        else if (originalItem.GetComponent<Equipment>())
        {
            newItemUI.AddComponent<EquipmentUI>();
            
            if (originalItem.GetEquipmentData().actionType == EquipmentActionType.torch)
                newItemUI.AddComponent<TorchUI>();
        
            if(originalItem.GetComponent<Storage>())
            {
                newItemUI.AddComponent<Storage>();
                newItemUI.GetComponent<Storage>().SetStorageData(originalItem.GetComponent<Storage>().GetStorageData());
            }

        }

        newItemUI.GetComponent<ItemUI>().SetItemData(originalItem.GetItemData());

        
        return newItemUI.GetComponent<ItemUI>();
    }

    public ItemUI CreateItemUI(ItemData itemData, StorageData storageData) // Create a new itemUI based on an old item data 
    {
        GameObject newItemUI = Instantiate(itemUIPrefab);

        Item originalItem = GetOriginalItem(itemData.objectName);

        if (originalItem.GetComponent<ItemMaterial>())
            newItemUI.AddComponent<ItemMaterialUI>();
        else if (originalItem.GetComponent<Food>())
            newItemUI.AddComponent<FoodUI>();
        else if (originalItem.GetComponent<Equipment>())
        {
            newItemUI.AddComponent<EquipmentUI>();

            if (originalItem.GetEquipmentData().actionType == EquipmentActionType.torch)
                newItemUI.AddComponent<TorchUI>();

            if (originalItem.GetComponent<Storage>())
            {
                newItemUI.AddComponent<Storage>();

                if(storageData == null)
                    newItemUI.GetComponent<Storage>().SetStorageData(originalItem.GetComponent<Storage>().GetStorageData());
                else
                    newItemUI.GetComponent<Storage>().SetStorageData(storageData);

            }

        }

        newItemUI.GetComponent<ItemUI>().SetItemData(itemData);


        return newItemUI.GetComponent<ItemUI>();

    }


    public Item CreateItem(ObjectName newItemName) // Create a totaly new item
    {
        Item originalItem = GetOriginalItem(newItemName);
        Item newItem = Instantiate(originalItem);

        newItem.SetItemData(originalItem.GetItemData());

        if (originalItem.GetComponent<Storage>())
            newItem.GetComponent<Storage>().SetStorageData(originalItem.GetComponent<Storage>().GetStorageData());


        return newItem;
    }


    public Item CreateItem(ItemData newData, StorageData storageData) // // Create a new itemUI based on an old item data
    {
        Item originalItem   = GetOriginalItem(newData.objectName);
        Item newItem        = Instantiate(originalItem);

        newItem.SetItemData(newData);

        if (originalItem.GetComponent<Storage>())
        {
            if (storageData == null)
                newItem.GetComponent<Storage>().SetStorageData(originalItem.GetComponent<Storage>().GetStorageData());
            else
                newItem.GetComponent<Storage>().SetStorageData(storageData);
        }

        return newItem;
    }


}
