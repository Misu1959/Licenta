using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;

    public GameObject itemUI;
    
    [HideInInspector] public GameObject[] items;
    [HideInInspector] public GameObject[] resources;

    
    
    private void Start()
    {
        instance = this;
        SetPrefabs();
    }

    void SetPrefabs()
    {
        items     = Resources.LoadAll<GameObject>("Items");
        resources = Resources.LoadAll<GameObject>("Res");
    }

    public GameObject SearchItemsList(string typeOfItemToFind)
    {
        foreach (GameObject item in items) 
            if (typeOfItemToFind == item.name)
                return item.gameObject;

        return null;
    }
    public GameObject SearchResourcesList(string typeOfItemToFind)
    {
        foreach (GameObject resource in resources)
            if (typeOfItemToFind == resource.name)
                return resource;

        return null;
    }


}
