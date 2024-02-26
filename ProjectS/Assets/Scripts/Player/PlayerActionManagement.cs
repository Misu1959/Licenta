using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
        search = 20,
        craft = 21,
        place = 22,
        build = 23,
        destroy = 24,
        addFuel = 31,
        cook = 32,
        eat = 33
    };

    public static PlayerActionManagement instance;

    [HideInInspector] public List<GameObject> itemsInRange = new List<GameObject>();
    public GameObject currentTarget { get; private set; }
    public Action currentAction { get; private set; }

    public bool isPerformingAction { get; private set; }

    void Start() =>   instance = this;

    void Update()
    {
        SearchForItemInRange();
        CancelActionByMoving();
    }
    void SearchForItemInRange()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;// If Space is not pressed return otherwise search for items in range
        if (!InteractionManager.CanPlayerInteractWithUI()) return;
        if (PlayerController.instance.keyboardMovement != Vector3.zero) return;// If it's moving from keyboard don't take space action

        InventoryManager.instance.SetBackToSlot();

        float closestDistance = 1000;
        GameObject closestObjectToInteractWith = null;

        for (int i = 0; i < itemsInRange.Count; i++)
            if (itemsInRange[i] == null)
                itemsInRange.RemoveAt(i);

        foreach (GameObject objectToInteractWith in itemsInRange)
        {
            if (objectToInteractWith.GetComponent<Resource>())
                if (!objectToInteractWith.GetComponent<Resource>().CheckIfCanBeGathered()) // If it can't be gathered skip it
                    continue;

            // Check if it's the closest one
            float dist = Vector3.Distance(transform.position, objectToInteractWith.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestObjectToInteractWith = objectToInteractWith;
            }
        }

        if (!closestObjectToInteractWith)
            PopUpManager.instance.ShowPopUp(this.transform, "Nothing in range!");
        else if (closestObjectToInteractWith.GetComponent<Item>()) // If closest item exist and is an item go and pick it
            SetTargetAndAction(closestObjectToInteractWith, Action.pick);
        else if(closestObjectToInteractWith.GetComponent<Resource>()) // If closest item exist and is a resource type go and gather/chop/mine it
            closestObjectToInteractWith.GetComponent<Resource>().SetToGather();

    }

    public void SetTargetAndAction(GameObject _target, Action _currentAction)
    {
        if (currentTarget == _target && currentTarget != null) return;// If I take the same action don't do anything
        if (!PlayerController.instance.canMove) return; // If I can't move

        if (currentTarget && _target) // If I take a new action cancel the current action
            CancelAction(true);
        else if(_target)
            PopUpManager.instance.ShowPopUp(this.transform, "Action taken!");



        currentTarget = _target;
        currentAction = _currentAction;

        if (currentAction == Action.place)
            InteractionManager.SetInteractionStatus(false);
    }

    public void PerformAction()
    {
        if (isPerformingAction) return;

        switch (currentAction)
        {
            case Action.pick:
                {
                    PlayerController.instance.SetCanMove(false);
                    PlayerStats.instance.animator.SetTrigger("PickDrop");
                
                    Invoke(nameof(CompleteAction), .5f);
                    break;
                }
            case Action.drop:
                {
                    PlayerController.instance.SetCanMove(false);
                    PlayerStats.instance.animator.SetTrigger("PickDrop");

                    Invoke(nameof(CompleteAction), .5f);
                    break;
                }
            case Action.search:
                {
                    PlayerController.instance.SetCanMove(false);
                    PlayerStats.instance.animator.SetTrigger("PickDrop");
                    currentTarget.transform.GetChild(0).GetComponent<Animator>().SetInteger("OpenClose", 1);

                    if (!InventoryManager.instance.chestPanel.gameObject.activeInHierarchy)
                    {
                        InventoryManager.instance.DisplayChest(currentTarget.GetComponent<Storage>());
                        Invoke(nameof(CompleteAction), .5f);
                    }
                    break;
                }
            case Action.equip:
                {
                    PlayerController.instance.SetCanMove(false);
                    PlayerStats.instance.animator.SetTrigger("PickDrop");

                    Invoke(nameof(CompleteAction), .5f);
                    break;
                }
            case Action.addFuel:
                {
                    PlayerController.instance.SetCanMove(false);
                    PlayerStats.instance.animator.SetTrigger("PickDrop");

                    Invoke(nameof(CompleteAction), .5f);
                    break;
                }
            case Action.eat:
                {
                    PlayerController.instance.SetCanMove(false);
                    if (currentTarget.GetComponent<Item_Base>().GetFoodData().quickEat == true)
                    {
                        PlayerStats.instance.animator.SetTrigger("IsQuickEating");
                        Invoke(nameof(CompleteAction), .5f);
                    }
                    else
                    { 
                        PlayerStats.instance.animator.SetTrigger("IsEating");
                        Invoke(nameof(CompleteAction), 1);
                    }
                    break;
                }
        }
        
        isPerformingAction = true;
        PlayerStats.instance.animator.SetBool("IsPerformingAction", true);

    }

    public void CompleteAction()
    {
        switch (currentAction)
        {
            case Action.pick:
                {
                    PlayerController.instance.SetCanMove(true);
                    InventoryManager.instance.AddItemToInventory(currentTarget.GetComponent<Item_Base>());
                    break;
                }
            case Action.drop:
                {
                    PlayerController.instance.SetCanMove(true);
                    currentTarget.GetComponent<Item>().SetTransparent(false);
                    break;
                }
            case Action.equip:
                {
                    PlayerController.instance.SetCanMove(true);
                    EquipmentManager.instance.SetEquipment(currentTarget.GetComponent<Item_Base>(), true, false);

                    break;
                }
            case Action.search:
                {
                    PlayerController.instance.SetCanMove(true);
                    PlayerStats.instance.animator.SetBool("IsPerformingAction", false);

                    return; ;
                }
            case Action.build:
                {
                    InteractionManager.SetInteractionStatus(true);
                    break;
                }
            case Action.addFuel:
                {
                    PlayerController.instance.SetCanMove(true);
                    currentTarget.GetComponent<Fireplace>().AddFuel();
                    break;
                }
            case Action.eat:
                {
                    PlayerController.instance.SetCanMove(true);
                    PlayerStats.instance.Eat(currentTarget.GetComponent<Item_Base>());
                    break;
                }
        }
        
        isPerformingAction = false;
        PlayerStats.instance.animator.SetBool("IsPerformingAction", false);
        
        SetTargetAndAction(null, Action.nothing);
        PopUpManager.instance.ShowPopUp(this.transform, "Action completed!");

    }

    public void CancelAction(bool newAction = false)
    {
        switch (currentAction)
        {
            case Action.drop:
                {
                    InventoryManager.instance.AddItemToInventory(currentTarget.GetComponent<Item_Base>());
                    Destroy(currentTarget);
                    break;
                }
            case Action.search:
                {
                    currentTarget.transform.GetChild(0).GetComponent<Animator>().SetInteger("OpenClose", -1);
                    InventoryManager.instance.DisplayChest();
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
        PlayerStats.instance.animator.SetBool("IsPerformingAction", false);

        if (!newAction)
        {
            SetTargetAndAction(null, Action.nothing);
            if (currentAction != Action.search)
                PopUpManager.instance.ShowPopUp(this.transform, "Action canceled!");
        }
        else
        {
            if (currentAction == Action.search)
                PopUpManager.instance.ShowPopUp(this.transform, "Action taken!");
            else
                PopUpManager.instance.ShowPopUp(this.transform, "New action taken!");
        }
    }

    private void CancelActionByMoving()
    {
        // If player has no target there is nothing to cancel
        if (!currentTarget) return;

        // If player is moving on X or Y axis from keyboard cancel the action
        if (PlayerController.instance.keyboardMovement == Vector3.zero) return;

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
     
        if (constructionToPlace.transform.parent?.gameObject == WorldManager.instance.constructions)
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
