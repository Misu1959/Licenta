using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Equipment : Item
{
    public int equipmentNumber; // 0-hand | 1-body | 2-head
    public int actionNumber;
    
    [SerializeField] private float maxDurability;
    public float durability { get; private set;}

    public void SetDurability(float _durability)
    {
        if (_durability == -1)
            durability = maxDurability;
        else
            durability = _durability;

        DisplayDurability();
    }

    public void UseTool()
    {
        durability--;
        DisplayDurability();

        if (durability <= 0)
        {
            //EquipmentManager.instance.SetEquipment(InventoryManager.instance.FindItem(GetComponent<Item>().type).GetComponent<Equipment>());
            Destroy(this.gameObject);
        }
    }

    public void DisplayDurability()
    {
        if(GetComponent<EquipmentUI>())
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = durability.ToString();
        }
    }

}
