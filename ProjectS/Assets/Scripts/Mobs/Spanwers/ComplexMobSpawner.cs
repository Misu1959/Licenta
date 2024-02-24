using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexMobSpawner : MobSpawner
{
    private List<GameObject> mobsList = new List<GameObject>();
    [SerializeField] private Timer respawnTimer;

    private void Update() => SpawnMob();

    protected override void SpawnMob()
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
        //foreach(GameObject mob in mobsList)
           // mob.GetComponent<MobController>().SetTarget()
    }

    private void GetMobsOutside()
    {

    }




}
