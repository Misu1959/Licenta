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
        return (int)(worldSettingsDictionary[WorldSettingName.WorldSize][SettingValue.normal] * 
                ((float)spawnSettingsDictionary[objectType][currentSettingValue] / 100) * 
                ((float)objectSpawnRarity / 100));

    }

    public override void SaveSetting(int settingNumber)
    {
        PlayerPrefs.SetInt(SaveData.SPAWN_SETTING_NAME + settingNumber, (int)objectName);
        PlayerPrefs.SetInt(SaveData.SPAWN_SETTING_VALUE + settingNumber, CalculateSetting());

    }


}
