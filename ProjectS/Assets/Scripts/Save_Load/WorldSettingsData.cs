using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSettingsData : MonoBehaviour
{
    public enum WorldSize
    {
        small = 50,
        noral = 100,
        large = 200
    };
    public enum ItemSpawnRate
    {
        none = 0,
        few = 20,
        normal = 40,
        more = 60
    };
    public enum ResourceSpawnRate
    {
        none = 0,
        few = 10,
        normal = 20,
        more = 30
    };

    public enum SpawnersSpawnRate
    {
        none = 0,
        few = 5,
        normal = 10,
        more = 15
    };
}
