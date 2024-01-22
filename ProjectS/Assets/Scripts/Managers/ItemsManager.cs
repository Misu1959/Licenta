using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;

    public GameObject itemUI;
    
    [HideInInspector] public Item[] items;
    [HideInInspector] public Resource[] resources;

    
    
    private void Start()
    {
        instance = this;
        SetPrefabs();
    }

    void SetPrefabs()
    {
        items     = Resources.LoadAll<Item>("Items");
        resources = Resources.LoadAll<Resource>("Res");
    }

    public Item SearchItemsList(Item.Name nameOfItemToFind)
    {
        foreach (Item item in items) 
            if (nameOfItemToFind == item.name)
                return item;

        return null;
    }
    public Resource SearchResourcesList(Resource.Name nameOfItemToFind)
    {
        foreach (Resource resource in resources)
            if (nameOfItemToFind == resource.name)
                return resource;

        return null;
    }


}
