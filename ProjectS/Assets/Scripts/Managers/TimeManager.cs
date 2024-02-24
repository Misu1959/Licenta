using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public enum DayState
    {
        day,
        dawn,
        night
    }


    [SerializeField] private GameObject sun;

    [SerializeField] private float dayDuration;
    [Range(1,16)][SerializeField] private int dayLength;
    [Range(1,16)][SerializeField] private int dawnLength;

    private const int nrOfHoursInDay = 16;
    private const float hourLength = .0625f;

    private int currentHour;
    private int currentDay;

    public DayState dayState { get; private set; }
    private Timer timerHour;
    
    private Timer timerPlayerInDarkness;
    private int nrOfPopUps;

    private IEnumerator Start()
    {
        instance = this;

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

    }

    void Update()
    {
        PassTime();
        PassHour();


        DarknessHitPlayer();
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
        // Save world

    }
    private void ChangeDayState(DayState newState)
    {
        switch(newState)
        {
            case DayState.day:
                {
                    PassDay();
                    break;
                }
            case DayState.dawn:
                {
                    // Turn on dawn light
                    break;
                }
            case DayState.night:
                {
                    // Turn of light
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
        else if (timerPlayerInDarkness.IsElapsedPercent(2 / 3) && nrOfPopUps == 1)
        {
            nrOfPopUps++;
            PopUpManager.instance.ShowPopUpDarkness(2);
        }
        else if (timerPlayerInDarkness.IsElapsedPercent(1 / 3) && nrOfPopUps == 0)
        {
            nrOfPopUps++;
            PopUpManager.instance.ShowPopUpDarkness(1);
        }
    }
}
