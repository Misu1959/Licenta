using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using static UnityEditor.Progress;

public class Resource : MonoBehaviour, IPointerDownHandler
{
    public string type { get; private set; }

    [SerializeField] private float hp;
    [SerializeField] private int howToGather; // 0-gather | 1-chop | 2-mine
    [SerializeField] private string[] dropTypes;

    public bool isGathering { get; private set;}

    private float maxTimeToGather = 1;
    private float timeToGather;

    public bool isGrown { get; private set; }
    
    private float maxTimeToGrow = 20;
    public float timeToGrow { get; private set; }

    private void Update()
    {
        GatherItemOfType();
        Harvest();

        Regrow();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(CheckIfCanBeGathered())
            SetToGather();
    }

    private void OnMouseEnter()
    {
        string popUpText = "";
        if (howToGather == 0)
            popUpText = "LMB - Gather";
        else if(howToGather == 1 && CheckIfCanBeGathered())
            popUpText = "LMB - Chop";
        else if (howToGather == 2 && CheckIfCanBeGathered())
            popUpText = "LMB - Mine";

        PopUpManager.instance.ShowMousePopUp(popUpText);
    }

    private void OnMouseExit()
    {
        PopUpManager.instance.ShowMousePopUp();
    }


    public void SetType(string  _type)
    {
        type = _type;
    }

    public void SetToGather()
    {
        if (howToGather == 0)
            PlayerGatherManager.instance.SetTarget(this.gameObject, 11);
        else if (howToGather == 1)
            PlayerGatherManager.instance.SetTarget(this.gameObject, 12);
        else if (howToGather == 2)
            PlayerGatherManager.instance.SetTarget(this.gameObject, 13);
    }

    void SetAnim()
    {
        if (!GetComponent<Animator>())
            return;
        
        if (timeToGrow <= .01f * maxTimeToGrow)
            GetComponent<Animator>().SetInteger("GrowthStage", 4);
        else if (timeToGrow <= .34f * maxTimeToGrow)
            GetComponent<Animator>().SetInteger("GrowthStage", 3);
        else if (timeToGrow <= .67f * maxTimeToGrow)
            GetComponent<Animator>().SetInteger("GrowthStage", 2);
        else if (timeToGrow <= maxTimeToGrow)
            GetComponent<Animator>().SetInteger("GrowthStage", -1);

    }

    void Regrow() 
    {
        if (isGrown)
            return;

        SetAnim();

        timeToGrow -= Time.deltaTime;

        if (timeToGrow <= 0)
            isGrown = true;
    }

    public void Harvest()
    {

        if (!isGathering || !isGrown || howToGather == 0)
            return;

        if (timeToGather <= 0)
        {
            EquipmentManager.instance.GetHandItem().UseTool();
            hp--;
            if (hp <= 0)
                DestroyResource();

            if (!Input.GetMouseButton(0) && !Input.GetKey(KeyCode.Space))
            {
                PlayerGatherManager.instance.SetTarget(null, 0);
                SetIsGathering(false);
            }
            else
            {

                if (EquipmentManager.instance.GetHandItem()?.durability > 0)
                    SetIsGathering(true);
                else
                {
                    PlayerGatherManager.instance.SetTarget(null, 0);
                    SetIsGathering(false);
                }
            }
        }
        else
            timeToGather -= Time.deltaTime;

    }

    void DestroyResource()
    {
        for (int i = 0; i < dropTypes.Length; i++)
            DropItemOfType(dropTypes[i]);

        PlayerGatherManager.instance.SetTarget(null, 0);
        Destroy(this.gameObject);
    }

    void DropItemOfType(string typeOfItem)
    {
        Item drop = Instantiate(ItemsManager.instance.SearchItemsList(typeOfItem)).GetComponent<Item>();
        drop.SetType(typeOfItem);
        drop.AddToStack(1);

        drop.transform.position = new Vector2(Random.Range(transform.position.x - 1, transform.position.x + 1),
                                              Random.Range(transform.position.y - 1, transform.position.y + 1));
        drop.transform.SetParent(SaveLoadManager.instance.items.transform);

    }

    void GatherItemOfType()
    {

        if (!isGathering || !isGrown || howToGather != 0)
            return;

        if (timeToGather <= 0)
        {
            GameObject item = Instantiate(ItemsManager.instance.SearchItemsList(dropTypes[0]));
            
            item.GetComponent<Item>().SetType(dropTypes[0]);
            item.GetComponent<Item>().AddToStack(1);

            PlayerGatherManager.instance.SetTarget(null, 0);
            InventoryManager.instance.AddItemToSlot(item);

            SetIsGathering(false);
            isGrown = false;
        }
        else
            timeToGather -= Time.deltaTime;
    }

    public void SetIsGathering(bool _isGathering)
    {
        isGathering = _isGathering;
        
        timeToGrow = maxTimeToGrow;
        timeToGather = maxTimeToGather;
    }

    public void SetIsGrown(bool _isGrown)
    {
        isGrown= _isGrown;
    }

    public void SetTimeToGrow(float _timeToGrow)
    {
        timeToGrow = _timeToGrow;
    }

    public bool CheckIfCanBeGathered()
    {

        if (howToGather == 0)
        {
            if (isGrown)
                return true;
            else
                return false;
        }
        else
            return EquipmentManager.instance.CheckWhatItemIsEquipedInHand(howToGather);
    }


}
