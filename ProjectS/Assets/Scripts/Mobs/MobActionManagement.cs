using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobActionManagement : MonoBehaviour
{
    public enum Action
    {
        nothing,
        rest,
        goInside,
        goOutside,
        gather,
        chop,
        mine,
        fight,
        eat
    };

    [HideInInspector] public List<GameObject> itemsInRange = new List<GameObject>();
    
    public GameObject currentTarget { get; private set; }
    public Action currentAction { get; private set; }
    public bool isPerformingAction { get; private set; }


    private void Update() => CancelActionForTakingToLong();

    void SearchForItemInRange()
    {
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

            
        if (closestObjectToInteractWith?.GetComponent<Food>()) // If closest item exist and is an item go and pick it
            SetTargetAndAction(closestObjectToInteractWith, Action.eat);
        else if (closestObjectToInteractWith?.GetComponent<Resource>()) // If closest item exist and is a resource type go and gather/chop/mine it
            closestObjectToInteractWith.GetComponent<Resource>().SetToGather();

    }

    public void SetTargetAndAction(GameObject _target, Action _currentAction)
    {
        CancelAction(true);
        currentTarget = _target;
        currentAction = _currentAction;
    }

    public void PerformAction()
    {
        if (isPerformingAction) return;
        actionExpireTimer.RestartTimer();

        switch (currentAction)
        {
            case Action.rest:
                {
                    GetComponent<MobController>().isWaiting = true;
                    Invoke(nameof(CompleteAction), 10f);
                    break;
                }
            case Action.goInside:
                {
                    CompleteAction();
                    break;
                }
            case Action.goOutside:
                {
                    Invoke(nameof(CompleteAction), .5f);
                    break;
                }

            case Action.fight:
                {
                    Invoke(nameof(CompleteAction), .5f);
                    break;
                }
            case Action.eat:
                {
                    Invoke(nameof(CompleteAction), .5f);
                    break;
                }
        }

        isPerformingAction = true;
    }

    public void CompleteAction()
    {
        switch (currentAction)
        {
            case Action.rest:
                {
                    GetComponent<MobController>().isWaiting = false;
                    break;
                }
            case Action.goInside:
                {
                    transform.gameObject.SetActive(false);
                    break;
                }
            case Action.goOutside:
                {
                    break;
                }

            case Action.eat:
                {
                    break;
                }
        }

        isPerformingAction = false;
        GetComponent<MobController>().SetTargetPosition();
    }

    public void CancelAction(bool newAction = false)
    {
        switch (currentAction)
        {
            case Action.rest:
                {
                    GetComponent<MobController>().isWaiting = false;
                    break;
                }
        }

        isPerformingAction = false;

        if(!newAction)
            GetComponent<MobController>().SetTargetPosition();
    }

    Timer actionExpireTimer = new Timer(10);
    private void CancelActionForTakingToLong()
    {
        actionExpireTimer.StartTimer();
        actionExpireTimer.Tick();
    
        if(actionExpireTimer.IsElapsed())
            CancelAction();

    }    
}
