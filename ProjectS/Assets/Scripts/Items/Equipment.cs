using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Equipment : Item
{
    public enum Type
    {
        hand = 0,
        body = 1,
        head = 2
    };
    public Type equipmentType;
    public int actionNumber;


    [SerializeField] private float maxDurability;
    public float durability { get; private set;}



    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        
        //if (IsOnTheGround())
        //  if (GetComponent<Equipment>())
        //      PlayerActionManagement.instance.SetTarget(this.gameObject, 33);

    }

    public override void OnMouseOver()
    {
        if (!InteractionManager.canInteract || InventoryManager.instance.selectedItem)
            return;

        if (IsOnTheGround())
        {
            string popUpText = "LMB - Pick\nRMB - Equip";
            PopUpManager.instance.ShowMousePopUp(popUpText);
        }
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
            Destroy(this.gameObject);
            EquipmentManager.instance.SetEquipment(InventoryManager.instance.FindItemOfType(GetComponent<Item>())?.GetComponent<Equipment>());
        }
    }

}
