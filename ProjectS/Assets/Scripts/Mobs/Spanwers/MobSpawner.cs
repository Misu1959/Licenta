using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] protected ObjectName mobName;
    [SerializeField] protected int maxNumberOfMobs;

    private void Start() => Invoke(nameof(SpawnMob), .5f);
    

    protected virtual void SpawnMob()
    {
        if (maxNumberOfMobs <= 0) return;

        maxNumberOfMobs--;


        GameObject newMob = Instantiate(ItemsManager.instance.SearchMobsList(mobName).gameObject);

        newMob.transform.position = transform.position;
        newMob.transform.SetParent(WorldManager.instance.mobs.transform);

        Invoke(nameof(SpawnMob), .5f);

    }
}
