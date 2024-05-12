using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] 
    private StorageData storageData;

    public StorageData GetStorageData() => storageData; 

    public void SetStorageData(StorageData newStorageData) => storageData = new StorageData(newStorageData); 

    public void AddData(ItemData dataToAdd, int posToAddAt) => storageData.items[posToAddAt] = dataToAdd; 

    public void RemoveData(int posToRemoveAt)  => storageData.items[posToRemoveAt] = null;


}
