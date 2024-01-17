using System.Collections;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerActionManagement : MonoBehaviour
{
    public enum Action
    {
        nothing = 0,
        pick = 1,
        drop = 2,
        equip = 3,
        fight = 6,
        gather = 11,
        chop = 12,
        mine = 13,
        craft = 21,
        place = 22,
        build = 23,
        destroy = 24,
        addFuel = 31,
        cook = 32,
        eat = 33
    };
    public Action currentAction { get; private set; }


    public static PlayerActionManagement instance;
    
    [HideInInspector] public List<GameObject> itemsInRange = new List<GameObject>();
    [HideInInspector] public GameObject currentTarget;

    public bool isPerformingAction { get; private set; }

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        SearchForItemInRange();

        CancelActionByMoving();
    }
    void SearchForItemInRange()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) // If Space is not pressed return otherwise search for items in range
            return;

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) // If it's moving from keyboard don't take space action
            return;

        if (!InteractionManager.canInteract)
            return;

        InventoryManager.instance.SetBackToSlot();

        float closestDim = 1000;
        GameObject closestItem = null;

        foreach (GameObject item in itemsInRange)
        {
            if (item.GetComponent<Resource>())
                if (!item.GetComponent<Resource>().CheckIfCanBeGathered()) // If it can't be gathered skip it
                    continue;

            // Check if it's the closest one
            float dist = Vector2.Distance(transform.position, item.transform.position);
            if (dist < closestDim)
            {
                closestDim = dist;
                closestItem = item;
            }
        }

        if (!closestItem)
            PopUpManager.instance.ShowPopUpAction("Nothing in range!");
        else if (closestItem.GetComponent<Item>()) // If closest item exist and is an item go and pick it
            SetTargetAndAction(closestItem, Action.pick);
        else // If closest item exist and is a resource type go and gather/chop/mine it
            closestItem.GetComponent<Resource>()?.SetToGather();

    }

    public void SetTargetAndAction(GameObject _target, Action _currentAction)
    {
        if (currentTarget == _target && currentTarget != null) // If I take the same action don't do anything
            return;

        if (currentTarget && _target) // If I take a new action cancel the current action
            CancelAction(true);
        else if(_target)
            PopUpManager.instance.ShowPopUpAction("Action taken!");

        currentTarget = _target;
        currentAction = _currentAction;

        if (currentAction == Action.place)
            InteractionManager.SetInteractionStatus(false);
    }

    public void PerformAction()
    {
        if (isPerformingAction)
            return;

        isPerformingAction = true;
        GetComponent<Animator>().SetBool("IsPerformingAction", true);

        switch (currentAction)
        {
            case Action.pick:
            {  
                GetComponent<Animator>().SetBool("PickDrop", true);

                Invoke("CompleteAction", .5f);
                break;
            }
            case Action.drop:
            {
                GetComponent<Animator>().SetBool("PickDrop", true);
                
                Invoke("CompleteAction", .5f);
                break;
            }
            case Action.equip:
            {
                GetComponent<Animator>().SetBool("PickDrop", true);

                Invoke("CompleteAction", .5f);
                break;
            }
            case Action.addFuel:
            {
                GetComponent<Animator>().SetBool("PickDrop", true);

                Invoke("CompleteAction", .5f);
                break;
            }
            case Action.eat:
            {
                int quickEat = currentTarget.GetComponent<Food>().quickEat == true ? 1 : 2;
                GetComponent<Animator>().SetInteger("IsEating", quickEat);

                Invoke("CompleteAction", quickEat / 2);
                break;
            }
        }
    }

    public void CompleteAction()
    {
        switch (currentAction)
        {
            case Action.pick:
                {
                    InventoryManager.instance.AddItemToSlot(currentTarget.GetComponent<Item>());
                    itemsInRange.Remove(currentTarget);
                    break;
                }
            case Action.drop:
                {
                    currentTarget.GetComponent<Item>().SetTransparent(false);

                    break;
                }
            case Action.equip:
                {
                    EquipmentManager.instance.SetEquipment(currentTarget.GetComponent<Item>().CreateItemUI().GetComponent<Equipment>());
                    itemsInRange.Remove(currentTarget);

                    break;
                }
            case Action.build:
                {
                    InteractionManager.SetInteractionStatus(true);
                    break;
                }
            case Action.addFuel:
                {
                    currentTarget.GetComponent<Fireplace>().AddFuel();
                    break;
                }
            case Action.eat:
                {
                    currentTarget.GetComponent<Food>().Consume();
                    break;
                }
        }

        SetTargetAndAction(null, Action.nothing);
        isPerformingAction = false;

        GetComponent<Animator>().SetBool("IsPerformingAction", false);
        GetComponent<Animator>().SetBool("PickDrop", false);
        GetComponent<Animator>().SetInteger("IsEating", 0);


        PopUpManager.instance.ShowPopUpAction("Action completed!");

    }

    public void CancelAction(bool newAction = false)
    {
        switch (currentAction)
        {
            case Action.drop:
                {
                    InventoryManager.instance.AddItemToSlot(currentTarget.GetComponent<Item>());
                    break;
                }
            case Action.place:
                {
                    InteractionManager.SetInteractionStatus(true);
                    break;
                }
            case Action.build:
                {
                    InteractionManager.SetInteractionStatus(true);
                    Destroy(currentTarget);
                    break;
                }
        }

        isPerformingAction = false;
        GetComponent<Animator>().SetBool("IsPerformingAction", false);

        if (newAction)
            PopUpManager.instance.ShowPopUpAction("New action taken!");
        else
        {
            SetTargetAndAction(null, Action.nothing);
            PopUpManager.instance.ShowPopUpAction("Action canceled!");
        }
    }

    private void CancelActionByMoving()
    {
        if (!currentTarget) // If player has no target there is nothing to cancel
            return;


        // If player is moving on X or Y axis from keyboard cancel the action
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            if(PlayerController.instance.CheckIfCanMove())
                CancelAction();
    }

    public bool IsGathering(GameObject resToGather)
    {
        if (currentTarget == resToGather && currentAction >= Action.gather && currentAction <= Action.mine && isPerformingAction)
            return true;
        else
            return false;
    }

    public bool IsPlacing(GameObject constructionToPlace)
    {
        if (currentAction != Action.place)
            return false;
     
        if (constructionToPlace.transform.parent?.gameObject == SaveLoadManager.instance.constructions)
            return false;

        return true; 
    }

    public bool IsBuilding(GameObject constructionToBuild)
    {
        if (currentTarget == constructionToBuild && currentAction == Action.build && isPerformingAction)
            return true;
        else
            return false;
    }

    public bool IsCooking(GameObject fireplace)
    {
        if (currentTarget == fireplace && currentAction == Action.cook && isPerformingAction)
            return true;
        else
            return false;
    }


}
