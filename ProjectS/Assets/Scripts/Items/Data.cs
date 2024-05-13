using System;
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
    [SerializeField] private Sprite _uiImg;
    public Sprite uiImg 
    {
        get  => _uiImg; 
        private set => _uiImg = value;
    }


    [SerializeField] private ObjectName _objectName;
    public ObjectName objectName
    {
        get => _objectName;
        private set => _objectName = value;
    }


    [SerializeField] private int _maxStack;
    public int maxStack 
    {
        get=> _maxStack;
        private set => _maxStack = value;
    }


    [SerializeField] private int _fuelValue;
    public int fuelValue
    {
        get => _fuelValue;
        private set => _fuelValue = value;
    }


    public int currentStack { get; private set; }
    public void SetCurrentStack(int newStackSize) => currentStack = newStackSize;

    public ItemData(ItemData newItemData)
    {
        objectName      = newItemData.objectName;

        uiImg           = newItemData.uiImg;
        maxStack        = newItemData.maxStack;
        currentStack    = (newItemData.currentStack == 0) ? 1 : newItemData.currentStack;
        fuelValue       = newItemData.fuelValue;
    }

    public ItemData(ObjectName _objectName, int _currentStack)
    {
        Item originalItme = ItemsManager.instance.GetOriginalItem(_objectName);
            
        objectName      = _objectName;
        currentStack    = _currentStack;

        uiImg           = originalItme.GetItemData().uiImg;
        maxStack        = originalItme.GetItemData().maxStack;
        fuelValue       = originalItme.GetItemData().fuelValue;
    }

}

[Serializable]
public class FoodData : ItemData
{
    [SerializeField] private int _hungerAmount;
    public int hungerAmount 
    {
        get => _hungerAmount;
        private set=> _hungerAmount = value;
    }
    

    [SerializeField] private int _hpAmount;
    public int hpAmount
    {
        get => _hpAmount;
        private set=> _hpAmount = value;
    }


    [SerializeField] private bool _quickEat;
    public bool quickEat
    {
        get => _quickEat;
        private set => _quickEat = value;
    }


    [SerializeField] private bool _canBeCooked;
    public bool canBeCooked
    {
        get => _canBeCooked; 
        private set => _canBeCooked = value;
    }

    
    [SerializeField] private Timer _cookTimer;
    public Timer cookTimer
    {
        get => _cookTimer; 
        private set => _cookTimer = value;
    }


    public FoodData(FoodData newItemData) : base(newItemData)
    {
        hungerAmount    = newItemData.hungerAmount;
        hpAmount        = newItemData.hpAmount;
        quickEat        = newItemData.quickEat;
        canBeCooked     = newItemData.canBeCooked;

        if (canBeCooked)
            cookTimer = new Timer(newItemData.cookTimer.MaxTime());
    }
    public FoodData(ObjectName _objectName, int _currentStack)
        : base(_objectName, _currentStack) 
    {
        Item originalItme = ItemsManager.instance.GetOriginalItem(_objectName);

        hungerAmount    = originalItme.GetFoodData().hungerAmount;
        hpAmount        = originalItme.GetFoodData().hpAmount;
        quickEat        = originalItme.GetFoodData().quickEat;
        canBeCooked     = originalItme.GetFoodData().canBeCooked;

        if (canBeCooked)
            cookTimer = new Timer(originalItme.GetFoodData().cookTimer.MaxTime());
    }

}

[Serializable]
public class EquipmentData : ItemData
{
    [SerializeField] private EquipmentType _equipmentType;
    public EquipmentType equipmentType
    {
        get => _equipmentType;
        private set => _equipmentType = value;
    }


    [SerializeField] private EquipmentActionType _actionType;
    public EquipmentActionType actionType
    {
        get => _actionType;
        private set => _actionType = value;
    }


    [SerializeField] private int _dmg;
    public int dmg 
    {
        get => _dmg;
        private set => _dmg = value;
    }
    
    
    [SerializeField] private int _maxDurability;
    public int maxDurability
    {
        get => _maxDurability; 
        private set => _maxDurability = value;
    }


    [HideInInspector] private int _durability;
    public int durability
    {
        get => _durability; 
        private set => _durability = value;
    }
    public void SetDurability(int newDurability) => durability = newDurability;


    public EquipmentData(EquipmentData newItemData) : base(newItemData)
    {
        equipmentType   = newItemData.equipmentType;
        actionType      = newItemData.actionType;
            
        dmg             = newItemData.dmg;  
        maxDurability   = newItemData.maxDurability;
        durability      = (newItemData.durability == 0) ? newItemData.maxDurability : newItemData.durability;

    }

    public EquipmentData(ObjectName _objectName, int _currentStack,int _durability)
        : base(_objectName, _currentStack)
    {
        Item originalItme = ItemsManager.instance.GetOriginalItem(_objectName);

        durability = _durability;

        equipmentType   = originalItme.GetEquipmentData().equipmentType;
        actionType      = originalItme.GetEquipmentData().actionType;
        dmg             = originalItme.GetEquipmentData().dmg;
        maxDurability   = originalItme.GetEquipmentData().maxDurability;
    
    
    }

}



[Serializable]
public class StorageData
{
    [SerializeField] private ItemData[] items;

    public StorageData(StorageData newStorageData)
    {
        items = new ItemData[newStorageData.GetSize()];
        for (int i = 0; i < newStorageData.items.Length; i++)
            if (newStorageData.HasElement(i))
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

    public int GetSize() => items.Length;
    public ItemData[] GetList() => items;
    public ItemData GetElement(int pos) => items[pos];


    public bool HasElement(int pos)
    {
        if (items[pos] == null) return false;
        if (items[pos].objectName == ObjectName.empty) return false;

        return true;
    }

    public void AddData(ItemData dataToAdd, int posToAddAt) => items[posToAddAt] = dataToAdd;

    public void RemoveData(int posToRemoveAt) => items[posToRemoveAt] = null;

}