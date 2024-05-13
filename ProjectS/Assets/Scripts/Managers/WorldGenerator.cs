using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

using static SaveLoadManager;
using System.IO;
using static UnityEditor.Progress;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TMPro;

public class WorldGenerator : MonoBehaviour
{
    private static System.Random rnd = new System.Random();

    public static WorldGenerator instance;

    [SerializeField] private TextMeshProUGUI labelGenerationStep;
    void Awake()
    {
        instance = this;
        if (Get_Old_World_Current() == 0)
            StartCoroutine(GenerateWorld());
        else
            StartCoroutine(LoadWorld());

        Set_Old_World_Current(1);
    }

    IEnumerator FinishGenerating()
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


        yield return new WaitForSeconds(1);
        labelGenerationStep.transform.parent.gameObject.SetActive(false);
        StartCoroutine(CraftingManager.instance.RefreshCraftingMenu());
    }

    #region Generate

    private IEnumerator GenerateWorld()
    {
        yield return null;

        LoadTerrain();

        PlayerStats.instance.SetStats();
        SpawnObjects();

        SetTimeSettings();

        StartCoroutine(FinishGenerating());
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
            if(objectName != ObjectName.houndSpawner)
                SpawnMobSpawners(objectName, objectSpawnValue);
    }
    private void SpawnItems(ObjectName objectName, int objectSpawnValue)
    {
        labelGenerationStep.text = "Generating world...\n Generating items data!";

        int nr = rnd.Next((int)(objectSpawnValue * .75f), (int)(objectSpawnValue * 1.25f));
        int spawnRange = Get_World_Size() / 2;


        for (int i = 0; i < nr; i++)
        {
            Item item = ItemsManager.instance.CreateItem(objectName);

            item.transform.SetParent(WorldManager.instance.items);
            item.transform.position = new Vector3(Random.Range(-1 * spawnRange, spawnRange), 0, Random.Range(-1 * spawnRange, spawnRange));

        }
    }
    private void SpawnResources(ObjectName objectName, int objectSpawnValue)
    {
        labelGenerationStep.text = "Generating world...\n Generating resources data!";

        int nr = rnd.Next((int)(objectSpawnValue * .75f), (int)(objectSpawnValue * 1.25f));
        int spawnRange = Get_World_Size() / 2;

        for (int i = 0; i < nr; i++)
        {
            Resource resource = Instantiate(ItemsManager.instance.GetOriginalResource(objectName));

            resource.transform.SetParent(WorldManager.instance.resources);
            resource.transform.position = new Vector3(Random.Range(-1 * spawnRange, spawnRange), 0, Random.Range(-1 * spawnRange, spawnRange));

            resource.GetComponent<Resource>().SetResourceData();

            resource.gameObject.AddComponent<SpawnOverlapZone>();
        }
    }
    private void SpawnMobSpawners(ObjectName objectName, int objectSpawnValue)
    {
        labelGenerationStep.text = "Generating world...\n Generating spawners data!";

        int nr = rnd.Next((int)(objectSpawnValue * .75f), (int)(objectSpawnValue * 1.25f));
        int spawnRange = Get_World_Size() / 2;

        for (int i = 0; i < nr; i++)
        {
            MobSpawner mobSpawner = Instantiate(ItemsManager.instance.GetOriginalMobSpawner(objectName));

            mobSpawner.transform.SetParent(WorldManager.instance.mobSpawners);
            mobSpawner.transform.position = new Vector3(Random.Range(-1 * spawnRange, spawnRange), 0, Random.Range(-1 * spawnRange, spawnRange));

            mobSpawner.GetComponent<ComplexMobSpawner>().SetSpawnerData();
            mobSpawner.gameObject.AddComponent<SpawnOverlapZone>();
        }
    }
    #endregion


    #region Load
    private IEnumerator LoadWorld()
    {
        yield return null;

        LoadTerrain();

        LoadPlayer();
        LoadInventory();
        LoadEquipment();

        LoadItems();
        LoadResources();
        LoadConstructions();
        LoadSpawners();

        LoadMobs(); // needs to be loaded after the spawners

        LoadTimeSettings();
        StartCoroutine(FinishGenerating());
    }

    void LoadTerrain()
    {
        int size = Get_World_Size();
        foreach (Transform border in WorldManager.instance.border)
        {
            if (border.position.x > 0)
                border.position = new Vector3(-size / 2, border.position.y, border.position.z);
            else if (border.position.x < 0)
                border.position = new Vector3(size / 2, border.position.y, border.position.z);
            else if (border.position.z > 0)
                border.position = new Vector3(border.position.x, border.position.y, size / 2);
            else if (border.position.z < 0)
                border.position = new Vector3(border.position.x, border.position.y, -size / 2);
        }
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
        labelGenerationStep.text = "Generating world...\n Loading player data!";

        int _hp         = Get_Object_Hp(Paths.Player.ToString());
        int _hunger     = Get_Object_Hunger(Paths.Player.ToString());
        int _speed      = Get_Object_Speed(Paths.Player.ToString());
        Vector3 _pos    = Get_Object_Pos(Paths.Player.ToString());


        PlayerStats.instance.SetStats(_hp, _hunger , _speed,  _pos);
    }


    void LoadInventory()
    {
        labelGenerationStep.text = "Generating world...\n Loading inventory data!";

        for (int i = 0; i < Get_Amount_Of_Objects(Paths.InventoryItem.ToString()); i++)
        {
            string inventoryItem_LoadPath = Paths.InventoryItem.ToString() + i.ToString();

            ObjectName name = Get_Object_Name(inventoryItem_LoadPath);
            int curentStack = Get_ITEM_Stack(inventoryItem_LoadPath);

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
                int durability = Get_ITEM_Durability(inventoryItem_LoadPath);
                data = new EquipmentData(name, curentStack, durability);


                if (originalItem.GetComponent<Storage>())
                {
                    // load backpack storage info
                }

                //Load equipment data
            }

            int slotNr = Get_ITEM_UI_Parent_Slot(inventoryItem_LoadPath);

            ItemUI itemUI = ItemsManager.instance.CreateItemUI(data, storageData);
            InventoryManager.instance.AddItemToInventorySlot(slotNr, itemUI);
        }
        // Load Selected item and drop it on the ground
        if (!Get_Has_Selected_Item(Paths.SelectedItem.ToString())) return;

        string selectedItem_LoadPath = Paths.SelectedItem.ToString();

        ObjectName selectedItemName = Get_Object_Name(selectedItem_LoadPath);
        int selectedItemStack = Get_ITEM_Stack(selectedItem_LoadPath);

        Item originalSelectedItem = ItemsManager.instance.GetOriginalItem(selectedItemName);
        ItemData selectedItemData = null;

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

            int selectedItemDurability = Get_ITEM_Durability(selectedItem_LoadPath); 

            selectedItemData = new EquipmentData(selectedItemName, selectedItemStack, selectedItemDurability);
        }

        ItemUI selectedItemUI = ItemsManager.instance.CreateItemUI(selectedItemData, null);

        if (selectedItemUI.GetComponent<Storage>())
            LoadStorage(selectedItem_LoadPath, selectedItemUI.GetComponent<Storage>().GetStorageData());

        InventoryManager.instance.DropItem(selectedItemUI,PlayerBehaviour.instance.transform.position);


    }

    void LoadEquipment()
    {
        labelGenerationStep.text = "Generating world...\n Loading equipment data!";

        ItemUI handitem;
        ItemUI bodyitem;
        ItemUI headitem;

        ItemData data;

        if (Get_Has_Equipment(EquipmentType.hand.ToString()))
        {
            string handItem_LoadPath = Paths.Equipment.ToString() + EquipmentType.hand.ToString();

            ObjectName name = Get_Object_Name(handItem_LoadPath);
            int stack       = Get_ITEM_Stack(handItem_LoadPath);
            int durability  = Get_ITEM_Durability(handItem_LoadPath);

            data = new EquipmentData(name, stack, durability);

            handitem = ItemsManager.instance.CreateItemUI(data, null);
            EquipmentManager.instance.SetEquipment(handitem, false, false);

        }
        if (Get_Has_Equipment(EquipmentType.body.ToString()))
        {
            string bodyItem_LoadPath = Paths.Equipment.ToString() + EquipmentType.body.ToString();

            ObjectName name = Get_Object_Name(bodyItem_LoadPath);
            int stack       = Get_ITEM_Stack(bodyItem_LoadPath);
            int durability  = Get_ITEM_Durability(bodyItem_LoadPath);

            data = new EquipmentData(name, stack, durability);

            bodyitem = ItemsManager.instance.CreateItemUI(data, null);
            EquipmentManager.instance.SetEquipment(bodyitem, false, false);

            if (bodyitem.GetComponent<Storage>())
            {
                LoadStorage(bodyItem_LoadPath, bodyitem.GetComponent<Storage>().GetStorageData());
                InventoryManager.instance.DisplayBackpack(bodyitem.GetComponent<Storage>());
            }
        }
        if (Get_Has_Equipment(EquipmentType.head.ToString()))
        {
            string headItem_LoadPath = Paths.Equipment.ToString() + EquipmentType.head.ToString();

            ObjectName name = Get_Object_Name(headItem_LoadPath);
            int stack       = Get_ITEM_Stack(headItem_LoadPath);
            int durability  = Get_ITEM_Durability(headItem_LoadPath);

            data = new EquipmentData(name, stack, durability);
            headitem = ItemsManager.instance.CreateItemUI(data, null);
            EquipmentManager.instance.SetEquipment(headitem, false, false);

        }
    }


    void LoadItems()
    {
        labelGenerationStep.text = "Generating world...\n Loading items data!";

        for (int i = 0; i < Get_Amount_Of_Objects(Paths.Item.ToString()); i++)
        {
            string item_LoadPath = Paths.Item.ToString() + i.ToString();

            ObjectName _name = Get_Object_Name(item_LoadPath);
            int _curentStack = Get_ITEM_Stack(item_LoadPath);

            
            Item originalItem = ItemsManager.instance.GetOriginalItem(_name);
            ItemData data = null;

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
                int _durability = Get_ITEM_Durability(item_LoadPath);
                data = new EquipmentData(_name, _curentStack, _durability);

                //Load equipment data
            }

            Item item = ItemsManager.instance.CreateItem(data, null);
            
            if (item.GetComponent<Storage>())
                LoadStorage(item_LoadPath, item.GetComponent<Storage>().GetStorageData());

            item.transform.SetParent(WorldManager.instance.items);
            item.transform.position = Get_Object_Pos(item_LoadPath);
        }
    }
    void LoadResources()
    {
        labelGenerationStep.text = "Generating world...\n Loading resources data!";

        for (int i = 0; i < Get_Amount_Of_Objects(Paths.Resource.ToString()); i++)
        {
            string resource_LoadPath = Paths.Resource.ToString() + i.ToString();

            Resource resource = Instantiate(ItemsManager.instance.GetOriginalResource(Get_Object_Name(resource_LoadPath)));

            resource.transform.SetParent(WorldManager.instance.resources);
            resource.transform.position = Get_Object_Pos(resource_LoadPath);

            float growTimer_RemainedTime = Get_RESOURCE_Time_To_Grow(resource_LoadPath);
            int res_hp = Get_Object_Hp(resource_LoadPath);

            resource.SetResourceData(growTimer_RemainedTime, res_hp);
        }

    }

    void LoadConstructions()
    {
        labelGenerationStep.text = "Generating world...\n Loading constructions data!";

        for (int i = 0; i < Get_Amount_Of_Objects(Paths.Construction.ToString()); i++)
        {
            string construction_LoadPath = Paths.Construction.ToString() + i.ToString();

            Construction construction = Instantiate(ItemsManager.instance.GetOriginalConstruction(Get_Object_Name(construction_LoadPath)));

            construction.transform.SetParent(WorldManager.instance.constructions);
            construction.transform.position = Get_Object_Pos(construction_LoadPath);

            float fireTimer_RemainedTime = Get_CONSTRUCTION_Fire_Time(construction_LoadPath);

            if (construction.GetComponent<Fireplace>())
                construction.GetComponent<Fireplace>().SetFireData(fireTimer_RemainedTime);

            if (construction.GetComponent<Storage>())
                LoadStorage(construction_LoadPath, construction.GetComponent<Storage>().GetStorageData());
        }
    }

    void LoadMobs()
    {
        labelGenerationStep.text = "Generating world...\n Loading mobs data!";

        for (int i = 0; i < Get_Amount_Of_Objects(Paths.Mob.ToString()); i++)
        {
            string mob_LoadPath = Paths.Mob.ToString() + i.ToString();

            MobStats mob = Instantiate(ItemsManager.instance.GetOriginalMob(Get_Object_Name(mob_LoadPath)));

            mob.transform.SetParent(WorldManager.instance.mobs);
            mob.transform.position = Get_Object_Pos(mob_LoadPath);

            float spawnerIndex = Get_MOB_Spawner_Index(mob_LoadPath);

            Transform _spawner;
            if((int)spawnerIndex != spawnerIndex)
                _spawner = (spawnerIndex == -1) ? null : WorldManager.instance.constructions.GetChild((int)spawnerIndex); 
            else
                _spawner = (spawnerIndex == -1) ? null : WorldManager.instance.mobSpawners.GetChild((int)spawnerIndex);

            int _hp = Get_Object_Hp(mob_LoadPath);

            mob.SetMobData(_spawner, _hp);
        }
    }

    void LoadSpawners()
    {
        labelGenerationStep.text = "Generating world...\n Loading spawners data!";

        for (int i = 0; i < Get_Amount_Of_Objects(Paths.Spawner.ToString()); i++)
        {
            string spawner_LoadPath = Paths.Spawner.ToString() + i.ToString();

            MobSpawner spawner = Instantiate(ItemsManager.instance.GetOriginalMobSpawner(Get_Object_Name(spawner_LoadPath)));

            spawner.transform.SetParent(WorldManager.instance.mobSpawners);
            spawner.transform.position = Get_Object_Pos(spawner_LoadPath);

            int mobsToSpawn                 = Get_SPAWNER_Mobs_Amount(spawner_LoadPath);
            float respawnTimer_RemainedTime = Get_SPAWNER_Mobs_Respawn_Time(spawner_LoadPath);
            
            spawner.SetSpawnerData(mobsToSpawn,respawnTimer_RemainedTime);
        }
    }


    void LoadStorage(string path, StorageData storageData)
    {
        for (int i = 0; i < Get_Amount_Of_Objects(path + Paths.StoredItem.ToString()); i++)
        {
            string storedItem_LoadPath = path + Paths.StoredItem.ToString() + i.ToString();

            ObjectName name = Get_Object_Name(storedItem_LoadPath);
            int curentStack = Get_ITEM_Stack(storedItem_LoadPath);
            int slotNr      = Get_ITEM_UI_Parent_Slot(storedItem_LoadPath);

            Item originalItem = ItemsManager.instance.GetOriginalItem(name);
            ItemData data = null;

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
                int durability = Get_ITEM_Durability(storedItem_LoadPath);
                data = new EquipmentData(name, curentStack, durability);

                //Load equipment data
            }

            storageData.AddData(data, slotNr);
        }
    }

    #endregion


    #region Save

    public void SaveWorld()
    {
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
        Set_Object_Hp(Paths.Player.ToString(), PlayerStats.instance.hp);
        Set_Object_Hunger(Paths.Player.ToString(), PlayerStats.instance.hunger);
        Set_Object_Speed(Paths.Player.ToString(), PlayerStats.instance.speed);
        Set_Object_Pos(Paths.Player.ToString(), PlayerStats.instance.transform.position);

    }

    void SaveInventory()
    {
        int nrOfItemsInInventory = 0;
        for (int i = 0; i < 15; i++)
            if (InventoryManager.instance.inventoryPanel.GetChild(i).childCount > 0)
            {
                string inventoryItem_SavePath = Paths.InventoryItem.ToString() + nrOfItemsInInventory;
                ItemUI itemUI = InventoryManager.instance.inventoryPanel.GetChild(i).GetComponent<InventorySlot>().GetItemInSlot();

                Set_ITEM_UI_Parent_Slot(inventoryItem_SavePath, i);

                Set_Object_Name(inventoryItem_SavePath, itemUI.GetItemData().objectName);
                Set_ITEM_Stack(inventoryItem_SavePath, itemUI.GetItemData().currentStack);

                if (itemUI.GetComponent<FoodUI>()) // Save food info
                {
                    // For now there is no data that needs to be saved
                }
                else if (itemUI.GetComponent<EquipmentUI>()) // Save equipment info
                {
                    Set_ITEM_Durability(inventoryItem_SavePath, itemUI.GetEquipmentData().durability);
                }

                nrOfItemsInInventory++;
            }
        Set_Amount_Of_Objects(Paths.InventoryItem.ToString(), nrOfItemsInInventory);
        
        // Save selected item

        string selectedItemUI_SavePath = Paths.SelectedItem.ToString();
        ItemUI selectedItemUI = InventoryManager.instance.selectedItemSlot.GetItemInSlot();
        Set_Has_Selected_Item(selectedItemUI_SavePath, (selectedItemUI == null) ? false : true);
        if (!selectedItemUI) return;


        Set_Object_Name(selectedItemUI_SavePath, selectedItemUI.GetItemData().objectName);
        Set_ITEM_Stack(selectedItemUI_SavePath, selectedItemUI.GetItemData().currentStack);

        if (selectedItemUI.GetComponent<FoodUI>()) // Save food info
        {
            // For now there is no data that needs to be saved
        }
        else if (selectedItemUI.GetComponent<EquipmentUI>()) // Save equipment info
        {
            Set_ITEM_Durability(selectedItemUI_SavePath, selectedItemUI.GetEquipmentData().durability);

            if (selectedItemUI.GetComponent<Storage>())
                SaveStorage(selectedItemUI_SavePath, selectedItemUI.GetComponent<Storage>().GetStorageData());
        }
    }

    void SaveEquipment()
    {
        ItemUI handitem = EquipmentManager.instance.GetHandItem();
        ItemUI bodyitem = EquipmentManager.instance.GetBodyItem();
        ItemUI headitem = EquipmentManager.instance.GetHeadItem();

        Set_Has_Equipment(EquipmentType.hand.ToString(), (handitem == null) ? false : true);
        Set_Has_Equipment(EquipmentType.body.ToString(), (bodyitem == null) ? false : true);
        Set_Has_Equipment(EquipmentType.head.ToString(), (headitem == null) ? false : true);

        if (handitem)
        {
            string handItem_SavePath = Paths.Equipment.ToString() + EquipmentType.hand.ToString();

            Set_Object_Name(handItem_SavePath, handitem.GetItemData().objectName);
            Set_ITEM_Stack(handItem_SavePath, handitem.GetItemData().currentStack);
            Set_ITEM_Durability(handItem_SavePath, handitem.GetEquipmentData().durability);
        }
        if (bodyitem)
        {
            string bodyItem_SavePath = Paths.Equipment.ToString() + EquipmentType.body.ToString();

            Set_Object_Name(bodyItem_SavePath, bodyitem.GetItemData().objectName);
            Set_ITEM_Stack(bodyItem_SavePath, bodyitem.GetItemData().currentStack);
            Set_ITEM_Durability(bodyItem_SavePath, bodyitem.GetEquipmentData().durability);

            if (bodyitem.GetComponent<Storage>())
                SaveStorage(bodyItem_SavePath, bodyitem.GetComponent<Storage>().GetStorageData());
        }
        if (headitem)
        {
            string headItem_SavePath = Paths.Equipment.ToString() + EquipmentType.head.ToString();

            Set_Object_Name(headItem_SavePath, headitem.GetItemData().objectName);
            Set_ITEM_Stack(headItem_SavePath, headitem.GetItemData().currentStack);
            Set_ITEM_Durability(headItem_SavePath, headitem.GetEquipmentData().durability);
        }
    }


    void SaveItems()
    {
        Set_Amount_Of_Objects(Paths.Item.ToString(), WorldManager.instance.items.childCount);
        for (int i = 0; i < WorldManager.instance.items.childCount; i++)
        {
            string item_SavePath = Paths.Item.ToString() + i.ToString();

            Item item = WorldManager.instance.items.GetChild(i).GetComponent<Item>();

            Set_Object_Name(item_SavePath, item.GetItemData().objectName);
            Set_Object_Pos(item_SavePath, item.transform.position);

            Set_ITEM_Stack(item_SavePath, item.GetItemData().currentStack);

            if(item.GetComponent<Food>()) // Save food info
            {
                // For now there is no data that needs to be saved
            }
            else if (item.GetComponent<Equipment>()) // Save equipment info
            {
                Set_ITEM_Durability(item_SavePath, item.GetEquipmentData().durability);

                if (item.GetComponent<Storage>())
                    SaveStorage(item_SavePath, item.GetComponent<Storage>().GetStorageData());
            }
        }
    }

    void SaveResources()
    {
        
        Set_Amount_Of_Objects(Paths.Resource.ToString(), WorldManager.instance.resources.childCount);

        for (int i = 0; i < WorldManager.instance.resources.childCount; i++)
        {
            string resource_SavePath = Paths.Resource.ToString() + i.ToString();

            Transform resource = WorldManager.instance.resources.GetChild(i);

            Set_Object_Name(resource_SavePath, resource.GetComponent<Resource>().objectName);
            Set_Object_Pos(resource_SavePath, resource.position);

            if (resource.GetComponent<ComplexResource>())
                Set_Object_Hp(resource_SavePath, resource.GetComponent<ComplexResource>().hp);


            Set_RESOURCE_Time_To_Grow(resource_SavePath, resource.GetComponent<Resource>().GetGrowTimer_RemainedTime());
        }
    }
    void SaveConstructions()
    {
        Set_Amount_Of_Objects(Paths.Construction.ToString(), WorldManager.instance.constructions.childCount);

        for (int i = 0; i < WorldManager.instance.constructions.childCount; i++)
        {
            string construction_SavePath = Paths.Construction.ToString() + i.ToString();

            Transform construction = WorldManager.instance.constructions.GetChild(i);

            Set_Object_Name(construction_SavePath, construction.GetComponent<Construction>().objectName);
            Set_Object_Pos(construction_SavePath, construction.position);


            if (construction.GetComponent<Fireplace>())
                Set_CONSTRUCTION_Fire_Time(construction_SavePath, construction.GetComponent<Fireplace>().GetFireTimer_RemainedTime());

            if (construction.GetComponent<Storage>())
                SaveStorage(construction_SavePath, construction.GetComponent<Storage>().GetStorageData());
        }
    }

    void SaveMobs()
    {
        Set_Amount_Of_Objects(Paths.Mob.ToString(), WorldManager.instance.mobs.childCount);

        for (int i = 0; i < WorldManager.instance.mobs.childCount; i++)
        {
            string mob_SavePath = Paths.Mob.ToString() + i.ToString();

            Transform mob = WorldManager.instance.mobs.GetChild(i);

            Set_Object_Name(mob_SavePath, mob.GetComponent<MobStats>().objectName);
            Set_Object_Hp(mob_SavePath, mob.GetComponent<MobStats>().hp);
            Set_Object_Pos(mob_SavePath, mob.position);


            float spawnerIndex = -1;
            if (!mob.GetComponent<MobStats>().spawner)
            {
                spawnerIndex = mob.GetComponent<MobStats>().spawner.GetSiblingIndex();

                if (mob.GetComponent<MobStats>().spawner.GetComponent<Construction>())
                    spawnerIndex += .1f;
            }
            Set_MOB_Spawner_Index(mob_SavePath, spawnerIndex);
           
        }
    }

    void SaveSpawners()
    {
        Set_Amount_Of_Objects(Paths.Spawner.ToString(), WorldManager.instance.mobSpawners.childCount);

        for (int i = 0; i < WorldManager.instance.mobSpawners.childCount; i++)
        {
            string spawner_SavePath = Paths.Spawner.ToString() + i.ToString();

            Transform spawner = WorldManager.instance.mobSpawners.GetChild(i);

            Set_Object_Name(spawner_SavePath, spawner.GetComponent<ComplexMobSpawner>().objectName);
            Set_Object_Pos(spawner_SavePath, spawner.position);

            Set_SPAWNER_Mobs_Amount(spawner_SavePath, spawner.GetComponent<ComplexMobSpawner>().mobsToSpawn);
            Set_SPAWNER_Mobs_Respawn_Time(spawner_SavePath, spawner.GetComponent<ComplexMobSpawner>().GetRespawnTimer_TimeRemained());

        }
    }


    void SaveStorage(string path, StorageData storageData)
    {
        int nrOfItemsInStorage = 0;
        for (int i = 0; i < storageData.GetSize(); i++)
            if (storageData.HasElement(i))
            {
                string storedItem_SavePath = path + Paths.StoredItem.ToString() + nrOfItemsInStorage.ToString();

                Item originalitem = ItemsManager.instance.GetOriginalItem(storageData.GetElement(i).objectName);

                Set_Object_Name(storedItem_SavePath, storageData.GetElement(i).objectName);
                Set_ITEM_Stack(storedItem_SavePath, storageData.GetElement(i).currentStack);
                Set_ITEM_UI_Parent_Slot(storedItem_SavePath, i);

                if (originalitem.GetComponent<Food>()) // Save food info
                {
                    // For now there is no data that needs to be saved
                }
                else if (originalitem.GetComponent<Equipment>()) // Save equipment info
                {
                    Set_ITEM_Durability(storedItem_SavePath, ((EquipmentData)storageData.GetElement(i)).durability);
                }

                nrOfItemsInStorage++;
            }
        Set_Amount_Of_Objects(path + Paths.StoredItem.ToString(), nrOfItemsInStorage);
    }

    #endregion
}
