using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public enum DayState
    {
        day,
        dawn,
        night,

        allDay
    }


    [SerializeField] private GameObject sun;

    private const int nrOfHoursInDay = 16;
    private const float hourLength = 100 / (float)nrOfHoursInDay / 100; // How much does one hour represent of a day length in percents 


    private int dayDuration;
    private int dayLength;
    private int dawnLength;
    private int nightLength;

    private int currentHour;
    private int currentDay;

    public DayState dayState { get; private set; }
    private Timer timerHour;
    
    private Timer timerPlayerInDarkness;
    private int nrOfPopUps;

    private IEnumerator Start()
    {
        instance = this;
        SetSettings();

        timerHour = new Timer(hourLength * dayDuration);
        timerPlayerInDarkness = new Timer(6);


        yield return null;

        if (currentDay == 0)
        {
            currentDay = 1;
            PlayerPrefs.SetInt("currentDay", currentDay);
        }
        else
            currentDay = PlayerPrefs.GetInt("currentDay");


        UIManager.instance.SetClock(dayLength * hourLength, dawnLength * hourLength);
        UIManager.instance.ShowDayCount(currentDay);

        dayState = DayState.day;


        WorldManager.instance.SendMobsToSleep(DayState.day);
        WorldManager.instance.SendMobsToSleep(DayState.day);
        WorldManager.instance.SetResourcesToHarvest(DayState.day);
    }

    void Update()
    {
        PassTime();
        PassHour();


        DarknessHitPlayer();
    }

    private void SetSettings()
    {
        dayDuration = SaveLoadManager.Get_Day_Duration();
        dayLength   = SaveLoadManager.Get_Day_Length();
        nightLength = SaveLoadManager.Get_Night_Length();
        dawnLength = 16 - dayLength - nightLength;
    }


    private void PassTime() => UIManager.instance.ShowTime((360 / dayDuration) * Time.deltaTime);
    private void PassHour()
    {
        timerHour.StartTimer();
        timerHour.Tick();

        if(timerHour.IsElapsed())
        {
            currentHour++;
            
            if (currentHour >= dayLength && dayState == DayState.day) // Turn dawn
            {
                if(dawnLength>dayLength) // There is dawn
                    ChangeDayState(DayState.dawn);
                else // There is no dawn
                    ChangeDayState(DayState.night);

            }
            else if (currentHour >= dawnLength && dayState == DayState.dawn) // Turn night
                ChangeDayState(DayState.night);
            else if (currentHour >= nrOfHoursInDay) // Turn day
                ChangeDayState(DayState.day);

        }
    }
    private void PassDay()
    {
        currentHour = 0;
        currentDay++;
        UIManager.instance.ShowDayCount(currentDay);
        // Save world

    }
    private void ChangeDayState(DayState newState)
    {
        WorldManager.instance.SendMobsToSleep(newState);
        WorldManager.instance.WakeUpMobs(newState);

        WorldManager.instance.SetResourcesToHarvest(newState);

        switch (newState)
        {
            case DayState.day:
                {
                    PassDay();
                    sun.GetComponent<Animator>().SetBool("isDay", true);
                    break;
                }
            case DayState.dawn:
                {
                    // Turn on dawn light
                    break;
                }
            case DayState.night:
                {
                    sun.GetComponent<Animator>().SetBool("isDay", false);
                    break;
                }
        }
        dayState = newState;
    }


    void DarknessHitPlayer()
    {

        if (dayState != DayState.night || PlayerStats.instance.isInLight > 0)
        {
            nrOfPopUps = 0;
            timerPlayerInDarkness.RestartTimer();
            return;
        }

        timerPlayerInDarkness.StartTimer();
        timerPlayerInDarkness.Tick();

        if (timerPlayerInDarkness.IsElapsed() && nrOfPopUps == 2)
        {
            nrOfPopUps = 0;
            PopUpManager.instance.ShowPopUpDarkness(3);

            PlayerStats.instance.TakeDmg(25);
        }
        else if (timerPlayerInDarkness.IsElapsedPercent(66) && nrOfPopUps == 1)
        {
            nrOfPopUps++;
            PopUpManager.instance.ShowPopUpDarkness(2);
        }
        else if (timerPlayerInDarkness.IsElapsedPercent(33) && nrOfPopUps == 0)
        {
            nrOfPopUps++;
            PopUpManager.instance.ShowPopUpDarkness(1);
        }
    }
}
