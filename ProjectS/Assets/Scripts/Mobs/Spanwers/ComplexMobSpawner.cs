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

        if (!respawnTimer.IsElapsed()) return;
        
        GameObject newMob = Instantiate(ItemsManager.instance.SearchMobsList(mobName).gameObject);
        newMob.GetComponent<MobStats>().SetSpawner(this.transform);

        newMob.transform.position = transform.position;
        newMob.transform.SetParent(WorldManager.instance.mobs);
        mobsList.Add(newMob);


    }

}
