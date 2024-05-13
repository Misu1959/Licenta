using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    public Transform border;
    public Transform ground;
    public Transform sun;

    public Transform items;
    public Transform resources;
    public Transform constructions;
    public Transform mobs;
    public Transform mobSpawners;

    void Awake()
    {
        instance = this;
        this.gameObject.SetActive(false);
    }


    public void SendMobsToSleep()
    {
        foreach (Transform mob in mobs)
        {
            if (mob.GetComponent<MobStats>().GetSleepPeriod() == TimeManager.DayState.night)
            {
                if (TimeManager.instance.dayState == TimeManager.DayState.dawn)
                {
                    if(mob.GetComponent<MobStats>().spawner)
                        mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob.GetComponent<MobStats>().spawner, MobBehaviour.Action.goInside);
                }
                else if (TimeManager.instance.dayState == TimeManager.DayState.night)
                    mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob, MobBehaviour.Action.sleep);

            }
            else if(mob.GetComponent<MobStats>().GetSleepPeriod() == TimeManager.DayState.day)
            {
                if (TimeManager.instance.dayState == TimeManager.DayState.day)
                    if(mob.GetComponent<MobStats>().spawner)
                        mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob.GetComponent<MobStats>().spawner, MobBehaviour.Action.goInside);
                    else
                        mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob, MobBehaviour.Action.sleep);
            }
        }
    }

    public void WakeUpMobs()
    {
        foreach (Transform mob in mobs)
        {
            if (mob.GetComponent<MobBehaviour>().action != MobBehaviour.Action.sleep) continue;

            if (mob.GetComponent<MobStats>().GetSleepPeriod() == TimeManager.DayState.night)
            {
                if (TimeManager.instance.dayState == TimeManager.DayState.day)
                {
                    mob.gameObject.SetActive(true);
                    mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob,MobBehaviour.Action.wakeUp);
                }
            }
            else if (mob.GetComponent<MobStats>().GetSleepPeriod() == TimeManager.DayState.day)
            {
                if (TimeManager.instance.dayState == TimeManager.DayState.dawn)
                {
                    mob.gameObject.SetActive(true);
                    mob.GetComponent<MobBehaviour>().SetNewTargetAndAction(mob, MobBehaviour.Action.wakeUp);
                }
            }

        }
    }


    public void SetResourcesToHarvest()
    {
        foreach (Transform res in resources)
            if (res.GetComponent<Resource>().GetHarvestPeriod() == TimeManager.DayState.allDay) 
                continue;
            else
            {
                if (res.GetChild(0).GetComponent<Animator>().GetInteger("Stage") >= (int)Resource.GrowthStages.full)
                {
                    if (res.GetComponent<Resource>().GetHarvestPeriod() != TimeManager.instance.dayState)
                        res.GetChild(0).GetComponent<Animator>().SetInteger("Stage", (int)Resource.GrowthStages.hide);
                    else
                        res.GetChild(0).GetComponent<Animator>().SetInteger("Stage", (int)Resource.GrowthStages.show);
                }
            }
    }

}
