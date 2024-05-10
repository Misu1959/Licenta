using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;


public class WorldGenerator : MonoBehaviour
{
    private static System.Random rnd = new System.Random();

    void Awake()
    {
        if (SaveLoadManager.Get_Old_World_Current() == 0)
            StartCoroutine(GenerateWorld());
        else
            StartCoroutine(LoadWorld());

        SaveLoadManager.Set_Old_World_Current(1);
        Invoke(nameof(SaveWorld), 3);
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
    }

    #region Generate

    private IEnumerator GenerateWorld()
    {
        Debug.Log("Generate world");
        yield return null;
        SetTimeSettings();
        PlayerStats.instance.SetStats();
        
        SpawnObjects();

        FinishGenerating();
    }

    private void SetTimeSettings()
    {
        int dayDuration = SaveLoadManager.Get_Day_Duration();
        int dayLength   = SaveLoadManager.Get_Day_Length();
        int nightLength = SaveLoadManager.Get_Night_Length();
        
        TimeManager.instance.SetTimeSettings(dayDuration, dayLength, nightLength);
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < SaveLoadManager.Get_Spawn_Setting_Amount(); i++)
        {
            ObjectName objectName = (ObjectName)SaveLoadManager.Get_Spawn_Setting_Name(i);
            int objectSpawnValue = SaveLoadManager.Get_Spawn_Setting_Value(i);

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
        int worldSize = SaveLoadManager.Get_World_Size();

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
        int worldSize = SaveLoadManager.Get_World_Size();

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
        int worldSize = SaveLoadManager.Get_World_Size();

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

        LoadTimeSettings();
        LoadPlayer();
        LoadInventory();
        
        LoadItems();
        LoadResources();
        LoadConstructions();
        LoadSpawners();

        LoadMobs(); // needs to be loaded after the spawners


        FinishGenerating();
    }

    void LoadTimeSettings()
    {
        int dayDuration = SaveLoadManager.Get_Day_Duration();
        int dayLength   = SaveLoadManager.Get_Day_Length();
        int nightLength = SaveLoadManager.Get_Night_Length();

        int currentHour     = SaveLoadManager.Get_Current_Hour();
        int currentDay      = SaveLoadManager.Get_Current_Day();
        int currentDayState = SaveLoadManager.Get_Current_DayState();

        TimeManager.instance.SetTimeSettings(dayDuration, dayLength, nightLength, currentHour, currentDay, currentDayState);

    }

    void LoadPlayer()
    {
        int _hp         = SaveLoadManager.Get_Player_Hp();
        int _hunger     = SaveLoadManager.Get_Player_Hunger();
        int _speed      = SaveLoadManager.Get_Player_Speed();
        Vector3 _pos    = SaveLoadManager.Get_Player_Pos();

        PlayerStats.instance.SetStats(_hp, _hunger , _speed,  _pos);
        //LoadInventory();
    }

    void LoadItems()
    {
        for (int i = 0; i < SaveLoadManager.Get_Nr_Of_Items(); i++)
        {
            ObjectName _name = (ObjectName)SaveLoadManager.Get_Item_Name(i);

            Item originalItem = ItemsManager.instance.GetOriginalItem(_name);
            ItemData data = null;

            Sprite _uiImg = originalItem.GetItemData().uiImg;
            int _maxStack = originalItem.GetItemData().maxStack;
            int _fuelValue = originalItem.GetItemData().fuelValue;

            int _curentStack = SaveLoadManager.Get_Item_Stack(i);

            if (originalItem.GetComponent<Item>())
            {
                data = new ItemData(_uiImg, _name, _maxStack, _curentStack, _fuelValue);
                
                //Load material data
            }
            else if (originalItem.GetComponent<Food>())
            {
                data = new FoodData(_uiImg, _name, _maxStack, _curentStack, _fuelValue);
                // No new data to be loaded for food at the moment
            }
            else if (originalItem.GetComponent<Equipment>())
            {
                int _durability = SaveLoadManager.Get_Item_Durability(i);
                data = new EquipmentData(_uiImg, _name, _maxStack, _curentStack, _fuelValue,_durability);

                //Load equipment data
            }

            Item item = ItemsManager.instance.CreateItem(data);

            item.transform.SetParent(WorldManager.instance.items);
            item.transform.position = SaveLoadManager.Get_Item_Pos(i);
        }
    }
    void LoadResources()
    {
        for (int i = 0; i < SaveLoadManager.Get_Nr_Of_Resources(); i++)
        {
            Resource resource = Instantiate(ItemsManager.instance.GetOriginalResource((ObjectName)SaveLoadManager.Get_Resource_Name(i)));

            resource.transform.SetParent(WorldManager.instance.resources);
            resource.transform.position = SaveLoadManager.Get_Resource_Pos(i);

            float growTimer_RemainedTime = SaveLoadManager.Get_Resource_Time_To_Grow(i);
            int res_hp = SaveLoadManager.Get_Resource_Hp(i);
            resource.SetResourceData(growTimer_RemainedTime, res_hp);
        }

    }

