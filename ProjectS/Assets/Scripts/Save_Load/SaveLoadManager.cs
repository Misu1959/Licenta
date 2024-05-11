using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using static WorldSettingsManager;

public static class SaveLoadManager 
{
    private const string S_WORLD            = "World";
    private const string S_OLD_WORLD        = "OldWorld";
        
    private const string S_LAST_WORLD       = "LastWorld";
    private const string S_SELECTED_WORLD   = "SelectedWorld";
    private static string World(int key) => S_WORLD + key.ToString();
    private static string Current_World() => World(Get_Selected_world());


    public static void Set_Last_world(int value) => PlayerPrefs.SetInt(S_LAST_WORLD, value);
    public static int Get_Last_world() => PlayerPrefs.GetInt(S_LAST_WORLD);


    public static void Set_Selected_world(int value) => PlayerPrefs.SetInt(S_SELECTED_WORLD, value);
    public static int Get_Selected_world() => PlayerPrefs.GetInt(S_SELECTED_WORLD);


    public static void Set_Old_World(int key, int value) => PlayerPrefs.SetInt(World(key) + S_OLD_WORLD, value);
    public static int Get_Old_World(int key) => PlayerPrefs.GetInt(World(key) + S_OLD_WORLD);


    public static void Set_Old_World_Current(int value) => PlayerPrefs.SetInt(Current_World() + S_OLD_WORLD, value);
    public static int Get_Old_World_Current() => PlayerPrefs.GetInt(Current_World() + S_OLD_WORLD);

#region Rebinds

    private static string S_NR_REBINDS_CHANGES = "rebinds_changes";
    private static string S_REBINDS = "rebinds";

    public static int Get_Nr_Rebinds_Changes() => PlayerPrefs.GetInt(S_NR_REBINDS_CHANGES);
    public static void Set_Nr_Rebinds_Changes(int value) => PlayerPrefs.SetInt(S_NR_REBINDS_CHANGES, value);

    public static string Get_Rebinds() => PlayerPrefs.GetString(S_REBINDS);
    public static void Set_Rebinds(string str) => PlayerPrefs.SetString(S_REBINDS, str);
    public static void Delete_Rebinds() => PlayerPrefs.DeleteKey(S_REBINDS);
#endregion

#region Spawn Settings

    private const string S_NR_SPAWN_SETTINGS = "NrSpawnSettings";
    private const string S_SPAWN_SETTING_NAME = "SpawnSettingName";
    private const string S_SPAWN_SETTING_VALUE = "SpawnSettingValue";

    public static int Get_Spawn_Setting_Amount() => PlayerPrefs.GetInt(Current_World() + S_NR_SPAWN_SETTINGS);
    public static void Set_Spawn_Setting_Amount(int value) => PlayerPrefs.SetInt(Current_World() + S_NR_SPAWN_SETTINGS, value);



    public static int Get_Spawn_Setting_Name(int key) => PlayerPrefs.GetInt(Current_World() + S_SPAWN_SETTING_NAME + key);
    public static void Set_Spawn_Setting_Name(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_SPAWN_SETTING_NAME + key, value);

    public static int Get_Spawn_Setting_Value(int key) => PlayerPrefs.GetInt(Current_World() + S_SPAWN_SETTING_VALUE + key);
    public static void Set_Spawn_Setting_Value(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_SPAWN_SETTING_VALUE + key, value);

#endregion

#region World Settings
    public static void Set_World_Setting(string name, int value) => PlayerPrefs.SetInt(Current_World() + name, value);
    public static int Get_World_Setting(string name) => PlayerPrefs.GetInt(Current_World() + name);


    public static int Get_World_Size() => Get_World_Setting(WorldSettingName.WorldSize.ToString());
    public static int Get_Day_Duration() => Get_World_Setting(WorldSettingName.DayDuration.ToString());
    public static int Get_Day_Length() => Get_World_Setting(WorldSettingName.DayLength.ToString());
    public static int Get_Dawn_Length() => Get_World_Setting(WorldSettingName.DawnLength.ToString());
    public static int Get_Night_Length() => Get_World_Setting(WorldSettingName.NightLength.ToString());


    #endregion

    #region World

    #region Time

