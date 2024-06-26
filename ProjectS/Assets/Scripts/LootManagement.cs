using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class LootManagement : MonoBehaviour
{
    [Serializable]
    struct Loot
    {
        public ObjectName name;
        public int amount;
    };

    [SerializeField] private List<Loot> loot;

    public void DropLoot()
    {
        for(int i=0;i<loot.Count;i++) 
        {
            for (int j = 0; j < loot[i].amount; j++)
            {
                Item drop = ItemsManager.instance.CreateItem(loot[i].name);

                // Set loot position
                drop.transform.position = new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), 0,
                                                      Random.Range(transform.position.z - 1, transform.position.z + 1));
                drop.transform.SetParent(WorldManager.instance.items); // Set loot parent object
            }
        }


    }

    public void CollectLoot()
    {
        for (int i = 0; i < loot.Count; i++)
        {
            Item originaItem = ItemsManager.instance.GetOriginalItem(loot[i].name);
            ItemData data = null;

            if (originaItem.GetComponent<ItemMaterial>())
                data = new ItemData(loot[i].name, loot[i].amount);
            else if (originaItem.GetComponent<Food>())
                data = new FoodData(loot[i].name, loot[i].amount);
            else if (originaItem.GetComponent<Equipment>())
                data = new EquipmentData(loot[i].name, loot[i].amount, originaItem.GetEquipmentData().maxDurability);

            ItemUI itemUI = ItemsManager.instance.CreateItemUI(data, null);
            InventoryManager.instance.AddItemToInventory(itemUI);
        }
    }

}
