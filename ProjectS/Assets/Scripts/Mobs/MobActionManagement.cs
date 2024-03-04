using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public enum ActionMode
    {
        idle,
        rest,
        performingAction,
        fallowingPlayer
    }

    [HideInInspector] public List<GameObject> itemsInRange = new List<GameObject>();

    
    private Transform personalTarget;
    public GameObject currentTarget { get; private set; }
    
    public Action currentAction { get; private set; }

    public ActionMode currentActionMode { get; private set; }


    private void Start()
    {
        SetPersonalTarget();
        SetTargetPosition();
    }

    private void Update() => CancelActionForTakingToLong();

    private void SearchForItemInRange()
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

    private void SetPersonalTarget()
    {
        personalTarget = transform.GetChild(1);
        personalTarget.SetParent(null);
    }

    private void SetTargetPosition()
    {
        Vector3 newPos;
        Vector3 targetPos;
        if(currentActionMode == ActionMode.fallowingPlayer)
            targetPos = PlayerStats.instance.transform.position;
        else if(GetComponent<MobStats>().GetSpawner())
            targetPos = GetComponent<MobStats>().GetSpawner().position;
        else
            targetPos = transform.position;
        
        newPos = new Vector3(Random.Range(targetPos.x - 10, targetPos.x + 10), 0,
                             Random.Range(targetPos.z - 10, targetPos.z + 10));

        personalTarget.transform.position = newPos;
        SetTargetAndAction(personalTarget.gameObject, Action.rest);
    }

    public void SetTargetAndAction(GameObject _target, Action _currentAction)
    {
        CancelAction(true);
        currentTarget = _target;
        currentAction = _currentAction;

        actionExpireTimer.RestartTimer();
    }

    public void PerformAction()
    {
        if (currentActionMode != ActionMode.idle) return;

        switch (currentAction)
        {
            case Action.rest:
                {
                    currentActionMode = ActionMode.rest;
                    Invoke(nameof(CompleteAction), 5f);
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

    }

    public void CompleteAction()
    {
        switch (currentAction)
        {
            case Action.rest:
                {
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

        currentActionMode = ActionMode.idle;
        SetTargetPosition();
    }

    public void CancelAction(bool newAction = false)
    {
        switch (currentAction)
        {
            case Action.rest:
                {
                    break;
                }
        }

        currentActionMode = ActionMode.idle;

        if (!newAction)
            SetTargetPosition();
    }

    Timer actionExpireTimer = new Timer(30);
    private void CancelActionForTakingToLong()
    {
        actionExpireTimer.StartTimer();
        actionExpireTimer.Tick();
    
        if(actionExpireTimer.IsElapsed())
            CancelAction();

    }    

    public bool IsRunning() => (personalTarget.gameObject == currentTarget) ? false : true;

}
