using System.Collections;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGatherManager : MonoBehaviour
{
    public static PlayerGatherManager instance;

    /*
        1 - Pick
        2 - Drop
        
        6 - Fight

        11 - Gather
        12 - Chop
        13 - Mine

        21 - Destroy
        22 - Build
        23 - Place


        31 - AddFuel
        32 - Cook
        33 - eat
        
     */
    [HideInInspector] public int actionType;
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
        CheckForCancelingAction();
        SearchForItemInRange();
    }

    public void SetTarget(GameObject _target, int _actionType, bool isMoving = false)
    {
        if ((_actionType != 0 || isMoving == true))
            if (actionType == 2)
                InventoryManager.instance.AddItemToSlot(target);
            else if (actionType >= 11 && actionType <= 13)
                target.GetComponent<Resource>().SetIsGathering(false);
            else if (actionType == 32)
                InventoryManager.instance.selectedItem.GetComponent<Food>().SetIsCooking(false);

        target = _target;
        if (_actionType == -1)
            _actionType = 0;
        actionType = _actionType;

        GetComponent<PlayerController>().SetTargetPos(target);
    }

    public void CancelAction(bool isMoving = false)
    {
        if (actionType == 0)
            return;

        if (actionType == 12 || actionType == 13 || actionType == 32 || isMoving)
        { 
            PopUpManager.instance.ShowPopUpActionCanceled();
            SetTarget(null, -1, isMoving);
        }
    }

    void CheckForCancelingAction()
    {
        if ((!target) && actionType == 0)
            return;

        bool actionCanceled = false;

        if (Input.GetKeyDown(KeyCode.W))
            actionCanceled = true;
        if (Input.GetKeyDown(KeyCode.A))
            actionCanceled = true;
        if (Input.GetKeyDown(KeyCode.S))
            actionCanceled = true;
        if (Input.GetKeyDown(KeyCode.D))
            actionCanceled = true;

        if (actionCanceled == true)
            CancelAction(true);

    }

    void SearchForItemInRange()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        InventoryManager.instance.SetBackToSlot();

        float closestDim = 1000;
        GameObject closestItem = null;

        foreach (GameObject item in itemsInRange)
        {
            if(item.GetComponent<Resource>())
                if(!item.GetComponent<Resource>().CheckIfCanBeGathered())
                    continue;

            float dist = Vector2.Distance(transform.position, item.transform.position);
            if (dist < closestDim)
            {
                closestDim = dist;
                closestItem = item;
            }
        }

        if (closestItem)
        {
            if(closestItem.GetComponent<Item>())
                SetTarget(closestItem, 1);

            closestItem.GetComponent<Resource>()?.SetToGather();
        }
    }

}
