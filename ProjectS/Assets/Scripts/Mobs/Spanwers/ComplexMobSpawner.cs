using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexMobSpawner : MobSpawner
{
    private List<MobStats> mobsList = new List<MobStats>();
    public override void AddMobToList(MobStats mob) => mobsList.Add(mob); 


    [SerializeField] private Timer respawnTimer;
    public float GetRespawnTimer_TimeRemained() => respawnTimer.RemainedTime();


    private void Update() => SpawnMobOnTime();

    public override void SetSpawnerData() 
    {
        base.SetSpawnerData();
        respawnTimer.SetTime(respawnTimer.MaxTime());
    }

    public override void SetSpawnerData(int _mobsToSpawn, float respawnTimer_RemainedTime)
    {
        base.SetSpawnerData(_mobsToSpawn,respawnTimer_RemainedTime);
        respawnTimer.SetTime(respawnTimer_RemainedTime);
    }



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
            newMob.SetMobData(this.transform);


            newMob.transform.position = transform.position;
            newMob.transform.SetParent(WorldManager.instance.mobs);
        }
        mobsToSpawn = 0;

    }

}
