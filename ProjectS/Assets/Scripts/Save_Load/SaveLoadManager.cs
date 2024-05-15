using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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

#region Time

    private const string S_CURRENT_TIME = "CurrentTime";
    private const string S_CURRENT_HOUR = "CurrentHour";
    private const string S_CURRENT_DAY = "CurrentDay";
    private const string S_CURRENT_DAY_STATE = "CurrentDayState";

    public static void Set_Current_Time() => PlayerPrefs.SetInt(Current_World() + S_CURRENT_TIME, (int)TimeManager.instance.currentTime);
    public static int Get_Current_Time() => PlayerPrefs.GetInt(Current_World() + S_CURRENT_TIME);

    public static void Set_Current_Hour() => PlayerPrefs.SetInt(Current_World() + S_CURRENT_HOUR, TimeManager.instance.currentHour);
    public static int Get_Current_Hour() => PlayerPrefs.GetInt(Current_World() + S_CURRENT_HOUR);


    public static void Set_Current_Day() => PlayerPrefs.SetInt(Current_World() + S_CURRENT_DAY, TimeManager.instance.currentDay);
    public static int Get_Current_Day() => PlayerPrefs.GetInt(Current_World() + S_CURRENT_DAY);
    public static int Get_World_Day(int key) => PlayerPrefs.GetInt(World(key) + S_CURRENT_DAY);


    public static void Set_Current_DayState() => PlayerPrefs.SetInt(Current_World() + S_CURRENT_DAY_STATE, (int)TimeManager.instance.dayState);
    public static int Get_Current_DayState() => PlayerPrefs.GetInt(Current_World() + S_CURRENT_DAY_STATE);


    #endregion
    