    void LoadConstructions()
    {
        for (int i = 0; i < SaveLoadManager.Get_Nr_Of_Constructions(); i++)
        {
            Construction construction = Instantiate(ItemsManager.instance.GetOriginalConstruction((ObjectName)SaveLoadManager.Get_Construction_Name(i)));

            construction.transform.SetParent(WorldManager.instance.constructions);
            construction.transform.position = SaveLoadManager.Get_Construction_Pos(i);

            float fireTimer_RemainedTime = SaveLoadManager.Get_Construction_Fire_Time(i);

            if (construction.GetComponent<Fireplace>())
                construction.GetComponent<Fireplace>().SetFireData(fireTimer_RemainedTime);

        }
    }

    void LoadMobs()
    {
        for (int i = 0; i < SaveLoadManager.Get_Nr_Of_Mobs(); i++)
        {
            MobStats mob = Instantiate(ItemsManager.instance.GetOriginalMob((ObjectName)SaveLoadManager.Get_Mob_Name(i)));

            mob.transform.SetParent(WorldManager.instance.mobs);
            mob.transform.position = SaveLoadManager.Get_Mob_Pos(i);

            float spawnerIndex = SaveLoadManager.Get_Mob_Spawner_Index(i);

            Transform _spawner;
            if((int)spawnerIndex != spawnerIndex)
                _spawner = (spawnerIndex == -1) ? null : WorldManager.instance.constructions.GetChild((int)spawnerIndex); 
            else
                _spawner = (spawnerIndex == -1) ? null : WorldManager.instance.mobSpawners.GetChild((int)spawnerIndex);

            int _hp = SaveLoadManager.Get_Mob_Hp(i);

            mob.SetMobData(_spawner, _hp);
        }
    }

    void LoadSpawners()
    {
        for (int i = 0; i < SaveLoadManager.Get_Nr_Of_Spawners(); i++)
        {
            MobSpawner spawner = Instantiate(ItemsManager.instance.GetOriginalMobSpawner((ObjectName)SaveLoadManager.Get_Spawner_Name(i)));

            spawner.transform.SetParent(WorldManager.instance.mobSpawners);
            spawner.transform.position = SaveLoadManager.Get_Spawner_Pos(i);

            int mobsToSpawn = SaveLoadManager.Get_Spawner_Mobs_Amount(i);
            float respawnTimer_RemainedTime = SaveLoadManager.Get_Spawner_Mobs_Respawn_Time(i);
            
            spawner.SetSpawnerData(mobsToSpawn,respawnTimer_RemainedTime);
        }
    }





    void LoadInventory()
    {

        for (int i = 0; i < SaveLoadManager.Get_Nr_Of_Inventory_Items(); i++)
        {
            ObjectName _name = (ObjectName)SaveLoadManager.Get_Inventory_Item_Name(i);

            Item originalItem = ItemsManager.instance.GetOriginalItem(_name);
            ItemData data = null;

            Sprite _uiImg = originalItem.GetItemData().uiImg;
            int _maxStack = originalItem.GetItemData().maxStack;
            int _fuelValue = originalItem.GetItemData().fuelValue;

            int _curentStack = SaveLoadManager.Get_Inventory_Item_Stack(i);

            if (originalItem.GetComponent<Item>())
            {
                data = new ItemData(_uiImg, _name, _maxStack, _curentStack, _fuelValue);

                //Load material data
            }
            else if (originalItem.GetComponent<Food>())
            {
                data = new FoodData(_uiImg, _name, _maxStack, _curentStack, _fuelValue);
                // No new data to be loaded for food at the moment
            }
            else if (originalItem.GetComponent<Equipment>())
            {
                int _durability = SaveLoadManager.Get_Item_Durability(i);
                data = new EquipmentData(_uiImg, _name, _maxStack, _curentStack, _fuelValue, _durability);

                //Load equipment data
            }
            int slotNr = SaveLoadManager.Get_Inventory_Item_Parent_Slot(i);

            Item tempitem = ItemsManager.instance.CreateItem(data);

            ItemUI itemUI = ItemsManager.instance.CreateItemUI(tempitem);
            InventoryManager.instance.AddItemToInventorySlot(slotNr, itemUI);

        }
        /*
        if (PlayerPrefs.GetInt("selectedItem") == 1)
        {
            GameObject selectedItem = Instantiate(ItemsManager.instance.itemUI);

            if (PlayerPrefs.GetInt("selectedItem typeOfItem") == 1)
                selectedItem.AddComponent<ItemUI>();
            else if (PlayerPrefs.GetInt("selectedItem typeOfItem") == 2)
            {
                selectedItem.AddComponent<FoodUI>();

                selectedItem.GetComponent<Food>().hpAmount = PlayerPrefs.GetFloat("selectedItem foodHpAmount");
                selectedItem.GetComponent<Food>().hungerAmount = PlayerPrefs.GetFloat("selectedItem foodHungerAmount");

            }
            else if (PlayerPrefs.GetInt("selectedItem typeOfItem") == 3)
            {
                selectedItem.AddComponent<EquipmentUI>();

                selectedItem.GetComponent<Equipment>().equipmentType = (Equipment.Type)PlayerPrefs.GetInt("selectedItem equipmentNumber");
                selectedItem.GetComponent<Equipment>().actionNumber = PlayerPrefs.GetInt("selectedItem equipmentActionNumber");
                selectedItem.GetComponent<Equipment>().SetDurability(PlayerPrefs.GetFloat("selectedItem equipmentDurability"));
            }

            selectedItem.GetComponent<Item>().SetName(PlayerPrefs.GetString("selectedItem type"));
            selectedItem.GetComponent<Item>().AddToStack(PlayerPrefs.GetInt("selectedItem currentStack"));

            selectedItem.GetComponent<Item>().maxStack = PlayerPrefs.GetInt("selectedItem maxStack");
            selectedItem.GetComponent<Item>().fuelValue = PlayerPrefs.GetInt("selectedItem fuelValue");

            selectedItem.GetComponent<Item>().transform.SetParent(InventoryManager.instance.inventoryPanel);
            InventoryManager.instance.se = selectedItem.GetComponent<Item>();

            selectedItem.GetComponent<Image>().color = ItemsManager.instance.SearchItemsList(selectedItem.GetComponent<Item>().name).GetComponent<SpriteRenderer>().color;
        }*/
    }







