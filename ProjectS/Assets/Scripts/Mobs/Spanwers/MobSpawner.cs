using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public ObjectName objectName;

    [SerializeField] protected ObjectName mobName;
    [SerializeField] protected int maxNumberOfMobs;

    private void Start() => Invoke(nameof(SpawnMobs), .5f);

    protected virtual void SpawnMobs()
    {

        MobStats newMob = Instantiate(ItemsManager.instance.GetOriginalMob(mobName));
        newMob.SetSpawner(this.transform);

        newMob.transform.position = transform.position;
        newMob.transform.SetParent(WorldManager.instance.mobs);

        maxNumberOfMobs--;
        Invoke(nameof(SpawnMobs), .5f);
    }
}
