using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawnerManager : MonoBehaviour
{
    private static System.Random rnd = new System.Random();
    private int maxItemsForType = 20;

    IEnumerator Start()
    {
        yield return null; // Wait for a frame
     
        if (PlayerPrefs.GetInt("prevWorld") <= 1)
        {
            SpawnItems(Item.Name.flint);

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

    void SpawnItems(Item.Name _name)
    {
        int nr = rnd.Next(maxItemsForType / 2, maxItemsForType);

        for (int i = 0; i < nr; i++)
        {
            Item item = Instantiate(ItemsManager.instance.SearchItemsList(_name)).GetComponent<Item>();
            item.name = _name;
            item.AddToStack(1);

            item.transform.SetParent(SaveLoadManager.instance.items.transform);
            item.transform.position = new Vector2(Random.Range(-25, 25), Random.Range(-25, 25));
        
        }
    }

    void SpawnResources(Resource.Name _name)
    {
        int nr = rnd.Next(maxItemsForType / 2, maxItemsForType);

        for (int i = 0; i < nr; i++)
        {
            Resource resource = Instantiate(ItemsManager.instance.SearchResourcesList(_name)).GetComponent<Resource>();

            resource.transform.SetParent(SaveLoadManager.instance.resources.transform);
            resource.transform.position = new Vector2(Random.Range(-25, 25), Random.Range(-25, 25));
            resource.name = _name;
        }
    }
}
