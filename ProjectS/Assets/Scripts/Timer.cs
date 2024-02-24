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
        isOn = false;

        maxTime= _maxTime;
        remainedtime = maxTime;
        
    }

    public Timer(float _maxTime, float _remainedTime)
    {
        isOn = false;

        maxTime = _maxTime;
        remainedtime = _remainedTime;
    }


    public void Tick() // Needs to be called every frame
    {
        if (!isOn) // If timer is not on return
            return;

        remainedtime -= Time.deltaTime; // Elapse time from timer

        if (remainedtime <= 0) // If the time elapsed 
            isOn = false; // Turn timer on
    }

    public void StartTimer() // Can be caled mutiple times, it will only start once (if it's not already started)
    {
        if (isOn) // If timer is on don't start again
            return;

        remainedtime = remainedtime <= 0 ? maxTime : remainedtime; // Set the time to max time if remained time is 0 is given
        isOn = true; // Turn timer on
    }

    public void RestartTimer() // Can be caled mutiple times, it will only restart once
    {
        if (!isOn) // If timer is of we don't restart it
            return;
        
        remainedtime = maxTime; // set the time
        isOn = false; // Turn timer of
    }

    public void StopTimer() // Stops the timer
    {
        isOn = false; // Turn timer of
    }

    public void ResumeTimer() // Resumes timer
    {
        isOn = true;
    }

    public void SetTime(float amount)
    {
        if (amount > maxTime)
            amount = maxTime;
            
        remainedtime = amount;
    }

    public void AddTime(float _timeToAdd) // Ads time to timer remained time
    {
        remainedtime = Mathf.Clamp(remainedtime + _timeToAdd, 0, maxTime); // clamps the time between 0 and maxTime
        
        StartTimer();// Start the timer if the timer is elapsed before adding time
    }

    public float RemainedTime() { return remainedtime; }// Returns the remained time

    public bool IsOn() // Return either the timer is on or of
    {
        return isOn;
    }

    public bool IsElapsed() // Check if the timer is completely elapsed
    {
        return remainedtime > 0 ? false : true; // return true if the timer is elapsed, false if not
    }

    public bool IsElapsedPercent(float percent) // Check if the timer has elapsed more than %
    {
        return maxTime - remainedtime < (percent/100) * maxTime ? false: true; // return true if the timer has elapsed more then %of the time
    }

}
