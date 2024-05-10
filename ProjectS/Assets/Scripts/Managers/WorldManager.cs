using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    [SerializeField] private GameObject world;

    public Transform items { get; private set; }
    public Transform resources { get; private set; }
    public Transform constructions { get; private set; }
    public Transform mobs { get; private set; }
    public Transform mobSpawners { get; private set; }

    void Awake()
    {
        instance = this;

        items           = world.transform.GetChild(2);
        resources       = world.transform.GetChild(3);
        constructions   = world.transform.GetChild(4);
        mobs            = world.transform.GetChild(5);
        mobSpawners     = world.transform.GetChild(6);

        this.gameObject.SetActive(false);
    }


    public void SendMobsToSleep(TimeManager.DayState currentDayState)
    {
        foreach (Transform mob in mobs)
        {
            if (mob.GetComponent<MobStats>().GetSleepPeriod() == TimeManager.DayState.night)
            {
                if (currentDayState == TimeManager.DayState.dawn)
                {
                    if(mob.GetComponent<MobStats>().spawner)
                        mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob.GetComponent<MobStats>().spawner, MobBehaviour.Action.goInside);
                }
                else if (currentDayState == TimeManager.DayState.night)
                    mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob, MobBehaviour.Action.sleep);

            }
            else if(mob.GetComponent<MobStats>().GetSleepPeriod() == TimeManager.DayState.day)
            {
                if (currentDayState == TimeManager.DayState.day)
                    if(mob.GetComponent<MobStats>().spawner)
                        mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob.GetComponent<MobStats>().spawner, MobBehaviour.Action.goInside);
                    else
                        mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob, MobBehaviour.Action.sleep);
            }
        }
    }

    public void WakeUpMobs(TimeManager.DayState currentDayState)
    {
        foreach (Transform mob in mobs)
        {
            if (mob.GetComponent<MobBehaviour>().action != MobBehaviour.Action.sleep) continue;

            if (mob.GetComponent<MobStats>().GetSleepPeriod() == TimeManager.DayState.night)
            {
                if (currentDayState == TimeManager.DayState.day)
                {
                    mob.gameObject.SetActive(true);
                    mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob,MobBehaviour.Action.wakeUp);
                }
            }
            else if (mob.GetComponent<MobStats>().GetSleepPeriod() == TimeManager.DayState.day)
            {
                if (currentDayState == TimeManager.DayState.dawn)
                {
                    mob.gameObject.SetActive(true);
                    mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob, MobBehaviour.Action.wakeUp);
                }
            }

        }
    }


    public void SetResourcesToHarvest(TimeManager.DayState currentDayState)
    {
        foreach (Transform res in resources)
            if (res.GetComponent<Resource>().GetHarvestPeriod() == TimeManager.DayState.allDay) 
                continue;
            else
            {
                if (res.GetChild(0).GetComponent<Animator>().GetInteger("Stage") >= (int)Resource.GrowthStages.full)
                {
                    if (res.GetComponent<Resource>().GetHarvestPeriod() != currentDayState)
                        res.GetChild(0).GetComponent<Animator>().SetInteger("Stage", (int)Resource.GrowthStages.hide);
                    else
                        res.GetChild(0).GetComponent<Animator>().SetInteger("Stage", (int)Resource.GrowthStages.show);
                }
            }

    }

}
