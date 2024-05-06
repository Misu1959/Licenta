using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldSettingsData;
using Random = UnityEngine.Random;


public class ResourceSpawnerManager : MonoBehaviour
{
    private static System.Random rnd = new System.Random();


    private enum ObjectSpawnRarity
    {
        common      = 100,
        uncommon    = 75,
        rare        = 40
    }

    private class ObjectSpawnData
    {
        private ObjectName objectName;

        private ObjectSpawnRarity objectSpawnRarity;
        public int spawnNumber { get; private set; }


        public ObjectSpawnData(ObjectName _objectName, ObjectSpawnRarity _objectSpawnRarity)
        {
            objectName          = _objectName;
            objectSpawnRarity   = _objectSpawnRarity;
            CalculateSpawnNumber();
        }

        private void CalculateSpawnNumber()
        {
            if (ItemsManager.instance.GetOriginalItem(objectName) != null
            || ItemsManager.instance.GetOriginalResource(objectName) != null
            || ItemsManager.instance.GetOriginalMobSpawner(objectName) != null)
            {
                /*
                spawnNumber = (int)((int)LoadWorldSettings.instance.worldSize *
                             ((float)PlayerPrefs.GetInt(objectName + " spawn rate") / 100) *
                             ((float)((int)objectSpawnRarity) / 100));
                */
                spawnNumber = 100;
            }

        }

        public void Spawn()
        {
            if (ItemsManager.instance.GetOriginalItem(objectName) != null)
                SpawnItems();
            else if (ItemsManager.instance.GetOriginalResource(objectName) != null)
                SpawnResources();
            else if (ItemsManager.instance.GetOriginalMobSpawner(objectName) != null)
                SpawnMobSpawners();
        }

        private void SpawnItems()
        {
            int nr = rnd.Next((int)(spawnNumber * .75f), (int)(spawnNumber * 1.25f));
            int worldSize = (int)LoadWorldSettings.instance.worldSize;

            for (int i = 0; i < nr; i++)
            {
                Item item = ItemsManager.instance.CreateItem(objectName);

                item.transform.SetParent(WorldManager.instance.items);
                item.transform.position = new Vector3(Random.Range(-1 * worldSize, worldSize), 0, Random.Range(-1 * worldSize, worldSize));

            }
        }

        private void SpawnResources()
        {
            int nr = rnd.Next((int)(spawnNumber * .75f), (int)(spawnNumber * 1.25f));
            int worldSize = (int)LoadWorldSettings.instance.worldSize;

            for (int i = 0; i < nr; i++)
            {
                Resource resource = Instantiate(ItemsManager.instance.GetOriginalResource(objectName));

                resource.transform.SetParent(WorldManager.instance.resources);
                resource.transform.position = new Vector3(Random.Range(-1 * worldSize, worldSize), 0, Random.Range(-1 * worldSize, worldSize));
            }
        }

        private void SpawnMobSpawners()
        {
            int nr = rnd.Next((int)(spawnNumber * .75f), (int)(spawnNumber * 1.25f));
            int worldSize = (int)LoadWorldSettings.instance.worldSize;

            for (int i = 0; i < nr; i++)
            {
                MobSpawner mobSpawner = Instantiate(ItemsManager.instance.GetOriginalMobSpawner(objectName));

                mobSpawner.transform.SetParent(WorldManager.instance.mobSpawners);
                mobSpawner.transform.position = new Vector3(Random.Range(-1 * worldSize, worldSize), 0, Random.Range(-1 * worldSize, worldSize));
            }
        }
    };
    
    
    private List<ObjectSpawnData> objectsToSpawn = new List<ObjectSpawnData>();
    
    void Start()
    {
        if (PlayerPrefs.GetInt("prevWorld") > 1)
            return;

        StartCoroutine(GenerateWorld());
        
    }

    private IEnumerator GenerateWorld()
    {
        yield return null;

        objectsToSpawn.Add(new  ObjectSpawnData(ObjectName.berryBush, ObjectSpawnRarity.common));

        foreach(ObjectSpawnData objectToSpawn in objectsToSpawn)
            objectToSpawn.Spawn();
    }






}
