using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldSettingsManager;

public static class SaveLoadManager 
{


    private const string S_WORLD                    = "WORLD";
    private const string S_OLD_WORLD                = "OLD_WORLD";
        
    private const string S_LAST_WORLD               = "LastWorld";
    private const string S_SELECTED_WORLD           = "SelectedWorld";

    #region Rebinds

    private static string S_NR_REBINDS_CHANGES = "rebinds changes";
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







}
