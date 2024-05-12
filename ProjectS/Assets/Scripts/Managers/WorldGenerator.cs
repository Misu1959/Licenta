using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

using static SaveLoadManager;
using System.IO;
using static UnityEditor.Progress;
using Mono.Cecil;

public class WorldGenerator : MonoBehaviour
{
    private static System.Random rnd = new System.Random();

    public static WorldGenerator instance;
    void Awake()
    {
        instance = this;
        if (Get_Old_World_Current() == 0)
            StartCoroutine(GenerateWorld());
        else
            StartCoroutine(LoadWorld());

        Set_Old_World_Current(1);
    }

    void FinishGenerating()
    {
        PlayerStats.instance.gameObject.SetActive(true);

        UIManager.instance.gameObject.SetActive(true);
        InventoryManager.instance.gameObject.SetActive(true);
        EquipmentManager.instance.gameObject.SetActive(true);
        InteractionManager.instance.gameObject.SetActive(true);
        PopUpManager.instance.gameObject.SetActive(true);
        TimeManager.instance.gameObject.SetActive(true);
        ItemsManager.instance.gameObject.SetActive(true);
        WorldManager.instance.gameObject.SetActive(true);

        StartCoroutine(CraftingManager.instance.RefreshCraftingMenu());
    }

    #region Generate

    private IEnumerator GenerateWorld()
    {
        Debug.Log("Generate world");
        yield return null;

        PlayerStats.instance.SetStats();
        SpawnObjects();

        SetTimeSettings();
        FinishGenerating();

        SaveWorld();
    }

