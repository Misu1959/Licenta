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
            newMob.GetComponent<MobController>().spawner = this.transform;

            newMob.transform.position = transform.position;
            newMob.transform.SetParent(WorldManager.instance.mobs.transform);
            mobsList.Add(newMob);

        }

    }

    public void GetMobsInside()
    {
        foreach (GameObject mob in mobsList)
            mob.GetComponent<MobActionManagement>().SetTargetAndAction(this.gameObject, MobActionManagement.Action.goInside);
    }

    public void GetMobsOutside()
    {
        foreach (GameObject mob in mobsList)
        {
            mob.SetActive(true);
            mob.GetComponent<MobController>().SetTargetPosition();
        }
    }

}
