using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Timer
{
    public float duration;
    public float durLeft;
    public bool active;

    public bool GetState()
    {
        return active;
    }

    public void SetState(bool active)
    {
        this.active = active;
    }

    public void SetValues(float duration, float durLeft)
    {
        this.duration = duration;
        this.durLeft = durLeft;
    }

    public void DecreaseOne()
    {
        if (durLeft > 0)
        {
            durLeft--;
        }
        RefreshState();
    }

    public void RefreshState()
    {
        if (durLeft <= 0)
        {
            durLeft = duration;
            active = false;
        }

        else active = true;
    }
}