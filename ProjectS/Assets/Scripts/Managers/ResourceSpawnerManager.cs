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
            SpawnItems(ItemData.Name.flint);
            SpawnItems(ItemData.Name.seeds);
         
            SpawnResources(Resource.Name.sappling);
            SpawnResources(Resource.Name.tree);
            SpawnResources(Resource.Name.rock);
            SpawnResources(Resource.Name.grassBush);

            SpawnResources(Resource.Name.berryBush);

            SpawnResources(Resource.Name.redShroom);
            SpawnResources(Resource.Name.greenShroom);
            SpawnResources(Resource.Name.blueShroom);
        }
    }

    void SpawnItems(ItemData.Name _name)
    {
        int nr = rnd.Next(maxItemsForType / 2, maxItemsForType);

        for (int i = 0; i < nr; i++)
        {
            Item item = ItemsManager.instance.CreateItem(_name);

            item.transform.SetParent(SaveLoadManager.instance.items.transform);
            item.transform.position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));

        }
    }

    void SpawnResources(Resource.Name _name)
    {
        int nr = rnd.Next(maxItemsForType / 2, maxItemsForType);

        for (int i = 0; i < nr; i++)
        {
            Resource resource = Instantiate(ItemsManager.instance.SearchResourcesList(_name));

            resource.transform.SetParent(SaveLoadManager.instance.resources.transform);
            resource.transform.position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            resource.name = _name;
        }
    }
}
