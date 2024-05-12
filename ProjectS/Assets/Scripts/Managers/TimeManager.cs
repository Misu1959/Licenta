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

    private const int nrOfHoursInDay = 16;
    private const float hourLength = 100 / (float)nrOfHoursInDay / 100; // How much does one hour represent of a day length in percents 

    private int dayDuration;
    private int dayLength;
    private int dawnLength;
    private int nightLength;


    public float currentTime { get; private set; }
    public int currentHour { get; private set; }
    public int currentDay { get; private set; }

    public DayState dayState { get; private set; }
    private Timer timerHour;
    
    private Timer timerPlayerInDarkness;
    private int nrOfPopUps;

    private void Awake()
    {
        instance = this;
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        PassTime();
        PassHour();


        DarknessHitPlayer();
    }

    public void SetTimeSettings(int _dayDuration, int _daylength, int _nightLength)
    {
        dayDuration = _dayDuration;
        dayLength   = _daylength;
        nightLength = _nightLength;
        dawnLength  = 16 - dayLength - nightLength;

        currentTime = 0;
        currentHour = 0;
        currentDay  = 1;
        dayState    = dayLength == 0 ? DayState.dawn : DayState.day;

        timerHour = new Timer(hourLength * dayDuration);
        timerPlayerInDarkness = new Timer(6);

        ChangeDayState(dayState, false);

        UIManager.instance.SetClock(dayLength * hourLength, dawnLength * hourLength, 0);
        UIManager.instance.ShowDayCount(currentDay);
    }

    public void SetTimeSettings(int _dayDuration,int _daylength,int _nightLength,int _currentTime,int _currentHour,int _currentDay,int _dayState)
    {
        dayDuration = _dayDuration;
        dayLength   = _daylength;
        nightLength = _nightLength;
        dawnLength  = 16 - dayLength - nightLength;

        currentTime = _currentTime;
        currentHour = _currentHour;
        currentDay  = _currentDay;
        dayState    = (DayState)_dayState;

        float hourDuration = hourLength * dayDuration;
        float remainedTimeFromHour = _currentTime % hourDuration;
        timerHour = new Timer(hourDuration, hourDuration - remainedTimeFromHour);
        timerPlayerInDarkness = new Timer(6);

        ChangeDayState(dayState, false);



        UIManager.instance.SetClock(dayLength * hourLength, dawnLength * hourLength, 360 * (currentTime / dayDuration));
        UIManager.instance.ShowDayCount(currentDay);
    }


    private void PassTime()
    {
        currentTime += Time.deltaTime;
        UIManager.instance.ShowTime((360f / dayDuration) * Time.deltaTime);
    
    }
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
    private void ChangeDayState(DayState newState, bool passDay = true)
    {
        WorldManager.instance.SendMobsToSleep();
        WorldManager.instance.WakeUpMobs();

        WorldManager.instance.SetResourcesToHarvest();

        switch (newState)
        {
            case DayState.day:
                {
                    if(passDay)
                        PassDay();
    
                    WorldManager.instance.sun.GetComponent<Animator>().SetBool("isDay", true);
                    break;
                }
            case DayState.dawn:
                {
                    // Turn on dawn light
                    break;
                }
            case DayState.night:
                {
                    WorldManager.instance.sun.GetComponent<Animator>().SetBool("isDay", false);
                    break;
                }
        }
        dayState = newState;
    }


    private void DarknessHitPlayer()
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
