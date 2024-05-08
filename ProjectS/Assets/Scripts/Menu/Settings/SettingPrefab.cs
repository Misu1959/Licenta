using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static WorldSettingsManager;

public abstract class SettingPrefab : MonoBehaviour
{

    [Header("Setting prefab")]
    [SerializeField] protected TextMeshProUGUI labelName;
    [SerializeField] protected TextMeshProUGUI labelValue;
    [SerializeField] protected Image image;
    [SerializeField] protected Button leftButton;
    [SerializeField] protected Button rightButton;

    [Header("Setting data")]
    
    [SerializeField] protected SettingValuesSet settingSet;
    protected SettingValue currentSettingValue = SettingValue.normal;
    
    private void Awake() => InitializePrefab();

    private void InitializePrefab()
    {
        leftButton.onClick.AddListener(() => LowerSetting());
        rightButton.onClick.AddListener(() => GreaterSetting());

        DisplayName();
        DisplayValue();
    }

    private void LowerSetting()
    {
        currentSettingValue--;
        leftButton.interactable  = CheckValueWithinRange(-1);
        rightButton.interactable = CheckValueWithinRange(1);

        DisplayValue();
    }
    private void GreaterSetting()
    {
        currentSettingValue++;
        leftButton.interactable = CheckValueWithinRange(-1);
        rightButton.interactable = CheckValueWithinRange(1);

        DisplayValue();
    }


    
    public void ResetSetting()
    {
        currentSettingValue = SettingValue.normal;
        leftButton.interactable = CheckValueWithinRange(-1);
        rightButton.interactable = CheckValueWithinRange(1);

        DisplayValue();
    }
    private bool CheckValueWithinRange(int val) => settingValuesDictionary[settingSet].ContainsKey(currentSettingValue + val) ? true : false;

    protected abstract void DisplayName();
    private void DisplayValue() => labelValue.text = settingValuesDictionary[settingSet][currentSettingValue].ToString();

    public abstract void SaveSetting(int settingNumber = -1);

}
