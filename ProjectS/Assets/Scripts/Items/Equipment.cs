using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Equipment : Item
{
    public int equipmentNumber; // 0-hand | 1-body | 2-head
    public int actionNumber;
    
    [SerializeField] private float maxDurability;
    public float durability { get; private set;}



    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        //  if (GetComponent<Equipment>())
        //      PlayerGatherManager.instance.SetTarget(this.gameObject, 33);

    }
    
    public override void OnMouseEnter()
    {
        string popUpText = "LMB - Pick\nRMB - Equip";
        PopUpManager.instance.ShowMousePopUp(popUpText);
    }

    public void SetDurability(float _durability)
    {
        if (_durability == -1)
            durability = maxDurability;
        else
            durability = _durability;

        GetComponent<EquipmentUI>()?.DisplayStack();
    }

    public void UseTool()
    {
        durability--;
        GetComponent<EquipmentUI>()?.DisplayStack();

        if (durability <= 0)
        {
            //EquipmentManager.instance.SetEquipment(InventoryManager.instance.FindItem(GetComponent<Item>().type).GetComponent<Equipment>());
            Destroy(this.gameObject);
        }
    }

}
