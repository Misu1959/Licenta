using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerBehaviour : MonoBehaviour
{
    public enum Action
    {
        nothing = 0,
        pick = 1,
        drop = 2,
        equip = 3,
        attack = 6,
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


    public static PlayerBehaviour instance;


    [HideInInspector] public List<Transform> itemsInRange = new List<Transform>();
    
    public Transform currentTarget { get; private set; }
    public Action currentAction { get; private set; }


    public bool isPerformingAction { get; private set; }



    void Start() =>   instance = this;

    void Update()
    {
        SearchForItemInRange();
        SearchForMobInRange();

        CancelActionByMoving();
    }
    void SearchForItemInRange()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;// If Space is not pressed return otherwise search for items in range
        if (!InteractionManager.CanPlayerInteractWithUI()) return;
        if (PlayerController.instance.keyboardMovement != Vector3.zero) return;// If it's moving from keyboard don't take space action

        InventoryManager.instance.SetBackToSlot();

        float closestDistance = 1000;
        Transform closestObjectToInteractWith = null;

        for (int i = 0; i < itemsInRange.Count; i++)
            if (itemsInRange[i] == null)
                itemsInRange.RemoveAt(i);

        foreach (Transform objectToInteractWith in itemsInRange)
        {
            if (objectToInteractWith.GetComponent<MobStats>()) continue;

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

    void SearchForMobInRange()
    {
        if (!Input.GetKeyDown(KeyCode.F)) return;// If F is not pressed return otherwise search for items in range
        if (!InteractionManager.CanPlayerInteractWithUI()) return;
        if (PlayerController.instance.keyboardMovement != Vector3.zero) return;
        if (!EquipmentManager.instance.GetHandItem()) return;

        InventoryManager.instance.SetBackToSlot();

        float closestDistance = 1000;
        Transform closestObjectToInteractWith = null;

        for (int i = 0; i < itemsInRange.Count; i++)
            if (itemsInRange[i] == null)
                itemsInRange.RemoveAt(i);

        foreach (Transform objectToInteractWith in itemsInRange)
        {
            if (!objectToInteractWith.GetComponent<MobStats>()) continue;

            // Check if it's the closest one
            float dist = Vector3.Distance(transform.position, objectToInteractWith.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestObjectToInteractWith = objectToInteractWith;
            }
        }

        if (!closestObjectToInteractWith)
            PopUpManager.instance.ShowPopUp(this.transform, "Nothing to attack in range!");
        else
            SetTargetAndAction(closestObjectToInteractWith, Action.attack);
    }

    public void SetTargetAndAction(Transform _target, Action _currentAction)
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

        PlayerStats.instance.animator.SetTrigger("PerformAction");
        isPerformingAction = true;

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
            case Action.place:
                {
                    InteractionManager.SetInteractionStatus(false);
                    break;
                }
            case Action.build:
                {
                    InteractionManager.SetInteractionStatus(false);
                    PlayerStats.instance.animator.SetBool("Gather", true);

                    break;
                }


            case Action.gather:
                {
                    PlayerStats.instance.animator.SetBool("Gather", true);

                    break;
                }
            case Action.chop:
                {
                    PlayerController.instance.SetCanMove(false);
                    PlayerStats.instance.animator.SetBool("Chop", true);
                    break;
                }

            case Action.mine:
                {
                    PlayerController.instance.SetCanMove(false);
                    PlayerStats.instance.animator.SetBool("Mine", true);
                    break;
                }

            case Action.eat:
                {
                    PlayerController.instance.SetCanMove(false);
                    if (currentTarget.GetComponent<Item_Base>().GetFoodData().quickEat == true)
                    {
                        PlayerStats.instance.animator.SetTrigger("QuickEat");
                        Invoke(nameof(CompleteAction), .5f);
                    }
                    else
                    { 
                        PlayerStats.instance.animator.SetTrigger("Eat");
                        Invoke(nameof(CompleteAction), 1);
                    }
                    break;
                }

            case Action.attack:
                {
                    PlayerController.instance.SetCanMove(false);
                    PlayerStats.instance.animator.SetTrigger("Attack");

                    Invoke(nameof(CompleteAction), .5f);
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
                    InventoryManager.instance.AddItemToInventory(currentTarget.GetComponent<Item_Base>());
                    break;
                }
            case Action.drop:
                {
                    currentTarget.GetComponent<Item>().SetTransparent(false);
                    break;
                }
            case Action.equip:
                {
                    EquipmentManager.instance.SetEquipment(currentTarget.GetComponent<Item_Base>(), true, false);
                    break;
                }
            case Action.search:
                {
                    PlayerController.instance.SetCanMove(true);
                    PlayerStats.instance.animator.SetBool("Gather", false);
                    return;
                }
            case Action.addFuel:
                {
                    currentTarget.GetComponent<Fireplace>().AddFuel();
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
                    PlayerStats.instance.animator.SetBool("Gather", false);

                    break;
                }


            case Action.gather:
                {
                    PlayerStats.instance.animator.SetBool("Gather", false);

                    break;
                }
            case Action.chop:
                {
                    PlayerStats.instance.animator.SetBool("Chop", false);
                    break;
                }

            case Action.mine:
                {
                    PlayerStats.instance.animator.SetBool("Mine", false);
                    break;
                }

            case Action.eat:
                {
                    PlayerStats.instance.Eat(currentTarget.GetComponent<Item_Base>());
                    break;
                }
        }

        PlayerController.instance.SetCanMove(true);

        isPerformingAction = false;
        
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

                    PlayerStats.instance.animator.SetBool("Gather", false);
                    Destroy(currentTarget.gameObject);
                    break;
                }

            case Action.gather:
                {
                    PlayerStats.instance.animator.SetBool("Gather", false);

                    break;
                }
            case Action.chop:
                {
                    PlayerStats.instance.animator.SetBool("Chop", false);
                    break;
                }

            case Action.mine:
                {
                    PlayerStats.instance.animator.SetBool("Mine", false);
                    break;
                }
        }

        isPerformingAction = false;

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

    public bool IsGathering(Transform resToGather)
    {
        if (currentTarget == resToGather && currentAction >= Action.gather && currentAction <= Action.mine && isPerformingAction)
            return true;
        else
            return false;
    }

    public bool IsPlacing(Transform constructionToPlace)
    {
        if (currentAction != Action.place)
            return false;
     
        if (constructionToPlace.transform.parent == WorldManager.instance.constructions)
            return false;

        return true; 
    }

    public bool IsBuilding(Transform constructionToBuild)
    {
        if (currentTarget == constructionToBuild && currentAction == Action.build && isPerformingAction)
            return true;
        else
            return false;
    }

    public bool IsCooking(Transform fireplace)
    {
        if (currentTarget == fireplace && currentAction == Action.cook && isPerformingAction)
            return true;
        else
            return false;
    }

}
