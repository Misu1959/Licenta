using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawnerManager : MonoBehaviour
{
    private static System.Random rnd = new System.Random();
    private int maxItemsForType = 20;

    IEnumerator Start()
    {
        yield return null;
     
        if (PlayerPrefs.GetInt("prevWorld") <= 1)
        {
            SpawnItems("Flint");

            SpawnResources("Tree");
            SpawnResources("Sapling");
            SpawnResources("Rock");
            SpawnResources("GrassBush");

            SpawnResources("BerryBush");

            SpawnResources("RedShroom");
            SpawnResources("BlueShroom");
            SpawnResources("GreenShroom");

        }
    }

    void SpawnItems(string type)
    {
        int nr = rnd.Next(maxItemsForType / 2, maxItemsForType);

        for (int i = 0; i < nr; i++)
        {
            Item item = Instantiate(ItemsManager.instance.SearchItemsList(type)).GetComponent<Item>();
            item.SetType(type);
            item.AddToStack(1);

            item.transform.SetParent(SaveLoadManager.instance.items.transform);
            item.transform.position = new Vector2(Random.Range(-25, 25), Random.Range(-25, 25));
        
        }
    }

    void SpawnResources(string type)
    {
        int nr = rnd.Next(maxItemsForType / 2, maxItemsForType);

        for (int i = 0; i < nr; i++)
        {
            Resource resource = Instantiate(ItemsManager.instance.SearchResourcesList(type)).GetComponent<Resource>();

            resource.transform.SetParent(SaveLoadManager.instance.resources.transform);
            resource.transform.position = new Vector2(Random.Range(-25, 25), Random.Range(-25, 25));
            resource.SetType(type);
        }
    }
}