    private void SetTimeSettings()
    {
        int dayDuration = Get_Day_Duration();
        int dayLength   = Get_Day_Length();
        int nightLength = Get_Night_Length();
        
        TimeManager.instance.SetTimeSettings(dayDuration, dayLength, nightLength);
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < Get_Spawn_Setting_Amount(); i++)
        {
            ObjectName objectName = (ObjectName)Get_Spawn_Setting_Name(i);
            int objectSpawnValue = Get_Spawn_Setting_Value(i);

            Spawn(objectName, objectSpawnValue);
        }
    }
    private void Spawn(ObjectName objectName, int objectSpawnValue)
    {
        if (ItemsManager.instance.GetOriginalItem(objectName) != null)
            SpawnItems(objectName, objectSpawnValue);
        else if (ItemsManager.instance.GetOriginalResource(objectName) != null)
            SpawnResources(objectName, objectSpawnValue);
        else if (ItemsManager.instance.GetOriginalMobSpawner(objectName) != null)
            SpawnMobSpawners(objectName, objectSpawnValue);
    }
    private void SpawnItems(ObjectName objectName, int objectSpawnValue)
    {
        int nr = rnd.Next((int)(objectSpawnValue * .75f), (int)(objectSpawnValue * 1.25f));
        int worldSize = Get_World_Size();

        for (int i = 0; i < nr; i++)
        {
            Item item = ItemsManager.instance.CreateItem(objectName);

            item.transform.SetParent(WorldManager.instance.items);
            item.transform.position = new Vector3(Random.Range(-1 * worldSize, worldSize), 0, Random.Range(-1 * worldSize, worldSize));

        }
    }
    private void SpawnResources(ObjectName objectName, int objectSpawnValue)
    {
        int nr = rnd.Next((int)(objectSpawnValue * .75f), (int)(objectSpawnValue * 1.25f));
        int worldSize = Get_World_Size();

        for (int i = 0; i < nr; i++)
        {
            Resource resource = Instantiate(ItemsManager.instance.GetOriginalResource(objectName));

            resource.transform.SetParent(WorldManager.instance.resources);
            resource.transform.position = new Vector3(Random.Range(-1 * worldSize, worldSize), 0, Random.Range(-1 * worldSize, worldSize));

            resource.GetComponent<Resource>().SetResourceData();
        }
    }
    private void SpawnMobSpawners(ObjectName objectName, int objectSpawnValue)
    {
        int nr = rnd.Next((int)(objectSpawnValue * .75f), (int)(objectSpawnValue * 1.25f));
        int worldSize = Get_World_Size();

        for (int i = 0; i < nr; i++)
        {
            MobSpawner mobSpawner = Instantiate(ItemsManager.instance.GetOriginalMobSpawner(objectName));

            mobSpawner.transform.SetParent(WorldManager.instance.mobSpawners);
            mobSpawner.transform.position = new Vector3(Random.Range(-1 * worldSize, worldSize), 0, Random.Range(-1 * worldSize, worldSize));

            mobSpawner.GetComponent<ComplexMobSpawner>().SetSpawnerData();
        }
    }
    #endregion


    #region Load
    private IEnumerator LoadWorld()
    {
        Debug.Log("Load world");
        yield return null;

        LoadPlayer();
        LoadInventory();
        LoadEquipment();

        LoadItems();
        LoadResources();
        LoadConstructions();
        LoadSpawners();

        LoadMobs(); // needs to be loaded after the spawners

        LoadTimeSettings();
        FinishGenerating();
    }

    void LoadTimeSettings()
    {
        int dayDuration = Get_Day_Duration();
        int dayLength   = Get_Day_Length();
        int nightLength = Get_Night_Length();

        int currentTime     = Get_Current_Time();
        int currentHour     = Get_Current_Hour();
        int currentDay      = Get_Current_Day();
        int currentDayState = Get_Current_DayState();

        TimeManager.instance.SetTimeSettings(dayDuration, dayLength, nightLength, currentTime, currentHour, currentDay, currentDayState);

    }

    void LoadPlayer()
    {
        int _hp         = Get_Object_Hp(Paths.Player, 1);
        int _hunger     = Get_Object_Hunger(Paths.Player, 1);
        int _speed      = Get_Object_Speed(Paths.Player, 1);
        Vector3 _pos    = Get_Object_Pos(Paths.Player, 1);


        PlayerStats.instance.SetStats(_hp, _hunger , _speed,  _pos);
    }


    void LoadInventory()
    {

        for (int i = 0; i < Get_Amount_Of_Objects(Paths.InventoryItem); i++)
        {
            ObjectName name = Get_Object_Name(Paths.InventoryItem, i);
            int curentStack = Get_ITEM_Stack(Paths.InventoryItem,i);

            Item originalItem = ItemsManager.instance.GetOriginalItem(name);
            ItemData data = null;
            StorageData storageData = null;

            if (originalItem.GetComponent<ItemMaterial>())
            {
                data = new ItemData(name, curentStack);

                //Load material data
            }
            else if (originalItem.GetComponent<Food>())
            {
                data = new FoodData(name, curentStack);
                // No new data to be loaded for food at the moment
            }
            else if (originalItem.GetComponent<Equipment>())
            {
                int durability = Get_ITEM_Durability(Paths.InventoryItem, i);
                data = new EquipmentData(name, curentStack, durability);


                if (originalItem.GetComponent<Storage>())
                {
                    // load backpack storage info
                }

                //Load equipment data
            }

            int slotNr = Get_ITEM_UI_Parent_Slot(Paths.InventoryItem, i);

            ItemUI itemUI = ItemsManager.instance.CreateItemUI(data, storageData);
            InventoryManager.instance.AddItemToInventorySlot(slotNr, itemUI);
        }
        // Load Selected item and drop it on the ground
        if (!Get_Has_Selected_Item()) return;

        ObjectName selectedItemName = Get_Object_Name(Paths.SelectedItem, 1);
        int selectedItemStack = Get_ITEM_Stack(Paths.SelectedItem, 1);

        Item originalSelectedItem = ItemsManager.instance.GetOriginalItem(selectedItemName);
        ItemData selectedItemData = null;
        StorageData selectedItemStorageData = null;

        if (originalSelectedItem.GetComponent<ItemMaterial>())
        {
            selectedItemData = new ItemData(selectedItemName, selectedItemStack);

            //Load material data
        }
        else if (originalSelectedItem.GetComponent<Food>()) // Save food info
        {
            selectedItemData = new FoodData(selectedItemName, selectedItemStack);

            // For now there is no data that needs to be saved
        }
        else if (originalSelectedItem.GetComponent<Equipment>()) // Save equipment info
        {

            int selectedItemDurability = Get_ITEM_Durability(Paths.SelectedItem, 1); 

            selectedItemData = new EquipmentData(selectedItemName, selectedItemStack, selectedItemDurability);
        }

        ItemUI selectedItemUI = ItemsManager.instance.CreateItemUI(selectedItemData, selectedItemStorageData);

        InventoryManager.instance.DropItem(selectedItemUI,PlayerBehaviour.instance.transform.position);
    }

    void LoadEquipment()
    {
        ItemUI handitem;
        ItemUI bodyitem;
        ItemUI headitem;

        ItemData data           = null;
        StorageData storageData = null;

        if (Get_Has_Equipment(EquipmentType.hand))
        {
            ObjectName name = Get_Object_Name(Paths.Equipment, (int)EquipmentType.hand);
            int stack       = Get_ITEM_Stack(Paths.Equipment,  (int)EquipmentType.hand);
            int durability  = Get_ITEM_Durability(Paths.Equipment, (int)EquipmentType.hand);

            data = new EquipmentData(name, stack, durability);

            handitem = ItemsManager.instance.CreateItemUI(data, storageData);
            EquipmentManager.instance.SetEquipment(handitem, false, false);

        }
        if (Get_Has_Equipment(EquipmentType.body))
        {
            ObjectName name = Get_Object_Name(Paths.Equipment, (int)EquipmentType.body);
            int stack       = Get_ITEM_Stack(Paths.Equipment, (int)EquipmentType.body);
            int durability  = Get_ITEM_Durability(Paths.Equipment, (int)EquipmentType.body);

            data = new EquipmentData(name, stack, durability);

            bodyitem = ItemsManager.instance.CreateItemUI(data, storageData);
            EquipmentManager.instance.SetEquipment(bodyitem, false, false);
        }
        if (Get_Has_Equipment(EquipmentType.head))
        {
            ObjectName name = Get_Object_Name(Paths.Equipment, (int)EquipmentType.head);
            int stack       = Get_ITEM_Stack(Paths.Equipment, (int)EquipmentType.head);
            int durability  = Get_ITEM_Durability(Paths.Equipment, (int)EquipmentType.head);

            data = new EquipmentData(name, stack, durability);
            headitem = ItemsManager.instance.CreateItemUI(data, storageData);
            EquipmentManager.instance.SetEquipment(headitem, false, false);

        }
    }


    void LoadItems()
    {
        for (int i = 0; i < Get_Amount_Of_Objects(Paths.Item); i++)
        {
            ObjectName _name = Get_Object_Name(Paths.Item, i);
            int _curentStack = Get_ITEM_Stack(Paths.Item, i);

            
            Item originalItem = ItemsManager.instance.GetOriginalItem(_name);
            ItemData data = null;
            StorageData storageData = null;



            if (originalItem.GetComponent<ItemMaterial>())
            {
                data = new ItemData(_name, _curentStack);
                
                //Load material data
            }
            else if (originalItem.GetComponent<Food>())
            {
                data = new FoodData(_name, _curentStack);
                // No new data to be loaded for food at the moment
            }
            else if (originalItem.GetComponent<Equipment>())
            {
                int _durability = Get_ITEM_Durability(Paths.Item, (int)i);
                data = new EquipmentData(_name, _curentStack, _durability);

                //Load equipment data
            }

            Item item = ItemsManager.instance.CreateItem(data, storageData);

            item.transform.SetParent(WorldManager.instance.items);
            item.transform.position = Get_Object_Pos(Paths.Item, i);
        }
    }
    void LoadResources()
    {
        for (int i = 0; i < Get_Amount_Of_Objects(Paths.Resource); i++)
        {
            Resource resource = Instantiate(ItemsManager.instance.GetOriginalResource(Get_Object_Name(Paths.Resource, i)));

            resource.transform.SetParent(WorldManager.instance.resources);
            resource.transform.position = Get_Object_Pos(Paths.Resource, i);

            float growTimer_RemainedTime = Get_RESOURCE_Time_To_Grow(i);
            int res_hp = Get_Object_Hp(Paths.Resource, i);

            resource.SetResourceData(growTimer_RemainedTime, res_hp);
        }

    }

    void LoadConstructions()
    {
        for (int i = 0; i < Get_Amount_Of_Objects(Paths.Construction); i++)
        {
            Construction construction = Instantiate(ItemsManager.instance.GetOriginalConstruction(Get_Object_Name(Paths.Construction, i)));

            construction.transform.SetParent(WorldManager.instance.constructions);
            construction.transform.position = Get_Object_Pos(Paths.Construction, i);

            float fireTimer_RemainedTime = Get_CONSTRUCTION_Fire_Time(i);

            if (construction.GetComponent<Fireplace>())
                construction.GetComponent<Fireplace>().SetFireData(fireTimer_RemainedTime);

            //if (construction.GetComponent<Storage>())
             //   construction.GetComponent<Storage>().SetStorageData(Get_Construction_Storage_Data(i));
        }
    }

    void LoadMobs()
    {
        for (int i = 0; i < Get_Amount_Of_Objects(Paths.Mob); i++)
        {
            MobStats mob = Instantiate(ItemsManager.instance.GetOriginalMob(Get_Object_Name(Paths.Mob, i)));

            mob.transform.SetParent(WorldManager.instance.mobs);
            mob.transform.position = Get_Object_Pos(Paths.Mob, i);

            float spawnerIndex = Get_MOB_Spawner_Index(i);

            Transform _spawner;
            if((int)spawnerIndex != spawnerIndex)
                _spawner = (spawnerIndex == -1) ? null : WorldManager.instance.constructions.GetChild((int)spawnerIndex); 
            else
                _spawner = (spawnerIndex == -1) ? null : WorldManager.instance.mobSpawners.GetChild((int)spawnerIndex);

            int _hp = Get_Object_Hp(Paths.Mob, i);

            mob.SetMobData(_spawner, _hp);
        }
    }

    void LoadSpawners()
    {
        for (int i = 0; i < Get_Amount_Of_Objects(Paths.Spawner); i++)
        {
            MobSpawner spawner = Instantiate(ItemsManager.instance.GetOriginalMobSpawner(Get_Object_Name(Paths.Spawner, i)));

            spawner.transform.SetParent(WorldManager.instance.mobSpawners);
            spawner.transform.position = Get_Object_Pos(Paths.Spawner, i);

            int mobsToSpawn                 = Get_SPAWNER_Mobs_Amount(i);
            float respawnTimer_RemainedTime = Get_SPAWNER_Mobs_Respawn_Time(i);
            
            spawner.SetSpawnerData(mobsToSpawn,respawnTimer_RemainedTime);
        }
    }

    #endregion


    #region Save

    public void SaveWorld()
    {
        Debug.Log("Save world");

        SaveTime();

        SavePlayer();
        SaveInventory();
        SaveEquipment();

        SaveItems();
        SaveResources();
        SaveConstructions();
        SaveMobs();
        SaveSpawners();
    }

    void SaveTime()
    {
        Set_Current_Time();
        Set_Current_Hour();
        Set_Current_Day();
        Set_Current_DayState();
    }

    void SavePlayer()
    {
        Set_Object_Hp(Paths.Player, 1, PlayerStats.instance.hp);
        Set_Object_Hunger(Paths.Player, 1, PlayerStats.instance.hunger);
        Set_Object_Speed(Paths.Player, 1, PlayerStats.instance.speed);
        Set_Object_Pos(Paths.Player, 1, PlayerStats.instance.transform.position);

        //SaveInventory();
        // Save equipment
    }

    void SaveInventory()
    {
        int nrOfItemsInInventory = 0;
        for (int i = 0; i < 15; i++)
            if (InventoryManager.instance.inventoryPanel.GetChild(i).childCount > 0)
            {
                ItemUI itemUI = InventoryManager.instance.inventoryPanel.GetChild(i).GetComponent<InventorySlot>().GetItemInSlot();

                Set_ITEM_UI_Parent_Slot(Paths.InventoryItem, nrOfItemsInInventory, i);

                Set_Object_Name(Paths.InventoryItem,nrOfItemsInInventory, itemUI.GetItemData().objectName);
                Set_ITEM_Stack(Paths.InventoryItem, nrOfItemsInInventory, itemUI.GetItemData().currentStack);

                if (itemUI.GetComponent<FoodUI>()) // Save food info
                {
                    // For now there is no data that needs to be saved
                }
                else if (itemUI.GetComponent<EquipmentUI>()) // Save equipment info
                {
                    Set_ITEM_Durability(Paths.InventoryItem, nrOfItemsInInventory, itemUI.GetEquipmentData().durability);
                }

                nrOfItemsInInventory++;
            }
        Set_Amount_Of_Objects(Paths.InventoryItem, nrOfItemsInInventory);
        // Save selected item
        
        ItemUI selectedItemUI = InventoryManager.instance.selectedItemSlot.GetItemInSlot();
        Set_Has_Selected_Item((selectedItemUI == null) ? false : true);
        if (!selectedItemUI) return;


        Set_Object_Name(Paths.SelectedItem, 1, selectedItemUI.GetItemData().objectName);
        Set_ITEM_Stack(Paths.SelectedItem, 1, selectedItemUI.GetItemData().currentStack);

        if (selectedItemUI.GetComponent<FoodUI>()) // Save food info
        {
            // For now there is no data that needs to be saved
        }
        else if (selectedItemUI.GetComponent<EquipmentUI>()) // Save equipment info
        {
            Set_ITEM_Durability(Paths.SelectedItem, 1, selectedItemUI.GetEquipmentData().durability);
        }
    }

    void SaveEquipment()
    {
        ItemUI handitem = EquipmentManager.instance.GetHandItem();
        ItemUI bodyitem = EquipmentManager.instance.GetBodyItem();
        ItemUI headitem = EquipmentManager.instance.GetHeadItem();

        Set_Has_Equipment(EquipmentType.hand, (handitem == null) ? false : true);
        Set_Has_Equipment(EquipmentType.body, (bodyitem == null) ? false : true);
        Set_Has_Equipment(EquipmentType.head, (headitem == null) ? false : true);

        if (handitem)
        {
            Set_Object_Name(Paths.Equipment,    (int)EquipmentType.hand, handitem.GetItemData().objectName);
            Set_ITEM_Stack(Paths.Equipment,     (int)EquipmentType.hand, handitem.GetItemData().currentStack);
            Set_ITEM_Durability(Paths.Equipment,(int)EquipmentType.hand, handitem.GetEquipmentData().durability);
        }
        if (bodyitem)
        {
            Set_Object_Name(Paths.Equipment,    (int)EquipmentType.body, bodyitem.GetItemData().objectName);
            Set_ITEM_Stack(Paths.Equipment,     (int)EquipmentType.body, bodyitem.GetItemData().currentStack);
            Set_ITEM_Durability(Paths.Equipment,(int)EquipmentType.body, bodyitem.GetEquipmentData().durability);
        }
        if (headitem)
        {
            Set_Object_Name(Paths.Equipment,    (int)EquipmentType.head, headitem.GetItemData().objectName);
            Set_ITEM_Stack(Paths.Equipment,     (int)EquipmentType.head, headitem.GetItemData().currentStack);
            Set_ITEM_Durability(Paths.Equipment,(int)EquipmentType.head, headitem.GetEquipmentData().durability);
        }
    }


    void SaveItems()
    {
        Set_Amount_Of_Objects(Paths.Item, WorldManager.instance.items.childCount);
        for (int i = 0; i < WorldManager.instance.items.childCount; i++)
        {
            Item item = WorldManager.instance.items.GetChild(i).GetComponent<Item>();

            Set_Object_Name(Paths.Item, i, item.GetItemData().objectName);
            Set_Object_Pos(Paths.Item, i, item.transform.position);

            Set_ITEM_Stack(Paths.Item, i, item.GetItemData().currentStack);

            if(item.GetComponent<Food>()) // Save food info
            {
                // For now there is no data that needs to be saved
            }
            else if (item.GetComponent<Equipment>()) // Save equipment info
            {
                Set_ITEM_Durability(Paths.Item, i, item.GetEquipmentData().durability);
            }
        }
    }

    void SaveResources()
    {
        
        Set_Amount_Of_Objects(Paths.Resource, WorldManager.instance.resources.childCount);

        for (int i = 0; i < WorldManager.instance.resources.childCount; i++)
        {
            Transform resource = WorldManager.instance.resources.GetChild(i);

            Set_Object_Name(Paths.Resource, i, resource.GetComponent<Resource>().objectName);
            Set_Object_Pos(Paths.Resource, i, resource.position);

            if (resource.GetComponent<ComplexResource>())
                Set_Object_Hp(Paths.Resource, i, resource.GetComponent<ComplexResource>().hp);


            Set_RESOURCE_Time_To_Grow(i, resource.GetComponent<Resource>().GetGrowTimer_RemainedTime());
        }
    }
    void SaveConstructions()
    {
        Set_Amount_Of_Objects(Paths.Construction, WorldManager.instance.constructions.childCount);

        for (int i = 0; i < WorldManager.instance.constructions.childCount; i++)
        {
            Transform construction = WorldManager.instance.constructions.GetChild(i);

            Set_Object_Name(Paths.Construction,i, construction.GetComponent<Construction>().objectName);
            Set_Object_Pos(Paths.Construction, i, construction.position);


            if (construction.GetComponent<Fireplace>())
                Set_CONSTRUCTION_Fire_Time(i, construction.GetComponent<Fireplace>().GetFireTimer_RemainedTime());

//            if(construction.GetComponent<Storage>())
  //              Set_CONSTRUCTION_Storage_Data(i, construction.GetComponent<Storage>().GetStorageData());
        }
    }

    void SaveMobs()
    {
        Set_Amount_Of_Objects(Paths.Mob, WorldManager.instance.mobs.childCount);

        for (int i = 0; i < WorldManager.instance.mobs.childCount; i++)
        {
            Transform mob = WorldManager.instance.mobs.GetChild(i);

            Set_Object_Name(Paths.Mob, i, mob.GetComponent<MobStats>().objectName);
            Set_Object_Hp(Paths.Mob, i, mob.GetComponent<MobStats>().hp);
            Set_Object_Pos(Paths.Mob, i, mob.position);


            float spawnerIndex = -1;
            if (!mob.GetComponent<MobStats>().spawner)
            {
                spawnerIndex = mob.GetComponent<MobStats>().spawner.GetSiblingIndex();

                if (mob.GetComponent<MobStats>().spawner.GetComponent<Construction>())
                    spawnerIndex += .1f;
            }
            Set_MOB_Spawner_Index(i, spawnerIndex);
           
        }
    }

    void SaveSpawners()
    {
        Set_Amount_Of_Objects(Paths.Spawner, WorldManager.instance.mobSpawners.childCount);

        for (int i = 0; i < WorldManager.instance.mobSpawners.childCount; i++)
        {
            Transform spawner = WorldManager.instance.mobSpawners.GetChild(i);

            Set_Object_Name(Paths.Spawner, i, spawner.GetComponent<ComplexMobSpawner>().objectName);
            Set_Object_Pos(Paths.Spawner, i, spawner.position);

            Set_SPAWNER_Mobs_Amount(i, spawner.GetComponent<ComplexMobSpawner>().mobsToSpawn);
            Set_SPAWNER_Mobs_Respawn_Time(i, spawner.GetComponent<ComplexMobSpawner>().GetRespawnTimer_TimeRemained());

        }
    }

    #endregion
}
