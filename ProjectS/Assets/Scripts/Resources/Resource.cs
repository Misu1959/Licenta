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

    [SerializeField] protected  int howToGather; // 0-gather | 1-chop | 2-mine
    [SerializeField] protected  string[] dropTypes;

    public bool isBeingGathered { get; private set;}

    protected float maxTimeToGather = 1;
    protected float timeToGather;

    [HideInInspector] public bool isGrown;
    [HideInInspector] public float timeToGrow;
    private float maxTimeToGrow = 20;

    private void Update()
    {
        GatherItemOfType();
        Regrow();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(CheckIfCanBeGathered())
            SetToGather();
    }

    public virtual void OnMouseEnter()
    {
        if(isGrown)
            PopUpManager.instance.ShowMousePopUp("LMB - Gather");
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
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.gather);
        else if (howToGather == 1)
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.chop);
        else if (howToGather == 2)
            PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.mine);
    }

    public void SetIsBeingGathered(bool _isBeingGathered)
    {
        isBeingGathered = _isBeingGathered;
        timeToGather = maxTimeToGather;
    }


    public virtual void GatherItemOfType()
    {

        // If player isn't harvesting it or if it's not grown
        if (!isBeingGathered || !isGrown)
            return;

        if(!PlayerActionManagement.instance.isPerformingAction)
        {
            SetIsBeingGathered(false);
            return;
        }

        timeToGather -= Time.deltaTime;
        if (timeToGather <= 0)
        {
            // Next lines adds the loot to the inventory
            GameObject item = Instantiate(ItemsManager.instance.SearchItemsList(dropTypes[0]));
            item.GetComponent<Item>().SetType(dropTypes[0]);
            item.GetComponent<Item>().AddToStack(1);
            InventoryManager.instance.AddItemToSlot(item);
            
            PlayerActionManagement.instance.CompleteAction(); // Complete the action
            SetIsBeingGathered(false);

            timeToGrow = maxTimeToGrow;
            isGrown = false;
        }
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

}