    private const string S_CURRENT_HOUR         = "CurrentHour";
    private const string S_CURRENT_DAY          = "CurrentDay";
    private const string S_CURRENT_DAY_STATE    = "CurrentDayState";

    public static void Set_Current_Hour() => PlayerPrefs.SetInt(Current_World() + S_CURRENT_HOUR, TimeManager.instance.currentHour);
    public static int Get_Current_Hour() => PlayerPrefs.GetInt(Current_World() + S_CURRENT_HOUR);


    public static void Set_Current_Day() => PlayerPrefs.SetInt(Current_World() + S_CURRENT_DAY, TimeManager.instance.currentDay);
    public static int Get_Current_Day() => PlayerPrefs.GetInt(Current_World() + S_CURRENT_DAY);
    public static int Get_World_Day(int key) => PlayerPrefs.GetInt(World(key) + S_CURRENT_DAY);


    public static void Set_Current_DayState() => PlayerPrefs.SetInt(Current_World() + S_CURRENT_DAY_STATE, (int)TimeManager.instance.dayState);
    public static int Get_Current_DayState() => PlayerPrefs.GetInt(Current_World() + S_CURRENT_DAY_STATE);


    #endregion


    #region Player

    private const string S_PLAYER_HP        = "Player_Hp";
    private const string S_PLAYER_HUNGER    = "Player_Hunger";

    private const string S_PLAYER_DMG       = "Player_Dmg";
    private const string S_PLAYER_SPEED     = "Player_Speed";

    private const string S_PLAYER_POS_X     = "Player_Pos_X";
    private const string S_PLAYER_POS_Z     = "Player_Pos_Z";

    public static void Set_Player_Hp() => PlayerPrefs.SetInt(Current_World() + S_PLAYER_HP, PlayerStats.instance.hp);
    public static int Get_Player_Hp() => PlayerPrefs.GetInt(Current_World() + S_PLAYER_HP);



    public static void Set_Player_Hunger() => PlayerPrefs.SetInt(Current_World() + S_PLAYER_HUNGER, PlayerStats.instance.hunger);
    public static int Get_Player_Hunger() => PlayerPrefs.GetInt(Current_World() + S_PLAYER_HUNGER);



    public static void Set_Player_Speed() => PlayerPrefs.SetInt(Current_World() + S_PLAYER_SPEED, PlayerStats.instance.speed);
    public static int Get_Player_Speed() => PlayerPrefs.GetInt(Current_World() + S_PLAYER_SPEED);



    public static void Set_Player_Pos()
    {
        Set_Player_Pos_X();
        Set_Player_Pos_Z();
    }
    private static void Set_Player_Pos_X() => PlayerPrefs.SetFloat(Current_World() + S_PLAYER_POS_X, PlayerStats.instance.transform.position.x);
    private static void Set_Player_Pos_Z() => PlayerPrefs.SetFloat(Current_World() + S_PLAYER_POS_Z, PlayerStats.instance.transform.position.z);


    public static Vector3 Get_Player_Pos() => new Vector3(Get_Player_Pos_X(), 0, Get_Player_Pos_Z());
    private static float Get_Player_Pos_X() => PlayerPrefs.GetFloat(Current_World() + S_PLAYER_POS_X);
    private static float Get_Player_Pos_Z() => PlayerPrefs.GetFloat(Current_World() + S_PLAYER_POS_Z);


    #endregion

    #region Inventory

    private const string S_NR_OF_ITEMS_IN_INVENTORY = "NrItemsInInventory";
    private const string S_INVENTORY_ITEM           = "ItemParentSlot";
    private const string S_ITEM_PARENT_SLOT         = "ItemParentSlot";

    public static void Set_Nr_Of_Inventory_Items(int val) => PlayerPrefs.SetInt(Current_World() + S_NR_OF_ITEMS_IN_INVENTORY, val);
    public static int Get_Nr_Of_Inventory_Items() => PlayerPrefs.GetInt(Current_World() + S_NR_OF_ITEMS_IN_INVENTORY);



    public static void Set_Inventory_Item_Name(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_INVENTORY_ITEM + S_ITEM_NAME + key, value);
    public static int Get_Inventory_Item_Name(int key) => PlayerPrefs.GetInt(Current_World() + S_INVENTORY_ITEM + S_ITEM_NAME + key);


