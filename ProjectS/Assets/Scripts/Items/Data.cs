using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public enum ItemType
    {
        material = 1,
        food = 2,
        equipment = 3
    }

    public enum Name
    {
        empty,
        twigs,
        log,
        flint,
        stone,
        grass,
        charcoal,
        pinecone,
        boards,
        cutstone,
        rope,
        silk,

        seeds,
        seedsC,
        berries,
        berriesC,
        redCap,
        redCapC,
        greenCap,
        greenCapC,
        blueCap,
        blueCapC,
        meat,
        meatC,
        honey,

        axe,
        pickaxe,
        spear,
        torch,
        backpack,
        grassArmor,
        woodArmor
    };


    public Sprite uiImg;

    public Name name;

    public int maxStack;
    [HideInInspector]public int currentStack;

    public int fuelValue;

    public ItemData(ItemData newItemData)
    {
        uiImg           = newItemData.uiImg;
        name            = newItemData.name;
        maxStack        = newItemData.maxStack;
        currentStack    = newItemData.currentStack == 0 ? 1 : newItemData.currentStack;
        fuelValue       = newItemData.fuelValue;
    }

    public virtual ItemType GetItemType() { return ItemType.material; }

}

[Serializable]
public class FoodData : ItemData
{
    public float hungerAmount;
    public float hpAmount;

    public float timeToCook;
    public bool quickEat;

    public Timer timer { get; private set; }



    public FoodData(FoodData newItemData) : base(newItemData)
    {
        hungerAmount    = newItemData.hungerAmount;
        hpAmount        = newItemData.hpAmount;
        timeToCook      = newItemData.timeToCook;
        quickEat        = newItemData.quickEat;
    }

    public override ItemType GetItemType() { return ItemType.food; }
}

[Serializable]
public class EquipmentData : ItemData
{
    public enum Type
    {
        hand = 0,
        body = 1,
        head = 2
    };
    public enum ActionType
    {
        chop,
        mine,
        torch,
        fight,
        storage
    };


    public Type equipmentType;
    public ActionType actionType;


    public float maxDurability;
    [HideInInspector]public float durability;


    public EquipmentData(EquipmentData newItemData) : base(newItemData)
    {
        equipmentType   = newItemData.equipmentType;
        actionType      = newItemData.actionType;
        maxDurability   = newItemData.maxDurability;
        durability      = newItemData.durability == 0 ? maxDurability : newItemData.durability;
    }

    public override ItemType GetItemType() { return ItemType.equipment; }
}

[Serializable]
public class StorageData
{
    public int size;
    public ItemData[] items;
    public StorageData(StorageData newStorageData)
    {
        size = newStorageData.size;

        items = new ItemData[size];
        for (int i = 0; i < newStorageData.items.Length; i++)
            items[i] = new ItemData(newStorageData.items[i]);
    }


}