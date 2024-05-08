using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldSettingsManager : MonoBehaviour
{
    #region World Settings 
    public enum WorldSettingName
    {
        WorldSize,
        DayDuration,
        DayLength,
        DawnLength,
        NightLength
    }

    private static Dictionary<SettingValue, int> worldSizeDictionary = new Dictionary<SettingValue, int>()
    {
        { SettingValue.less,    50  },
        { SettingValue.normal,  100  },
        { SettingValue.more,    200  }

    };

    private static Dictionary<SettingValue, int> dayDurationDictionary = new Dictionary<SettingValue, int>()
    {
        { SettingValue.less,    120  },
        { SettingValue.normal,  240  },
        { SettingValue.more,    360  }

    };

    private static Dictionary<SettingValue, int> dayLengthDictionary = new Dictionary<SettingValue, int>()
    {
        { SettingValue.none,    0  },
        { SettingValue.less,    120  },
        { SettingValue.normal,  240  },
        { SettingValue.more,    360  }

    };

    private static Dictionary<SettingValue, int> dawnLengthDictionary = new Dictionary<SettingValue, int>()
    {
        { SettingValue.none,    0  },
        { SettingValue.less,    120  },
        { SettingValue.normal,  240  },
        { SettingValue.more,    360  }

    };

    private static Dictionary<SettingValue, int> nightLengthDictionary = new Dictionary<SettingValue, int>()
    {
        { SettingValue.none,    0  },
        { SettingValue.less,    120  },
        { SettingValue.normal,  240  },
        { SettingValue.more,    360  }

    };


    public static Dictionary<WorldSettingName, Dictionary<SettingValue, int>> worldSettingsDictionary = new Dictionary<WorldSettingName, Dictionary<SettingValue, int>>()
    {
        {WorldSettingName.WorldSize,    worldSizeDictionary},
        {WorldSettingName.DayDuration,  dayDurationDictionary},
        {WorldSettingName.DayLength,    dayLengthDictionary},
        {WorldSettingName.DawnLength,   dawnLengthDictionary},
        {WorldSettingName.NightLength,  nightLengthDictionary}


    };
    #endregion

    #region Spawn Settings
    public enum ObjectSpawnRarity
    {
        common = 100,
        uncommon = 75,
        rare = 40
    }

    private static Dictionary<SettingValue, int> itemsSpawnRate = new Dictionary<SettingValue, int>()
    {
        { SettingValue.none,    0   },
        { SettingValue.less,    20  },
        { SettingValue.normal,  40  },
        { SettingValue.more,    60  }

    };

    private static Dictionary<SettingValue, int> resourceSpawnRate = new Dictionary<SettingValue, int>()
    {
        { SettingValue.none,    0   },
        { SettingValue.less,    10  },
        { SettingValue.normal,  20  },
        { SettingValue.more,    30  }

    };

    private static Dictionary<SettingValue, int> spawnersSpawnRate = new Dictionary<SettingValue, int>()
    {
        { SettingValue.none,    0   },
        { SettingValue.less,    5  },
        { SettingValue.normal,  10  },
        { SettingValue.more,    15  }

    };

    public static Dictionary<ObjectType, Dictionary<SettingValue, int>> spawnSettingsDictionary = new Dictionary<ObjectType, Dictionary<SettingValue, int>>()
    {
        {ObjectType.item,      itemsSpawnRate},
        {ObjectType.resource,  resourceSpawnRate},
        {ObjectType.spawner,   spawnersSpawnRate}



    };
    #endregion

    #region Setting Values
    

    public enum SettingValuesSet
    {
        worldSize,
        dayDuration,
        periodlength,
        
        spawnAmount
    }
    public enum SettingValue
    {
        none,
        less,
        normal,
        more
    }

    private static Dictionary<SettingValue, string> worldSizeValuesDictionary = new Dictionary<SettingValue, string>()
    {
        {SettingValue.less, "Smaller" },
        {SettingValue.normal, "Normal" },
        {SettingValue.more, "Bigger" }
    };
    private static Dictionary<SettingValue, string> dayDurationValuesDictionary = new Dictionary<SettingValue, string>()
    {
        {SettingValue.less, "Shorther" },
        {SettingValue.normal, "Normal" },
        {SettingValue.more, "Longer" }
    };
    private static Dictionary<SettingValue, string> periodlengthValuesDictionary = new Dictionary<SettingValue, string>()
    {
        {SettingValue.none, "No" },
        {SettingValue.less, "Shorter" },
        {SettingValue.normal, "Normal" },
        {SettingValue.more, "Longer" }
    };
    private static Dictionary<SettingValue, string> spawnAmountValuesDictionary = new Dictionary<SettingValue, string>()
    {
        {SettingValue.none, "None" },
        {SettingValue.less, "Less" },
        {SettingValue.normal, "Normal" },
        {SettingValue.more, "More" }
    };

    public static Dictionary<SettingValuesSet, Dictionary<SettingValue, string>> settingValuesDictionary = new Dictionary<SettingValuesSet, Dictionary<SettingValue, string>>()
    {
        { SettingValuesSet.worldSize, worldSizeValuesDictionary},
        { SettingValuesSet.dayDuration, dayDurationValuesDictionary},
        { SettingValuesSet.periodlength, periodlengthValuesDictionary},
        { SettingValuesSet.spawnAmount, spawnAmountValuesDictionary}
    };

    #endregion


    static SettingPrefab[] settingPrefabs;

    private void Awake() => settingPrefabs = GameObject.FindObjectsOfType<SettingPrefab>();
    public static void SaveAllSetings()
    {

        int x = 0;
        for(int i=0;i<settingPrefabs.Length;i++)
            if (settingPrefabs[i].GetComponent<SpawnSettingPrefab>())
                settingPrefabs[i].SaveSetting(x++);
            else
                settingPrefabs[i].SaveSetting();

        PlayerPrefs.SetInt(SaveData.NR_SPAWN_SETTINGS, x);
    }

    public static void ResetAllSettings()
    {
        for (int i = 0; i < settingPrefabs.Length; i++)
            settingPrefabs[i].ResetSetting();
    }

}
