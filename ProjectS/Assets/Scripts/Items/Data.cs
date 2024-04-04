using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum ObjectName
{

    empty,

    #region Items/Materials
    [InspectorName(null)] items_materials = 1000,
    [InspectorName("Items/Materials/Twigs")] twigs,
    [InspectorName("Items/Materials/Logs")] log,
    [InspectorName("Items/Materials/Flint")] flint,
    [InspectorName("Items/Materials/Stone")] stone,
    [InspectorName("Items/Materials/Grass")] grass,
    [InspectorName("Items/Materials/Charcoal")] charcoal,
    [InspectorName("Items/Materials/Pinecone")] pinecone,
    [InspectorName("Items/Materials/Boards")] boards,
    [InspectorName("Items/Materials/Cutstone")] cutstone,
    [InspectorName("Items/Materials/Rope")] rope,
    [InspectorName("Items/Materials/Silk")] silk,
    [InspectorName("Items/Materials/Gold")] gold,
    [InspectorName("Items/Materials/Pig Skin")] pigSkin,

    #endregion

    #region Items/Food
    [InspectorName(null)] items_food = 2000,

    [InspectorName("Items/Food/Seeds")] seeds,
    [InspectorName("Items/Food/Seeds coocked")] seedsC,
    [InspectorName("Items/Food/Berries")] berries,
    [InspectorName("Items/Food/Berries coocked")] berriesC,
    [InspectorName("Items/Food/Red mushroom")] redCap,
    [InspectorName("Items/Food/Red mushroom coocked")] redCapC,
    [InspectorName("Items/Food/Green mushroom")] greenCap,
    [InspectorName("Items/Food/Green mushroom coocked")] greenCapC,
    [InspectorName("Items/Food/Blue mushroom")] blueCap,
    [InspectorName("Items/Food/Blue mushroom coocked")] blueCapC,
    [InspectorName("Items/Food/Meat")] meat,
    [InspectorName("Items/Food/Meat coocked")] meatC,
    [InspectorName("Items/Food/Honey")] honey,
    #endregion

    #region Items/Equipment
    [InspectorName(null)] items_equipment = 3000,

    [InspectorName("Items/Equipment/Hand/Axe")] axe,
    [InspectorName("Items/Equipment/Hand/Pickaxe")] pickaxe,
    [InspectorName("Items/Equipment/Hand/Spear")] spear,
    [InspectorName("Items/Equipment/Hand/Torch")] torch,
    [InspectorName("Items/Equipment/Hand/Gold Axe")] goldAxe,
    [InspectorName("Items/Equipment/Hand/Gold Pickaxe")] goldPickaxe,
    [InspectorName("Items/Equipment/Body/Backpack")] backpack,
    [InspectorName("Items/Equipment/Body/Grass Armor")] grassArmor,
    [InspectorName("Items/Equipment/Body/Wood Armor")] woodArmor,
    [InspectorName("Items/Equipment/Body/Piggy Backpack")] piggyBackpack,

    #endregion

    #region Resources
    [InspectorName(null)] resources = 10000,

    [InspectorName("Resources/Berry Bush")] berryBush,
    [InspectorName("Resources/Grass Bush")] grassBush,
    [InspectorName("Resources/Sappling")] sappling,
    [InspectorName("Resources/Rock")] rock,
    [InspectorName("Resources/Rock Gold")] rockGold,
    [InspectorName("Resources/Tree")] tree,
    [InspectorName("Resources/Red Mushroom")] redShroom,
    [InspectorName("Resources/Green Mushroom")] greenShroom,
    [InspectorName("Resources/Blue Mushroom")] blueShroom,

    #endregion

    #region Constructions
    [InspectorName(null)] constructions = 20000,

    #endregion

    #region MobSpawners
    [InspectorName(null)] mobSpawners = 30000,

    #endregion

    #region Mobs
    [InspectorName(null)] mobs = 40000,

    [InspectorName("Mobs/Pig")] pig,
    [InspectorName("Mobs/Rabbit")] rabbit,
    [InspectorName("Mobs/Bee")] bee,
    [InspectorName("Mobs/Hound")] hound,
    [InspectorName("Mobs/Spider")] spider
    #endregion
};

public enum ItemType
{
    material,
    food,
    equipment
}

public enum EquipmentType
{
    hand,
    body,
    head
};
public enum EquipmentActionType
{
    chop,
    mine,
    torch,
    fight,
    storage
};



[Serializable]
public class ItemData
{
    public Sprite uiImg;

    public ObjectName name;

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
    public int hungerAmount;
    public int hpAmount;

    public bool quickEat;

    public Timer cookTimer;

    public FoodData(FoodData newItemData) : base(newItemData)
    {
        hungerAmount    = newItemData.hungerAmount;
        hpAmount        = newItemData.hpAmount;
        quickEat        = newItemData.quickEat;
    }

    public override ItemType GetItemType() { return ItemType.food; }
}

[Serializable]
public class EquipmentData : ItemData
{

    public EquipmentType equipmentType;
    public EquipmentActionType actionType;

    public int dmg;

    public int maxDurability;
    [HideInInspector] public int durability;


    public EquipmentData(EquipmentData newItemData) : base(newItemData)
    {
        equipmentType   = newItemData.equipmentType;
        actionType      = newItemData.actionType;
            
        dmg             = newItemData.dmg;  
        maxDurability   = newItemData.maxDurability;
        durability      = newItemData.durability == 0 ? newItemData.maxDurability : newItemData.durability;

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