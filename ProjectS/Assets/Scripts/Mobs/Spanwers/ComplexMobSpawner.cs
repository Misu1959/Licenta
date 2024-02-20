using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexMobSpawner : MobSpawner
{
    [SerializeField] private ObjectName mobName;
    [SerializeField] private int maxNumberOfMobs;
    
    private List<GameObject> mobsList = new List<GameObject>();

    private Timer respawnTimer;

    private void Start()
    {
        respawnTimer = new Timer(3);
    }

    private void Update()
    {
        SpawnMob();
    }

    private void SpawnMob()
    {
        if(mobsList.Count == maxNumberOfMobs)
        {
            respawnTimer.RestartTimer();
            return;
        }

        respawnTimer.StartTimer();
        respawnTimer.Tick();

        if (respawnTimer.IsElapsed())
        {
            GameObject newMob = Instantiate(ItemsManager.instance.SearchMobsList(mobName).gameObject);

            newMob.transform.position = transform.position;
            newMob.transform.SetParent(SaveLoadManager.instance.mobs.transform);
            mobsList.Add(newMob);

        }

    }

    private void GetMobsInside()
    {

    }

    private void GetMobsOutside()
    {

    }




}