    public static void Set_Inventory_Item_Stack(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_INVENTORY_ITEM + S_ITEM_STACK + key, value);
    public static int Get_Inventory_Item_Stack(int key) => PlayerPrefs.GetInt(Current_World() + S_INVENTORY_ITEM + S_ITEM_STACK + key);

    public static void Set_Inventory_Item_Durability(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_INVENTORY_ITEM + S_ITEM_DURABILITY + key, value);
    public static int Get_Inventory_Item_Durability(int key) => PlayerPrefs.GetInt(Current_World() + S_INVENTORY_ITEM + S_ITEM_DURABILITY + key);


    public static void Set_Inventory_Item_Parent_Slot(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_INVENTORY_ITEM + S_ITEM_PARENT_SLOT + key, value);
    public static int Get_Inventory_Item_Parent_Slot(int key) => PlayerPrefs.GetInt(Current_World() + S_INVENTORY_ITEM + S_ITEM_PARENT_SLOT + key);

    #endregion



    #region Items

    private const string S_NR_OF_ITEMS      = "NrItems";
    private const string S_ITEM_NAME        = "ItemName";
    private const string S_ITEM_STACK       = "ItemStack";
    private const string S_ITEM_DURABILITY  = "ItemDurability";

    private const string S_ITEM_POS_X = "Item_Pos_X";
    private const string S_ITEM_POS_Z = "Item_Pos_Z";

    public static void Set_Nr_Of_Items() => PlayerPrefs.SetInt(Current_World() + S_NR_OF_ITEMS, WorldManager.instance.items.childCount);
    public static int Get_Nr_Of_Items() => PlayerPrefs.GetInt(Current_World() + S_NR_OF_ITEMS);



    public static void Set_Item_Name(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_ITEM_NAME + key, value);
    public static int Get_Item_Name(int key) => PlayerPrefs.GetInt(Current_World() + S_ITEM_NAME + key);


    public static void Set_Item_Stack(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_ITEM_STACK + key, value);
    public static int Get_Item_Stack(int key) => PlayerPrefs.GetInt(Current_World() + S_ITEM_STACK + key);

    public static void Set_Item_Durability(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_ITEM_DURABILITY + key, value);
    public static int Get_Item_Durability(int key) => PlayerPrefs.GetInt(Current_World() + S_ITEM_DURABILITY + key);



    public static void Set_Item_Pos(int key, float posX, float posZ)
    {
        Set_Item_Pos_X(key, posX);
        Set_Item_Pos_Z(key, posZ);
    }
    private static void Set_Item_Pos_X(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_ITEM_POS_X + key, value);
    private static void Set_Item_Pos_Z(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_ITEM_POS_Z + key, value);


    public static Vector3 Get_Item_Pos(int key) => new Vector3(Get_Item_Pos_X(key), 0, Get_Item_Pos_Z(key));
    private static float Get_Item_Pos_X(int key) => PlayerPrefs.GetFloat(Current_World() + S_ITEM_POS_X + key);
    private static float Get_Item_Pos_Z(int key) => PlayerPrefs.GetFloat(Current_World() + S_ITEM_POS_Z + key);


    #endregion


    #region Resources
    private const string S_NR_OF_RESOURCES          = "NrResources";
    private const string S_RESOURCE_NAME            = "ResourceName";
    private const string S_RESOURCE_HP              = "ResourceHp";

    private const string S_RESOURCE_TIME_TO_GROW    = "ResourceTimeToGrow";

    private const string S_RESOURCE_POS_X           = "Resource_Pos_X";
    private const string S_RESOURCE_POS_Z           = "Resource_Pos_Z";

    public static void Set_Nr_Of_Resources() => PlayerPrefs.SetInt(Current_World() + S_NR_OF_RESOURCES, WorldManager.instance.resources.childCount);
    public static int Get_Nr_Of_Resources() => PlayerPrefs.GetInt(Current_World() + S_NR_OF_RESOURCES);



    public static void Set_Resource_Name(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_RESOURCE_NAME + key, value);
    public static int Get_Resource_Name(int key) => PlayerPrefs.GetInt(Current_World() + S_RESOURCE_NAME + key);


    public static void Set_Resource_Hp(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_RESOURCE_HP + key, value);
    public static int Get_Resource_Hp(int key) => PlayerPrefs.GetInt(Current_World() + S_RESOURCE_HP + key);