#region World

    public enum Paths
    {
        Player,

        InventoryItem,
        SelectedItem,
        Equipment,

        StoredItem,

        Item,
        Resource,
        Construction,
        Mob,
        Spawner
    }

    private const string S_AMOUNT_OF                = "Amount_of";
    private const string S_NAME                     = "Name";
    private const string S_POS_X                    = "Pos_X";
    private const string S_POS_Z                    = "Pos_Z";


    private const string S_HP                       = "Hp";
    private const string S_HUNGER                   = "Hunger";
    private const string S_DMG                      = "Dmg";
    private const string S_SPEED                    = "Speed";

    private const string S_ITEM_UI_parent_slot          = "ParentSlot";

    private const string S_ITEM_stack                   = "Stack";
    private const string S_ITEM_durability              = "Durability";
    private const string S_RESOUCER_time_to_grow        = "TimeToGrow";
    private const string S_CONSTRUCTION_fire_time       = "FireTime";
    private const string S_MOB_spawner_index            = "SpawnerIndex";
    private const string S_SPAWNER_mobs_respawn_amount  = "MobsAmount";
    private const string S_SPAWNER_mobs_respawn_time    = "MobsRespawnTime";


    public static void Set_Amount_Of_Objects(string path, int value) => PlayerPrefs.SetInt(Current_World() + S_AMOUNT_OF + path, value);
    public static int Get_Amount_Of_Objects(string path) => PlayerPrefs.GetInt(Current_World() + S_AMOUNT_OF + path);

    public static void Set_Object_Name(string path,  ObjectName name) => PlayerPrefs.SetInt(Current_World() + path  + S_NAME, (int)name);
    public static ObjectName Get_Object_Name(string path) => (ObjectName)PlayerPrefs.GetInt(Current_World() + path  + S_NAME);


    public static void Set_Object_Pos(string path,  Vector3 pos)
    {
        Set_Object_Pos_X(path, pos.x);
        Set_Object_Pos_Z(path, pos.z);
    }
    private static void Set_Object_Pos_X(string path,  float value) => PlayerPrefs.SetFloat(Current_World() + path  + S_POS_X, value);
    private static void Set_Object_Pos_Z(string path,  float value) => PlayerPrefs.SetFloat(Current_World() + path  + S_POS_Z, value);


    public static Vector3 Get_Object_Pos(string path) => new Vector3(Get_Object_Pos_X(path), 0, Get_Object_Pos_Z(path));
    private static float Get_Object_Pos_X(string path) => PlayerPrefs.GetFloat(Current_World() + path  + S_POS_X);
    private static float Get_Object_Pos_Z(string path) => PlayerPrefs.GetFloat(Current_World() + path  + S_POS_Z);



    public static void Set_Object_Hp(string path, int value) => PlayerPrefs.SetInt(Current_World() + path  + S_HP, value);
    public static int Get_Object_Hp(string path) => PlayerPrefs.GetInt(Current_World() + path  + S_HP);

    public static void Set_Object_Hunger(string path,  int value) => PlayerPrefs.SetInt(Current_World() + path  + S_HUNGER, value);
    public static int Get_Object_Hunger(string path) => PlayerPrefs.GetInt(Current_World() + path  + S_HUNGER);

    public static void Set_Object_Dmg(string path,  int value) => PlayerPrefs.SetInt(Current_World() + path  + S_DMG, value);
    public static int Get_Object_Dmg(string path) => PlayerPrefs.GetInt(Current_World() + path  + S_DMG);

    public static void Set_Object_Speed(string path,  int value) => PlayerPrefs.SetInt(Current_World() + path  + S_SPEED, value);
    public static int Get_Object_Speed(string path) => PlayerPrefs.GetInt(Current_World() + path  + S_SPEED);




    public static void Set_ITEM_Stack(string path,  int value) => PlayerPrefs.SetInt(Current_World() + path  + S_ITEM_stack, value);
    public static int Get_ITEM_Stack(string path) => PlayerPrefs.GetInt(Current_World() + path  + S_ITEM_stack);

    public static void Set_ITEM_Durability(string path,  int value) => PlayerPrefs.SetInt(Current_World() + path  + S_ITEM_durability, value);
    public static int Get_ITEM_Durability(string path) => PlayerPrefs.GetInt(Current_World() + path  + S_ITEM_durability);




    public static void Set_RESOURCE_Time_To_Grow(string path, float value) => PlayerPrefs.SetFloat(Current_World() + path + S_RESOUCER_time_to_grow, value);
    public static float Get_RESOURCE_Time_To_Grow(string path) => PlayerPrefs.GetFloat(Current_World() + path + S_RESOUCER_time_to_grow);



    public static void Set_CONSTRUCTION_Fire_Time(string path, float value) => PlayerPrefs.SetFloat(Current_World() + path + S_CONSTRUCTION_fire_time, value);
    public static float Get_CONSTRUCTION_Fire_Time(string path) => PlayerPrefs.GetFloat(Current_World() + path + S_CONSTRUCTION_fire_time);



    public static void Set_MOB_Spawner_Index(string path, float value) => PlayerPrefs.SetFloat(Current_World() + path  + S_MOB_spawner_index, value);
    public static float Get_MOB_Spawner_Index(string path) => PlayerPrefs.GetInt(Current_World() + path + S_MOB_spawner_index);




    public static void Set_SPAWNER_Mobs_Amount(string path, int value) => PlayerPrefs.SetInt(Current_World() + path + S_SPAWNER_mobs_respawn_amount, value);
    public static int Get_SPAWNER_Mobs_Amount(string path) => PlayerPrefs.GetInt(Current_World() + path + S_SPAWNER_mobs_respawn_amount);

    public static void Set_SPAWNER_Mobs_Respawn_Time(string path, float value) => PlayerPrefs.SetFloat(Current_World() + path + S_SPAWNER_mobs_respawn_time, value);
    public static float Get_SPAWNER_Mobs_Respawn_Time(string path) => PlayerPrefs.GetFloat(Current_World() + path + S_SPAWNER_mobs_respawn_time);



    public static void Set_ITEM_UI_Parent_Slot(string path,  int value) => PlayerPrefs.SetInt(Current_World() + path  + S_ITEM_UI_parent_slot, value);
    public static int Get_ITEM_UI_Parent_Slot(string path) => PlayerPrefs.GetInt(Current_World() + path  + S_ITEM_UI_parent_slot);


    public static void Set_Has_Selected_Item(string path, bool value)
    {
        int aux = (value == false) ? 0 : 1;
        PlayerPrefs.SetInt(Current_World() + path, aux);
    }
    public static bool Get_Has_Selected_Item(string path) => (PlayerPrefs.GetInt(Current_World() + path) == 0) ? false : true;


    public static void Set_Has_Equipment(string path, bool value)
    {
        int aux = (value == false) ? 0 : 1;
        PlayerPrefs.SetInt(Current_World() + path , aux);
    }
    public static bool Get_Has_Equipment(string path) => PlayerPrefs.GetInt(Current_World() + path) == 0 ? false : true;

    #endregion
}
