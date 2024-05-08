using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using static WorldSettingsManager;

public class WorldSettingPrefab : SettingPrefab
{
    [Header("World setting data")]
    [SerializeField] protected WorldSettingName settingName;

    protected override void DisplayName() => labelName.text = settingName.ToString();

    public override void SaveSetting(int settingNumber) => PlayerPrefs.SetInt(settingName.ToString(), worldSettingsDictionary[settingName][currentSettingValue]);

}