using System.Collections;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

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
        destroy = 22,
        addFuel = 31,
        cook = 32,
        eat = 33
    };

    public static PlayerActionManagement instance;
    public Action currentAction { get; private set; }

    [HideInInspector] public GameObject target;
    
    [HideInInspector] public List<GameObject> itemsInRange = new List<GameObject>();

    public GameObject playerBody { get; private set; }
    public GameObject darknessCollider { get; private set; }
    public GameObject actionCollider { get; private set; }
    public GameObject searchCollider { get; private set; }


    void Start()
    {
        instance = this;

        playerBody       = this.gameObject;
        darknessCollider = transform.GetChild(0).gameObject;
        actionCollider   = transform.GetChild(1).gameObject;
        searchCollider   = transform.GetChild(2).gameObject;
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
            PerformAction(closestItem, Action.pick);
        else // If closest item exist and is a resource type go and gather/chop/mine it
            closestItem?.GetComponent<Resource>()?.SetToGather();
    }

    public void PerformAction(GameObject _target, Action _currentAction)
    {
        
        target = _target;
        if (_currentAction == Action.nothing)
            _currentAction = 0;

        currentAction = _currentAction;
        GetComponent<PlayerController>().SetTarget(target);
    }

    public void CompleteAction()
    {
        PerformAction(null, Action.nothing);
    }

    public void CancelAction()
    {
        PopUpManager.instance.ShowPopUpActionCanceled();
        PerformAction(null, Action.nothing);
    }

    private void CancelActionByMoving()
    {
        if (currentAction == Action.nothing) // If player is doing nothing there is nothing to cancel
            return;


        // If player is moving on X or Y axis from keyboard cancel the action
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") !=0)
            CancelAction();
        
    }


}
