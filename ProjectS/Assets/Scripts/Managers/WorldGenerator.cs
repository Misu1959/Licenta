using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class WorldGenerator : MonoBehaviour
{
    private static System.Random rnd = new System.Random();


    void Start()
    {
        if (PlayerPrefs.GetInt(SaveData.NEW_WORLD + PlayerPrefs.GetInt(SaveData.SELECTED_WORLD)) == 1)
        {
            Destroy(this);
            return;
        }

        PlayerPrefs.SetInt(SaveData.NEW_WORLD + PlayerPrefs.GetInt(SaveData.SELECTED_WORLD), 1);
        StartCoroutine(GenerateWorld());

    }
    private IEnumerator GenerateWorld()
    {
        yield return null;


        for (int i = 0; i < PlayerPrefs.GetInt(SaveData.NR_SPAWN_SETTINGS); i++)
        {
            ObjectName objectName   = (ObjectName)PlayerPrefs.GetInt(SaveData.SPAWN_SETTING_NAME + i);
            int objectSpawnValue    = PlayerPrefs.GetInt(SaveData.SPAWN_SETTING_VALUE + i);

            Spawn(objectName, objectSpawnValue);
        }
    }

    public void Spawn(ObjectName objectName, int objectSpawnValue)
    {
        if (ItemsManager.instance.GetOriginalItem(objectName) != null)
            SpawnItems(objectName, objectSpawnValue);
        else if (ItemsManager.instance.GetOriginalResource(objectName) != null)
            SpawnResources(objectName, objectSpawnValue);
        else if (ItemsManager.instance.GetOriginalMobSpawner(objectName) != null)
            SpawnMobSpawners(objectName, objectSpawnValue);
    }

    private void SpawnItems(ObjectName objectName, int objectSpawnValue)
    {
        int nr = rnd.Next((int)(objectSpawnValue * .75f), (int)(objectSpawnValue * 1.25f));
        int worldSize = PlayerPrefs.GetInt("WorldSize");

        for (int i = 0; i < nr; i++)
        {
            Item item = ItemsManager.instance.CreateItem(objectName);

            item.transform.SetParent(WorldManager.instance.items);
            item.transform.position = new Vector3(Random.Range(-1 * worldSize, worldSize), 0, Random.Range(-1 * worldSize, worldSize));

        }
    }

    private void SpawnResources(ObjectName objectName, int objectSpawnValue)
    {
        int nr = rnd.Next((int)(objectSpawnValue * .75f), (int)(objectSpawnValue * 1.25f));
        int worldSize = PlayerPrefs.GetInt("WorldSize");

        for (int i = 0; i < nr; i++)
        {
            Resource resource = Instantiate(ItemsManager.instance.GetOriginalResource(objectName));

            resource.transform.SetParent(WorldManager.instance.resources);
            resource.transform.position = new Vector3(Random.Range(-1 * worldSize, worldSize), 0, Random.Range(-1 * worldSize, worldSize));
        }
    }

    private void SpawnMobSpawners(ObjectName objectName, int objectSpawnValue)
    {
        int nr = rnd.Next((int)(objectSpawnValue * .75f), (int)(objectSpawnValue * 1.25f));
        int worldSize = PlayerPrefs.GetInt("WorldSize");

        for (int i = 0; i < nr; i++)
        {
            MobSpawner mobSpawner = Instantiate(ItemsManager.instance.GetOriginalMobSpawner(objectName));

            mobSpawner.transform.SetParent(WorldManager.instance.mobSpawners);
            mobSpawner.transform.position = new Vector3(Random.Range(-1 * worldSize, worldSize), 0, Random.Range(-1 * worldSize, worldSize));
        }
    }






    



}
