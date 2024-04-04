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
    public AgroType GetAgroType() => agroType;

    public Behaviour behaviour { get; private set; }
    public Action action { get; private set; }

    Timer actionTimer;

    private void Start() => actionTimer = new Timer(10, .001f);

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

    public void CompleteAction()
    {
        if (action == Action.nothing || !GetComponent<MobController>().currentTarget) return;

        switch(action)
        {
            case Action.action_1:
                GetComponent<MobStats>().animator.SetTrigger("Action_1");
                break;
            case Action.action_2:
                GetComponent<MobStats>().animator.SetTrigger("Action_2");
                break;
            case Action.action_3:
                GetComponent<MobStats>().animator.SetTrigger("Action_3");
                break;

            case Action.sleep:
                GetComponent<MobStats>().animator.SetTrigger("Sleep");
                GetComponent<MobController>().currentTarget = null;
                return;
            case Action.wakeUp:
                GetComponent<MobStats>().animator.SetTrigger("WakeUp");
                GetComponent<MobController>().currentTarget = null;
                actionTimer.SetTime(.5f);
                return;
            
            case Action.goInside:
                gameObject.SetActive(false);
                break;
            case Action.attack:
                GetComponent<MobStats>().animator.SetTrigger("Attack");
                break;
        }

        action = Action.nothing;
        actionTimer.SetTime(3);
    }

    void CheckForNewAction()
    {

        actionTimer.StartTimer();
        actionTimer.Tick();

        if (!actionTimer.IsElapsed()) return;

        if (action == Action.goInside) return;
        if (action == Action.sleep) return;
        
        SetNewTargetAndAction();
    }



    void FallowPlayer() => behaviour = Behaviour.fallowPlayer;

    void RunAwayFromTarget() => SetNewTargetAndAction(PlayerStats.instance.transform, Action.runAway);

}
