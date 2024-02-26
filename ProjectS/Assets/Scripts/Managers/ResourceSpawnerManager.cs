using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawnerManager : MonoBehaviour
{
    private static System.Random rnd = new System.Random();
    private int maxItemsForType = 100;

    IEnumerator Start()
    {
        yield return null; // Wait for a frame
     
        if (PlayerPrefs.GetInt("prevWorld") <= 1)
        {
            SpawnItems(ObjectName.flint);
            SpawnItems(ObjectName.seeds);
         
            SpawnResources(ObjectName.sappling);
            SpawnResources(ObjectName.tree);
            SpawnResources(ObjectName.rock);
            SpawnResources(ObjectName.grassBush);

            SpawnResources(ObjectName.berryBush);

            SpawnResources(ObjectName.redShroom);
            SpawnResources(ObjectName.greenShroom);
            SpawnResources(ObjectName.blueShroom);
        }
    }

    void SpawnItems(ObjectName _name)
    {
        int nr = rnd.Next(maxItemsForType / 2, maxItemsForType);

        for (int i = 0; i < nr; i++)
        {
            Item item = ItemsManager.instance.CreateItem(_name);

            item.transform.SetParent(WorldManager.instance.items.transform);
            item.transform.position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));

        }
    }

    void SpawnResources(ObjectName _name)
    {
        int nr = rnd.Next(maxItemsForType / 2, maxItemsForType);

        for (int i = 0; i < nr; i++)
        {
            Resource resource = Instantiate(ItemsManager.instance.SearchResourcesList(_name));

            resource.transform.SetParent(WorldManager.instance.resources.transform);
            resource.transform.position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            resource.name = _name;
        }
    }
}