    public static void Set_Resource_Time_To_Grow(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_RESOURCE_TIME_TO_GROW + key, value);
    public static float Get_Resource_Time_To_Grow(int key) => PlayerPrefs.GetFloat(Current_World() + S_RESOURCE_TIME_TO_GROW + key);



    public static void Set_Resource_Pos(int key,float posX,float posZ)
    {
        Set_Resource_Pos_X(key, posX);
        Set_Resource_Pos_Z(key, posZ);
    }
    private static void Set_Resource_Pos_X(int key,float value) => PlayerPrefs.SetFloat(Current_World() + S_RESOURCE_POS_X + key, value);
    private static void Set_Resource_Pos_Z(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_RESOURCE_POS_Z + key, value);


    public static Vector3 Get_Resource_Pos(int key) => new Vector3(Get_Resource_Pos_X(key), 0, Get_Resource_Pos_Z(key));
    private static float Get_Resource_Pos_X(int key) => PlayerPrefs.GetFloat(Current_World() + S_RESOURCE_POS_X + key);
    private static float Get_Resource_Pos_Z(int key) => PlayerPrefs.GetFloat(Current_World() + S_RESOURCE_POS_Z + key);

    #endregion

    #region Constructions

    private const string S_NR_OF_CONSTRUCTIONS = "NrConstructions";
    private const string S_CONSTRUCTION_NAME = "ConstructionName";

    private const string S_CONSTRUCTION_POS_X = "Construction_Pos_X";
    private const string S_CONSTRUCTION_POS_Z = "Construction_Pos_Z";


    private const string S_CONSTRUCTION_FIRE_TIME = "ConstructionFireTime";


    public static void Set_Nr_Of_Constructions() => PlayerPrefs.SetInt(Current_World() + S_NR_OF_CONSTRUCTIONS, WorldManager.instance.constructions.childCount);
    public static int Get_Nr_Of_Constructions() => PlayerPrefs.GetInt(Current_World() + S_NR_OF_CONSTRUCTIONS);



    public static void Set_Construction_Name(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_CONSTRUCTION_NAME + key, value);
    public static int Get_Construction_Name(int key) => PlayerPrefs.GetInt(Current_World() + S_CONSTRUCTION_NAME + key);



    public static void Set_Construction_Pos(int key, float posX, float posZ)
    {
        Set_Construction_Pos_X(key, posX);
        Set_Construction_Pos_Z(key, posZ);
    }
    private static void Set_Construction_Pos_X(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_CONSTRUCTION_POS_X + key, value);
    private static void Set_Construction_Pos_Z(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_CONSTRUCTION_POS_Z + key, value);


    public static Vector3 Get_Construction_Pos(int key) => new Vector3(Get_Construction_Pos_X(key), 0, Get_Construction_Pos_Z(key));
    private static float Get_Construction_Pos_X(int key) => PlayerPrefs.GetFloat(Current_World() + S_CONSTRUCTION_POS_X + key);
    private static float Get_Construction_Pos_Z(int key) => PlayerPrefs.GetFloat(Current_World() + S_CONSTRUCTION_POS_Z + key);

    public static void Set_Construction_Fire_Time(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_CONSTRUCTION_FIRE_TIME + key, value);
    public static float Get_Construction_Fire_Time(int key) => PlayerPrefs.GetFloat(Current_World() + S_CONSTRUCTION_FIRE_TIME + key);


    #endregion

    #region Mobs

    private const string S_NR_OF_MOBS   = "NrMobs";
    private const string S_MOB_NAME     = "MobName";
    private const string S_MOB_HP       = "MobHp";

    private const string S_MOB_SPAWNER_INDEX    = "MobSpawnerIndex";

    private const string S_MOB_POS_X    = "Mob_Pos_X";
    private const string S_MOB_POS_Z    = "Mob_Pos_Z";


    public static void Set_Nr_Of_Mobs() => PlayerPrefs.SetInt(Current_World() + S_NR_OF_MOBS, WorldManager.instance.mobs.childCount);
    public static int Get_Nr_Of_Mobs() => PlayerPrefs.GetInt(Current_World() + S_NR_OF_MOBS);