    #endregion


    #region Save

    public void SaveWorld()
    {
        Debug.Log("Save world");

        SavePlayer();

        SaveItems();
        SaveResources();
        SaveConstructions();
        SaveMobs();
        SaveSpawners();
    }

    void SavePlayer()
    {
        SaveLoadManager.Set_Player_Hp();
        SaveLoadManager.Set_Player_Hunger();
        SaveLoadManager.Set_Player_Speed();
        SaveLoadManager.Set_Player_Pos();

        //SaveInventory();
        // Save equipment
    }

    void SaveInventory()
    {
        int nrOfItemsInInventory = 0;
        for (int i = 0; i < 15; i++)
            if (InventoryManager.instance.inventoryPanel.GetChild(i).childCount > 0)
            {
                ItemUI item = InventoryManager.instance.inventoryPanel.GetChild(i).GetComponent<InventorySlot>().GetItemInSlot();

                SaveLoadManager.Set_Inventory_Item_Parent_Slot(nrOfItemsInInventory, i);

                SaveLoadManager.Set_Inventory_Item_Name(i, (int)item.GetItemData().objectName);
                SaveLoadManager.Set_Inventory_Item_Stack(i, item.GetItemData().currentStack);

                if (item.GetComponent<Food>()) // Save food info
                {
                    // For now there is no data that needs to be saved
                }
                else if (item.GetComponent<Equipment>()) // Save equipment info
                {
                    SaveLoadManager.Set_Inventory_Item_Durability(i, item.GetEquipmentData().durability);
                }

                nrOfItemsInInventory++;
            }
        SaveLoadManager.Set_Nr_Of_Inventory_Items(nrOfItemsInInventory);

        /*
        if(InventoryManager.instance.selectedItem)
        {
            PlayerPrefs.SetInt("selectedItem", 1);

            Item selectedItem = InventoryManager.instance.selectedItem;

            if (selectedItem.GetComponent<ItemUI>())
                PlayerPrefs.SetInt("selectedItem typeOfItem", 1);
            else if (selectedItem.GetComponent<FoodUI>())
            {
                PlayerPrefs.SetInt("selectedItem typeOfItem", 2);
                PlayerPrefs.SetFloat("selectedItem foodHpAmount", selectedItem.GetComponent<Food>().hpAmount);
                PlayerPrefs.SetFloat("selectedItem hungerHpAmount", selectedItem.GetComponent<Food>().hungerAmount);
            }
            if (selectedItem.GetComponent<EquipmentUI>())
            {
                PlayerPrefs.SetInt("selectedItem typeOfItem", 3);
                PlayerPrefs.SetInt("selectedItem equipmentNumber", (int)selectedItem.GetComponent<Equipment>().equipmentType);
                PlayerPrefs.SetInt("selectedItem equipmentActionNumber", selectedItem.GetComponent<Equipment>().actionNumber);
                PlayerPrefs.SetFloat("selectedItem equipmentDurability", selectedItem.GetComponent<Equipment>().durability);
            }


            PlayerPrefs.SetString("selectedItem type", InventoryManager.instance.selectedItem.name);

            PlayerPrefs.SetInt("selectedItem maxStack", InventoryManager.instance.selectedItem.maxStack);
            PlayerPrefs.SetInt("selectedItem currentStack", InventoryManager.instance.selectedItem.currentStack);

            PlayerPrefs.SetInt("selectedItem fuelValue", InventoryManager.instance.selectedItem.fuelValue);
        }
        else
            PlayerPrefs.SetInt("selectedItem", 0);
        */
    }




