using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    [SerializeField] private GameObject world;

    public GameObject items { get; private set; }
    public GameObject resources { get; private set; }
    public GameObject constructions { get; private set; }
    public GameObject mobs { get; private set; }
    public GameObject mobSpawners { get; private set; }

    void Start()
    {
        instance = this;

        items           = world.transform.GetChild(2).gameObject;
        resources       = world.transform.GetChild(3).gameObject;
        constructions   = world.transform.GetChild(4).gameObject;
        mobs            = world.transform.GetChild(5).gameObject;
        mobSpawners     = world.transform.GetChild(6).gameObject;
    }

    public void SendMobsToSleep()
    {
        for (int i = 0; i < mobSpawners.transform.childCount; i++)
            mobSpawners.transform.GetChild(i).GetComponent<ComplexMobSpawner>().GetMobsInside();

        for (int i = 0; i < constructions.transform.childCount; i++)
            constructions.transform.GetChild(i).GetComponent<ComplexMobSpawner>()?.GetMobsInside();
    }

    public void WakeUpMobs()
    {
        for (int i = 0; i < mobSpawners.transform.childCount; i++)
            mobSpawners.transform.GetChild(i).GetComponent<ComplexMobSpawner>().GetMobsOutside();

        for (int i = 0; i < constructions.transform.childCount; i++)
            constructions.transform.GetChild(i).GetComponent<ComplexMobSpawner>()?.GetMobsOutside();

    }
}
