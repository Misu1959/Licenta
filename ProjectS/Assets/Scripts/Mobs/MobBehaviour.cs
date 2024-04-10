using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MobBehaviour : MonoBehaviour
{
    static System.Random rnd = new System.Random();

    public enum AgroType
    {
        passive,
        neutral,
        aggressive,

        teritorial_passive,
        teritorial_aggressive
    }
    public enum Behaviour
    {
        walk,
        run,
        fallowPlayer,
    };
    public enum Action
    {
        nothing,
        walk,

        sleep,
        wakeUp,

        goInside,
        runAway,

        attack,

        action_1,
        action_2,
        action_3
    };


    [SerializeField] private AgroType agroType;
    public Behaviour behaviour { get; private set; }
    public Action action { get; private set; }


    [Tooltip("Time between actions")]
    [SerializeField] private Timer actionTimer;
    
    [Tooltip("How long does agro last")]
    [SerializeField] private Timer agroTimer;

    public bool isPlayerInRange { private get; set; }


    private void Start() => actionTimer.SetTime(.001f);

    void Update() => CheckForNewAction();
    
    public void SetNewTargetAndAction(Transform newTarget = null, Action newAction = Action.nothing)
    {
        actionTimer.RestartTimer();

        if (!newTarget || newAction == Action.nothing)
            newAction = SelectAction();

        SetNewAction(newAction);
        SetNewTarget(newTarget);
    
    }
    void SetNewAction(Action newAction)
    {
        action = newAction;
        if (behaviour == Behaviour.fallowPlayer)
        {
            switch(newAction)
            { 
                case Action.sleep:
                    behaviour = Behaviour.walk;
                    break;
                case Action.wakeUp:
                    behaviour = Behaviour.walk;
                    break;
                case Action.goInside:
                    behaviour = Behaviour.run;
                    break;
            }
        }
        else
        {
            switch (newAction)
            {
                case Action.walk:
                    behaviour = Behaviour.walk;
                    break;

                case Action.action_1:
                    behaviour = Behaviour.walk;
                    break;
                case Action.action_2:
                    behaviour = Behaviour.walk;
                    break;
                case Action.action_3:
                    behaviour = Behaviour.walk;
                    break;

                case Action.sleep:
                    behaviour = Behaviour.walk;
                    break;
                case Action.wakeUp:
                    behaviour = Behaviour.walk;
                    break;


                case Action.goInside:
                    behaviour = Behaviour.run;
                    break;

                case Action.attack:
                    behaviour = Behaviour.run;
                    break;
                case Action.runAway:
                    behaviour = Behaviour.run;
                    break;


            }
        }
    }

    void SetNewTarget(Transform newTarget = null)
    {
        
        if (!newTarget)
        {
            Vector3 pos;
            if (behaviour == Behaviour.fallowPlayer)
                pos = PlayerStats.instance.transform.position;
            else if (GetComponent<MobStats>().GetSpawner())
                pos = GetComponent<MobStats>().GetSpawner().position;
            else
                pos = transform.position;

            GetComponent<MobController>().personalTarget.position = new Vector3(Random.Range(pos.x - 5, pos.x + 5),0,
                                                                                Random.Range(pos.z - 5, pos.z + 5));

            newTarget = GetComponent<MobController>().personalTarget;
        }
        else if (newTarget == this.transform)
        {
            GetComponent<MobController>().personalTarget.position = transform.position;
            newTarget = GetComponent<MobController>().personalTarget;
        }

        GetComponent<MobController>().currentTarget = newTarget;
    }

    Action SelectAction()
    {
        int nr = rnd.Next(1, 10);

        if (nr <= 7)
            return Action.walk;
        else if (nr == 8)
            return Action.action_1;
        else if (nr == 9)
            return Action.action_2;
        else if (nr == 10)
            return Action.action_3;

        return Action.walk;
    }

    public IEnumerator CompleteAction()
    {
        if (GetComponent<MobController>().currentTarget && action != Action.nothing)
        {
            switch (action)
            {
                case Action.walk:
                    {
                        action = Action.nothing;
                        actionTimer.SetTime(3);

                        break;
                    }
                case Action.action_1:
                    GetComponent<MobStats>().animator.SetTrigger("Action_1");
                    action = Action.nothing;
                    actionTimer.SetTime(3);

                    break;
                case Action.action_2:
                    GetComponent<MobStats>().animator.SetTrigger("Action_2");
                    action = Action.nothing;
                    actionTimer.SetTime(3);
                    
                    break;
                case Action.action_3:
                    GetComponent<MobStats>().animator.SetTrigger("Action_3");
                    action = Action.nothing;
                    actionTimer.SetTime(3);

                    break;
                case Action.sleep:
                    GetComponent<MobStats>().animator.SetTrigger("Sleep");
                    GetComponent<MobController>().currentTarget = null;

                    GetComponent<MobController>().SetCanMove(false);
                    break;
                case Action.wakeUp:

                    GetComponent<MobStats>().animator.SetTrigger("WakeUp");
                    GetComponent<MobController>().currentTarget = null;

                    action = Action.nothing;
                    actionTimer.SetTime(.5f);
                    yield return new WaitForSeconds(.5f);
                    GetComponent<MobController>().SetCanMove(true);

                    break;
                case Action.goInside:
                    action = Action.sleep;
                    gameObject.SetActive(false);
                
                    break;
                case Action.attack:
                    GetComponent<MobStats>().animator.SetTrigger("Attack");
                    action = Action.nothing;

                    GetComponent<MobController>().SetCanMove(false);
                    actionTimer.SetTime(1);

                    yield return new WaitForSeconds(1);
                    GetComponent<MobController>().SetCanMove(true);

                    break;
            }
        }
        yield return null;
    }

    void CheckForNewAction()
    {
        actionTimer.StartTimer();
        actionTimer.Tick();
        agroTimer.Tick();

        if (!actionTimer.IsElapsed()) return;

        if (action == Action.goInside) return;
        if (action == Action.sleep) return;

        if (!agroTimer.IsElapsed())
        {
            FightOrFlight();
            return;
        }
        
        if (CheckTeritory()) return;
        SetNewTargetAndAction();
    }


    public void FightOrFlight()
    {
        //Wake mob ub
        GetComponent<MobController>().SetCanMove(true);


        if (agroTimer.IsOn())
            agroTimer.RestartTimer();

        agroTimer.StartTimer();


        switch(agroType)
        {
            case AgroType.passive:
                {
                    SetNewTargetAndAction(PlayerStats.instance.transform, Action.runAway);
                    break;
                }
            case AgroType.teritorial_passive:
                {
                    SetNewTargetAndAction(PlayerStats.instance.transform, Action.runAway);
                    break;
                }


            case AgroType.neutral:
                {
                    SetNewTargetAndAction(PlayerStats.instance.transform, Action.attack);
                    break;
                }
            case AgroType.aggressive:
                {
                    SetNewTargetAndAction(PlayerStats.instance.transform, Action.attack);
                    break;
                }
            case AgroType.teritorial_aggressive:
                {
                    SetNewTargetAndAction(PlayerStats.instance.transform, Action.attack);
                    break;
                }
        }
        
    
    }

    private bool CheckTeritory()
    {
        if (agroType == AgroType.aggressive)
        {
            FightOrFlight();
            return true;
        }

        if (!isPlayerInRange) return false;
        switch (agroType)
        {
            case AgroType.teritorial_passive:
                {
                    SetNewTargetAndAction(PlayerStats.instance.transform, Action.runAway);
                    return true;
                }

            case AgroType.teritorial_aggressive:
                {
                    SetNewTargetAndAction(PlayerStats.instance.transform, Action.attack);
                    return true;
                }
        }

        return false;
    }

    void FallowPlayer() => behaviour = Behaviour.fallowPlayer;
}
