using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public ObjectName objectName;

    [SerializeField] protected ObjectName mobName;
    [SerializeField] protected int maxNumberOfMobs;
    public int mobsToSpawn { get; protected set; }
    public virtual void AddMobToList(MobStats mob) { } // it's use is in complex mob spawner

    public virtual void SetSpawnerData()
    {
        mobsToSpawn = maxNumberOfMobs;
        SpawnMobs();
    }

    public virtual void SetSpawnerData(int _mobsToSpawn, float respawnTimer_RemainedTime)
    {
        mobsToSpawn = _mobsToSpawn;
        SpawnMobs();
    }

    protected virtual void SpawnMobs()
    {
        if(mobsToSpawn <= 0)
        {
            Destroy(this.gameObject);
            return;
        }

        MobStats newMob = Instantiate(ItemsManager.instance.GetOriginalMob(mobName));

        newMob.transform.position = transform.position;
        newMob.transform.SetParent(WorldManager.instance.mobs);
        
        newMob.SetMobData(this.transform);


        mobsToSpawn--;
        Invoke(nameof(SpawnMobs), .5f);
    }
}
