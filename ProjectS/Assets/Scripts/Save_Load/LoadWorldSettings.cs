using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadWorldSettings : MonoBehaviour
{
    public static LoadWorldSettings instance { get;private set; }

    public WorldSettingsData.WorldSize worldSize { get; private set; }

    void Start() => LoadWorldSettingsMethod();

    private void LoadWorldSettingsMethod()
    {
        instance = this;

        worldSize = (WorldSettingsData.WorldSize)PlayerPrefs.GetInt("WorldSize");










    }
}
