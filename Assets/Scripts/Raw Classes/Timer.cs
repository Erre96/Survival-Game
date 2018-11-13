using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TimerEC
{
    float maxTimerInSeconds;
    float curTimerInSeconds;
    bool active;

    public bool GetActivity()
    {
        return active;
    }

    public void SetActivity(bool active)
    {
        this.active = active;
    }

    public TimerEC()
    {

    }

    public TimerEC(float length)
    {
        this.maxTimerInSeconds = length; //countdown in seconds starts at this value
        this.curTimerInSeconds = length; //seconds left basically
    }

    public void SetNewMaxTimerInSeconds(float length)
    {
        maxTimerInSeconds = length;
        curTimerInSeconds = length;

        if (maxTimerInSeconds < 0.05f)
        {
            maxTimerInSeconds = 0.05f;
        }
    }

    public void SetCurTimerInSeconds(float length)
    {
        curTimerInSeconds = length;
    }

    public float GetCurTimerInSeconds()
    {
        return curTimerInSeconds;
    }

    public float GetMaxTimerInSeconds()
    {
        return maxTimerInSeconds;
    }

    public void Countdown()
    {
        curTimerInSeconds -= 1 * Time.deltaTime;
    }
    public void ResetTimer()
    {
        curTimerInSeconds = maxTimerInSeconds;
        SetActivity(false);
    }
}
