using System;
using UnityEngine;

[Serializable]
public class Timer
{
    [SerializeField]
    private float maxTime;
    private float remainedtime;
    private bool isOn;

    public Timer(float _maxTime)
    {
        isOn             = false;

        maxTime         = _maxTime;
        remainedtime    = _maxTime;
        
    }

    public Timer(float _maxTime, float _remainedTime)
    {
        isOn            = false;

        maxTime         = _maxTime;
        remainedtime    = _remainedTime;
    }
    
    public void SetTime(float amount)
    {
        remainedtime = Mathf.Clamp(amount, 0, maxTime);
    }

    public void AddTime(float _timeToAdd) // Ads time to timer remained time
    {
        remainedtime = Mathf.Clamp(remainedtime + _timeToAdd, 0, maxTime); // clamps the time between 0 and maxTime
        StartTimer();// Start the timer if the timer is elapsed before adding time
    }

    public void StartTimer() // Can be caled mutiple times, it will only start once (if it's not already started)
    {
        if (isOn) return;// If timer is on don't start again

        remainedtime = remainedtime <= 0 ? maxTime : remainedtime; // Set the time to max time if remained time is 0 is given
        isOn = true; // Turn timer on
    }

    public void RestartTimer() // Can be caled mutiple times, it will only restart once
    {
        if (!isOn) return;// If timer is of we don't restart it


        remainedtime = maxTime; // set the time
        isOn = false; // Turn timer of
    }
    
    public void Tick() // Needs to be called every frame
    {
        if (!isOn) return;// If timer is not on return


        remainedtime -= Time.deltaTime; // Elapse time from timer

        if (remainedtime <= 0) // If the time elapsed 
            isOn = false; // Turn timer on
    }


    private float ElapsedTime() => maxTime - remainedtime;

    public float RemainedTime() => remainedtime;

    public float MaxTime() => maxTime;

    public bool IsOn() => isOn;

    public void StopTimer() => isOn = false;

    public void ResumeTimer() => isOn = true;



    public bool IsElapsed() => remainedtime > 0 ? false : true;

    public bool IsElapsedPercent(float percent) => ElapsedTime() < (percent / 100) * maxTime ? false : true;
}
