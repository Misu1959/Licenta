using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum ObjectType
{
    item,
    resource,
    construction,
    mob,
    spawner
}

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
    [InspectorName("Constructions/WoodWall")] woodWall,
    [InspectorName("Constructions/StoneWall")] stoneWall,
    [InspectorName("Constructions/Fire")] fire,
    [InspectorName("Constructions/Campfire")] campfire,
    [InspectorName("Constructions/Chest")] chest,
    [InspectorName("Constructions/ScienceMachine")] scienceMachine,
    [InspectorName("Constructions/BeeBox")] beeBox,
    [InspectorName("Constructions/PigHouse")] pigHouse,

    #endregion

    #region MobSpawners
    [InspectorName(null)] mobSpawners = 30000,
    [InspectorName("MobSpawners/RabbitHole")] rabbitHole,
    [InspectorName("MobSpawners/Beehive")] beehive,
    [InspectorName("MobSpawners/Cacoon")] cocoon,
    [InspectorName("MobSpawners/HoundSpawner")] houndSpawner,



    #endregion

    #region Mobs
    [InspectorName(null)] mobs = 40000,

    [InspectorName("Mobs/Pig")] pig,
    [InspectorName("Mobs/Rabbit")] rabbit,
    [InspectorName("Mobs/Bee")] bee,
    [InspectorName("Mobs/Spider")] spider,
    [InspectorName("Mobs/Hound")] hound
    #endregion
};

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

    public ObjectName objectName;

    public int maxStack;
    public int currentStack;

    public int fuelValue;

    public ItemData(ItemData newItemData)
    {
        objectName      = newItemData.objectName;

        uiImg           = newItemData.uiImg;
        maxStack        = newItemData.maxStack;
        currentStack    = (newItemData.currentStack == 0) ? 1 : newItemData.currentStack;
        fuelValue       = newItemData.fuelValue;
    }

    public ItemData(Sprite _uiImg,ObjectName _objectName,int _maxStack, int _currentStack,int _fuelValue)
    {
        uiImg           = _uiImg;
        objectName      = _objectName;
        maxStack        = _maxStack;
        currentStack    = _currentStack;
        fuelValue       = _fuelValue;
    }

}

[Serializable]
public class FoodData : ItemData
{
    public int hungerAmount;
    public int hpAmount;

    public bool quickEat;

    public bool canBeCoocked;
    public Timer cookTimer;

    public FoodData(FoodData newItemData) : base(newItemData)
    {
        hungerAmount    = newItemData.hungerAmount;
        hpAmount        = newItemData.hpAmount;
        quickEat        = newItemData.quickEat;
        canBeCoocked    = newItemData.canBeCoocked;

        if (canBeCoocked)
            cookTimer = new Timer(newItemData.cookTimer.MaxTime());
    }
    public FoodData(Sprite _uiImg, ObjectName _objectName, int _maxStack, int _currentStack, int _fuelValue)
        : base(_uiImg, _objectName, _maxStack, _currentStack, _fuelValue) { }

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

    public EquipmentData(Sprite _uiImg, ObjectName _objectName, int _maxStack, int _currentStack, int _fuelValue,int _durability)
        : base(_uiImg, _objectName, _maxStack, _currentStack, _fuelValue)
    {
        durability = _durability;
    }

    public bool HasStorageData() => (actionType == EquipmentActionType.storage) ? true : false;
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
            if (newStorageData.items[i] != null)
                if (newStorageData.items[i].objectName != ObjectName.empty)
                {
                    Item originalItem = ItemsManager.instance.GetOriginalItem(newStorageData.items[i].objectName);

                    if (originalItem.GetComponent<ItemMaterial>())
                        items[i] = new ItemData(newStorageData.items[i]);
                    else if (originalItem.GetComponent<Food>())
                        items[i] = new FoodData((FoodData)newStorageData.items[i]);
                    else if (originalItem.GetComponent<Equipment>())
                        items[i] = new EquipmentData((EquipmentData)newStorageData.items[i]);
                }
    }


}