    void SaveItems()
    {
        SaveLoadManager.Set_Nr_Of_Items();
        for (int i = 0; i < WorldManager.instance.items.childCount; i++)
        {
            Item item = WorldManager.instance.items.GetChild(i).GetComponent<Item>();

            SaveLoadManager.Set_Item_Pos(i, item.transform.position.x, item.transform.position.z);

            SaveLoadManager.Set_Item_Name(i,(int)item.GetItemData().objectName);
            SaveLoadManager.Set_Item_Stack(i, item.GetItemData().currentStack);

            if(item.GetComponent<Food>()) // Save food info
            {
                // For now there is no data that needs to be saved
            }
            else if (item.GetComponent<Equipment>()) // Save equipment info
            {
                SaveLoadManager.Set_Item_Durability(i, item.GetEquipmentData().durability);
            }
        }
    }
    void SaveResources()
    {
        
        SaveLoadManager.Set_Nr_Of_Resources();

        for (int i = 0; i < WorldManager.instance.resources.childCount; i++)
        {
            Transform resource = WorldManager.instance.resources.GetChild(i);

            SaveLoadManager.Set_Resource_Name(i, (int)resource.GetComponent<Resource>().objectName);
            SaveLoadManager.Set_Resource_Time_To_Grow(i,resource.GetComponent<Resource>().GetGrowTimer_RemainedTime());

            SaveLoadManager.Set_Resource_Pos(i, resource.position.x, resource.position.z);

            if (resource.GetComponent<ComplexResource>())
                SaveLoadManager.Set_Resource_Hp(i, resource.GetComponent<ComplexResource>().hp);
        }
    }
    void SaveConstructions()
    {
        SaveLoadManager.Set_Nr_Of_Constructions();

        for (int i = 0; i < WorldManager.instance.constructions.childCount; i++)
        {
            Transform construction = WorldManager.instance.constructions.GetChild(i);

            SaveLoadManager.Set_Construction_Name(i, (int)construction.GetComponent<Construction>().objectName);
            SaveLoadManager.Set_Construction_Pos(i, construction.position.x, construction.position.z);

            if (construction.GetComponent<Fireplace>())
                SaveLoadManager.Set_Construction_Fire_Time(i, construction.GetComponent<Fireplace>().GetFireTimer_RemainedTime());
        }
    }

    void SaveMobs()
    {
        SaveLoadManager.Set_Nr_Of_Mobs();

        for (int i = 0; i < WorldManager.instance.mobs.childCount; i++)
        {
            Transform mob = WorldManager.instance.mobs.GetChild(i);

            SaveLoadManager.Set_Mob_Name(i, (int)mob.GetComponent<MobStats>().objectName);
            SaveLoadManager.Set_Mob_Hp(i, mob.GetComponent<MobStats>().hp);

            float spawnerIndex = -1;
            if (!mob.GetComponent<MobStats>().spawner)
            {
                spawnerIndex = mob.GetComponent<MobStats>().spawner.GetSiblingIndex();

                if (mob.GetComponent<MobStats>().spawner.GetComponent<Construction>())
                    spawnerIndex += .1f;
            }
            SaveLoadManager.Set_Mob_Spawner_Index(i, spawnerIndex);
            SaveLoadManager.Set_Mob_Pos(i, mob.position.x, mob.position.z);
           
        }
    }

    void SaveSpawners()
    {
        SaveLoadManager.Set_Nr_Of_Spawners();

        for (int i = 0; i < WorldManager.instance.mobSpawners.childCount; i++)
        {
            Transform spawner = WorldManager.instance.mobSpawners.GetChild(i);

            SaveLoadManager.Set_Spawner_Name(i, (int)spawner.GetComponent<ComplexMobSpawner>().objectName);

            SaveLoadManager.Set_Spawner_Mobs_Amount(i, spawner.GetComponent<ComplexMobSpawner>().mobsToSpawn);
            SaveLoadManager.Set_Spawner_Mobs_Respawn_Time(i, spawner.GetComponent<ComplexMobSpawner>().GetRespawnTimer_TimeRemained());

            SaveLoadManager.Set_Spawner_Pos(i, spawner.position.x, spawner.position.z);
        }
    }

    #endregion


}
