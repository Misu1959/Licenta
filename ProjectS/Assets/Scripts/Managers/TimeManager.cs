using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private GameObject sun;

    [SerializeField] private float dayLength;
    [Range(0,1)][SerializeField] private float dayTime;
    private float currentTime;
    private bool isDay = true;
    private int currentDay;


    private float maxTimeForPlayerInDarkness = 6;
    private float timeForPlayerInDarkness;

    private int nrOfPopUps;

    private IEnumerator Start()
    {
        yield return null;

        currentDay = PlayerPrefs.GetInt("currentDay");

        UIManager.instance.SetClock(dayTime);
        UIManager.instance.ShowDayCount(currentDay);

    }

    void Update()
    {
        UIManager.instance.ShowTime((360 / dayLength) * Time.deltaTime);

        currentTime += Time.deltaTime;


        if (currentTime >= dayLength - 1)
        {
            currentDay++;
            currentTime -= dayLength;
            
            StartCoroutine(SetDayTime(true));
            SaveLoadManager.instance.SaveWorld();
            PlayerPrefs.SetInt("currentDay", currentDay); // Save day

        }
        else if (currentTime >= (dayTime * dayLength) - 1 && isDay)
            if(sun.GetComponent<Animator>().GetBool("isDay"))
                StartCoroutine(SetDayTime(false));

        DarknessHitPlayer();
    }

    IEnumerator SetDayTime(bool _isDay)
    {
        if (sun.GetComponent<Animator>().GetBool("isDay") != _isDay)
            sun.GetComponent<Animator>().SetBool("isDay", _isDay);

        yield return new WaitForSeconds(1.25f);
        isDay = _isDay;
    }

    void DarknessHitPlayer()
    {
        if (isDay || PlayerStats.instance.isInLight > 0)
        {
            if (timeForPlayerInDarkness != maxTimeForPlayerInDarkness)
            {
                timeForPlayerInDarkness = maxTimeForPlayerInDarkness;
                nrOfPopUps = 0;
            }
            return;
        }

        if (timeForPlayerInDarkness <= 0)
        {
            PlayerStats.instance.TakeDmg(25);
            PopUpManager.instance.ShowPopUpDarkness(3);

            nrOfPopUps = 0;
            timeForPlayerInDarkness = maxTimeForPlayerInDarkness;
        }
        else if (timeForPlayerInDarkness < maxTimeForPlayerInDarkness / 3)
        {
            if (nrOfPopUps == 1)
            {
                PopUpManager.instance.ShowPopUpDarkness(2);
                nrOfPopUps++;
            }
        }
        else if (timeForPlayerInDarkness < (2 * maxTimeForPlayerInDarkness) / 3)
        {
            if(nrOfPopUps == 0)
            {
                PopUpManager.instance.ShowPopUpDarkness(1);
                nrOfPopUps++;
            }
        }
        timeForPlayerInDarkness-=Time.deltaTime;

    }
}