    public static void Set_Mob_Name(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_MOB_NAME + key, value);
    public static int Get_Mob_Name(int key) => PlayerPrefs.GetInt(Current_World() + S_MOB_NAME + key);


    public static void Set_Mob_Hp(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_MOB_HP + key, value);
    public static int Get_Mob_Hp(int key) => PlayerPrefs.GetInt(Current_World() + S_MOB_HP + key);


    public static void Set_Mob_Spawner_Index(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_MOB_SPAWNER_INDEX + key, value);
    public static float Get_Mob_Spawner_Index(int key) => PlayerPrefs.GetInt(Current_World() + S_MOB_SPAWNER_INDEX + key);



    public static void Set_Mob_Pos(int key, float posX, float posZ)
    {
        Set_Mob_Pos_X(key, posX);
        Set_Mob_Pos_Z(key, posZ);
    }
    private static void Set_Mob_Pos_X(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_MOB_POS_X + key, value);
    private static void Set_Mob_Pos_Z(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_MOB_POS_Z + key, value);


    public static Vector3 Get_Mob_Pos(int key) => new Vector3(Get_Mob_Pos_X(key), 0, Get_Mob_Pos_Z(key));
    private static float Get_Mob_Pos_X(int key) => PlayerPrefs.GetFloat(Current_World() + S_MOB_POS_X + key);
    private static float Get_Mob_Pos_Z(int key) => PlayerPrefs.GetFloat(Current_World() + S_MOB_POS_Z + key);


    #endregion

    #region Spawners

    private const string S_NR_OF_SPAWNERS       = "NrSpawners";
    private const string S_SPAWNER_NAME         = "SpawnerName";

    private const string S_SPAWNER_MOBS_RESPAWN_AMOUNT  = "SpawnerMobsAmount";
    private const string S_SPAWNER_MOBS_RESPAWN_TIME    = "SpawnerMobsTIME";

    private const string S_SPAWNER_POS_X        = "Spawner_Pos_X";
    private const string S_SPAWNER_POS_Z        = "Spawner_Pos_Z";


    public static void Set_Nr_Of_Spawners() => PlayerPrefs.SetInt(Current_World() + S_NR_OF_SPAWNERS, WorldManager.instance.mobSpawners.childCount);
    public static int Get_Nr_Of_Spawners() => PlayerPrefs.GetInt(Current_World() + S_NR_OF_SPAWNERS);



    public static void Set_Spawner_Name(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_SPAWNER_NAME + key, value);
    public static int Get_Spawner_Name(int key) => PlayerPrefs.GetInt(Current_World() + S_SPAWNER_NAME + key);


    public static void Set_Spawner_Mobs_Amount(int key, int value) => PlayerPrefs.SetInt(Current_World() + S_SPAWNER_MOBS_RESPAWN_AMOUNT + key, value);
    public static int Get_Spawner_Mobs_Amount(int key) => PlayerPrefs.GetInt(Current_World() + S_SPAWNER_MOBS_RESPAWN_AMOUNT + key);


    public static void Set_Spawner_Mobs_Respawn_Time(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_SPAWNER_MOBS_RESPAWN_TIME + key, value);
    public static float Get_Spawner_Mobs_Respawn_Time(int key) => PlayerPrefs.GetFloat(Current_World() + S_SPAWNER_MOBS_RESPAWN_TIME + key);





    public static void Set_Spawner_Pos(int key, float posX, float posZ)
    {
        Set_Spawner_Pos_X(key, posX);
        Set_Spawner_Pos_Z(key, posZ);
    }
    private static void Set_Spawner_Pos_X(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_SPAWNER_POS_X + key, value);
    private static void Set_Spawner_Pos_Z(int key, float value) => PlayerPrefs.SetFloat(Current_World() + S_SPAWNER_POS_Z + key, value);


    public static Vector3 Get_Spawner_Pos(int key) => new Vector3(Get_Spawner_Pos_X(key), 0, Get_Spawner_Pos_Z(key));
    private static float Get_Spawner_Pos_X(int key) => PlayerPrefs.GetFloat(Current_World() + S_SPAWNER_POS_X + key);
    private static float Get_Spawner_Pos_Z(int key) => PlayerPrefs.GetFloat(Current_World() + S_SPAWNER_POS_Z + key);

    #endregion

    #endregion







}
