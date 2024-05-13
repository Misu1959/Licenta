using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldSettingsManager;

public class SpawnSettingPrefab : SettingPrefab
{

    [Header("Spawn setting data")]

    [SerializeField] protected ObjectName objectName;
    [SerializeField] protected ObjectType objectType;
    [SerializeField] protected ObjectSpawnRarity objectSpawnRarity;


    protected override void DisplayName() => labelName.text = objectName.ToString();


    int CalculateSetting()
    {
        return  (int) (SaveLoadManager.Get_World_Size() / 2 * 
                ((float)spawnSettingsDictionary[objectType][currentSettingValue] / 100) * 
                ((float)objectSpawnRarity / 100));

    }

    public override void SaveSetting(int settingNumber)
    {
        SaveLoadManager.Set_Spawn_Setting_Name(settingNumber, (int)objectName);
        SaveLoadManager.Set_Spawn_Setting_Value(settingNumber, CalculateSetting());
    }


}
