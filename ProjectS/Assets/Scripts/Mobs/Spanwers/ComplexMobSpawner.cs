using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexMobSpawner : MobSpawner
{
    private List<MobStats> mobsList = new List<MobStats>();
    [SerializeField] private Timer respawnTimer;

    private int mobsToSpawn;

    private IEnumerator Start()
    {
        yield return null;
        mobsToSpawn = maxNumberOfMobs;
        SpawnMobs();
    }
    private void Update() => SpawnMobOnTime();

    private void SpawnMobOnTime()
    {
        if (mobsList.Count == maxNumberOfMobs)
        {
            respawnTimer.RestartTimer();
            return;
        }

        respawnTimer.StartTimer();
        respawnTimer.Tick();

        if (!respawnTimer.IsElapsed()) return;

        if (ItemsManager.instance.GetOriginalMob(mobName).GetSleepPeriod() == TimeManager.instance.dayState)
            respawnTimer.SetTime(.001f);
        else
        {
            if (mobsToSpawn + mobsList.Count < maxNumberOfMobs)
                mobsToSpawn++;

            SpawnMobs();
        }
    }

    protected override void SpawnMobs()
    {
        if (ItemsManager.instance.GetOriginalMob(mobName).GetSleepPeriod() == TimeManager.instance.dayState) return;


        for (int i = 0; i < mobsToSpawn; i++)
        {
            MobStats newMob = Instantiate(ItemsManager.instance.GetOriginalMob(mobName));
            newMob.SetSpawner(this.transform);

            newMob.transform.position = transform.position;
            newMob.transform.SetParent(WorldManager.instance.mobs);
            mobsList.Add(newMob);
        }
        mobsToSpawn = 0;

    }

}
