using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AnimationManager : MonoBehaviour
{
    public string curAnim;
    public int spriteIndexLowest;
    public int spriteIndexHighest;
    public float indexCurValue = 0;
    public int frames;
    public Sprite[] sprites = new Sprite[20];
    AnimationManager anim;
    SpriteRenderer SpriteRenderer;
    bool animate;
    TimerEC changeStanceTimer;

    public void SetSpriteLoopValues(int min, int max, string dir)
    {
        spriteIndexHighest = max;
        spriteIndexLowest = min;

        if (curAnim != dir)
        {
            indexCurValue = min;
            curAnim = dir;

            if (curAnim == "walking_left")
            {
                transform.localScale = new Vector3(-3, 3, 1);
            }

            if (curAnim == "walking_right")
            {
                transform.localScale = new Vector3(3, 3, 1);
            }
        }
    }

    public void ChangeDir()
    {
        bool allowed = UpdateCountdownStanceTimer();
        if(allowed)
        {
            SpriteRenderer.sprite = sprites[spriteIndexLowest];
        }
    }

    void Start()
    {
        changeStanceTimer = new TimerEC(0.1f);
        anim = this;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 12; i++)
        {
            sprites[i] = Resources.Load<Sprite>("Spritesheets/Warrior/"+i);
        }

        animate = true;
        anim.SetSpriteLoopValues(0, 3, "walking_down");
    }

    void Update()
    {

    }

    public void AnimateSprite(bool state)
    {
        animate = state;

        if(animate == true)
        {
            changeStanceTimer.ResetTimer();
            Move();
        }

        if (animate == false)
        {
            bool allowed = UpdateCountdownStanceTimer();
            if(allowed)
            {
                StayIdle();
            }
        }
    }
    void Move()
    {
        anim.indexCurValue += Time.deltaTime * frames;
        SpriteRenderer.sprite = sprites[Mathf.RoundToInt(anim.indexCurValue)];
        if (anim.indexCurValue > anim.spriteIndexHighest)
        {
            anim.indexCurValue = anim.spriteIndexLowest;
        }
    }


    bool UpdateCountdownStanceTimer()
    {
        changeStanceTimer.Countdown();
        float tl = changeStanceTimer.GetCurTimerInSeconds();
        if(tl < 0)
        {
            changeStanceTimer.ResetTimer();
            return true;
        }
        return false;
    }
    public void StayIdle()
    {
        indexCurValue = anim.spriteIndexLowest;
        SpriteRenderer.sprite = sprites[Mathf.RoundToInt(anim.spriteIndexLowest)];
    }
}