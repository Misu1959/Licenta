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

        if (closestItem?.GetComponent<Item>()) // If closest item exist and is an item go and pick it
            SetTargetAndAction(closestItem, Action.pick);
        else // If closest item exist and is a resource type go and gather/chop/mine it
            closestItem?.GetComponent<Resource>()?.SetToGather();
    }

    public void SetTargetAndAction(GameObject _target, Action _currentAction)
    {
        if(currentTarget && _target)
            CancelAction();
    
        currentTarget = _target;
        currentAction = _currentAction;
    }

    public void PerformAction()
    {
        isPerformingAction = true;

        switch(currentAction)
        {
            case Action.pick:
            {
                InventoryManager.instance.AddItemToSlot(currentTarget);
                itemsInRange.Remove(currentTarget);
                CompleteAction();
                break;
            }
            case Action.drop:
            {
                currentTarget.GetComponent<Item>().SetTransparent(false);
                CompleteAction();
                break;
            }
            case Action.gather: 
            {
                break;
            }
            case Action.chop:
            {
                break;
            }
            case Action.mine:
            {
                break;
            }
            case Action.addFuel:
            {
                currentTarget.GetComponent<Fire>().AddFuel(InventoryManager.instance.selectedItem);
                CompleteAction();
                break;
            }
            case Action.cook:
            {
                break;
            }
            case Action.eat:
            {
                currentTarget.GetComponent<Food>().Consume();
                CompleteAction();
                break;
            }
            default:
                break;
        }
    }

    public void CompleteAction()
    {
        SetTargetAndAction(null, Action.nothing);
        isPerformingAction = false;
    }

    private void CancelAction()
    {
        switch (currentAction)
        {
            case Action.pick:
                {
                    break;
                }
            case Action.drop:
                {
                    InventoryManager.instance.AddItemToSlot(currentTarget);
                    break;
                }
            case Action.gather:
                {
                    break;
                }
            case Action.chop:
                {
                    break;
                }
            case Action.mine:
                {
                    break;
                }
            case Action.addFuel:
                {
                    break;
                }
            case Action.cook:
                {
                    break;
                }
            case Action.eat:
                {
                    break;
                }
            case Action.build:
                {
                    CraftingManager.instance.ActivateCraftingButtons(true);
                    Destroy(currentTarget);
                    break;
                }

            default:
                break;
        }

        isPerformingAction = false;

        PopUpManager.instance.ShowPopUpActionCanceled();
        SetTargetAndAction(null, Action.nothing);
    }

    private void CancelActionByMoving()
    {
        if (!currentTarget) // If player has no target there is nothing to cancel
            return;


        // If player is moving on X or Y axis from keyboard cancel the action
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") !=0)
            CancelAction();
        
    }

}
