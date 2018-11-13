using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    public Transform center;
    SpriteRenderer spriteRenderer;
    bool isMoving = false;
    int fingerId = -1;

    int dirX;
    int dirY;

    PlayerController pcon;


    // Use this for initialization
    void Start()
    {
        center = transform.parent.GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        pcon = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(touch.position);
            position.z = 1;

            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

            if (hit)
            {
                if (hit.collider.gameObject.tag == "Joystick")
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            isMoving = true;
                            fingerId = touch.fingerId;
                            Debug.Log(fingerId);

                            break;
                            
                        case TouchPhase.Moved:
                            if (isMoving && fingerId == touch.fingerId)
                            {
                                transform.position = position;
                                position = PreventCircleEscape(position.x, position.y, 1);
                                MovementPressPoint mpp = hit.collider.gameObject.GetComponent<MovementPressPoint>();
                                Vector3 goHere = new Vector3(mpp.dirX, mpp.dirY, -1);
                                SetStepDirection(goHere);
                            }
                            break;

                        case TouchPhase.Stationary:
                            if(isMoving && fingerId == touch.fingerId)
                            {
                                transform.position = position;
                                position = PreventCircleEscape(position.x,position.y,1);
                                MovementPressPoint mpp = hit.collider.gameObject.GetComponent<MovementPressPoint>();
                                Vector3 goHere = new Vector3(mpp.dirX, mpp.dirY, -1);
                                SetStepDirection(goHere);
                            }
                            break;

                        case TouchPhase.Ended:
                            if (isMoving && fingerId == touch.fingerId)
                            {
                                isMoving = false;
                                fingerId = -1;
                                transform.position = center.position;
                                ResetDir();
                            }
                            break;
                    }
                }
            }
        }
    }

    public void ResetDir()
    {
        dirX = Mathf.RoundToInt(center.position.x);
        dirY = Mathf.RoundToInt(center.position.y);
    }

    Vector3 PreventCircleEscape(float x, float y,int maxDist)
    {

        if(transform.position.x > center.position.x + maxDist)
        {
            x = center.position.x + maxDist;
        }

        if (transform.position.x < center.position.x - maxDist)
        {
            x = center.position.x - maxDist;
        }

        if (transform.position.y > center.position.y + maxDist)
        {
            y = center.position.y + maxDist;
        }

        if (transform.position.y < center.position.y - maxDist)
        {
            y = center.position.y - maxDist;
        }
        
        return new Vector3(x, y, -1);
    }

    string GetDir()
    {
        //print(dirX + "    " + dirY);
        string ret = "";

        while(ret == "")
        {
            if (dirX == -1 && dirY == -1)
            {
                ret = "ld";
                break;
            }

            if (dirX == 0 && dirY == -1)
            {
                ret = "d";
                break;
            }

            if (dirX == 1 && dirY == -1)
            {
                ret = "rd";
                break;
            }

            if (dirX == -1 && dirY == 0)
            {
                ret = "l";
                break;
            }
            
            if (dirX == 0 && dirY == 0)
            {
                break;
            }

            if (dirX == 1 && dirY == 0)
            {
                ret = "r";
                break;
            }

            if (dirX == -1 && dirY == 1)
            {
                ret = "lu";
                break;
            }

            if (dirX == 0 && dirY == 1)
            {
                ret = "u";
                break;
            }

            if (dirX == 1 && dirY == 1)
            {
                ret = "ru";
                break;
            }
        }
        
        return ret;
    }

    void shit()
    {

    }
    public void SetStepDirection(Vector3 position)
    {
        dirX =  Mathf.RoundToInt(position.x);
        dirY = Mathf.RoundToInt(position.y);
        string dir = GetDir();
        pcon.pinfo.SetDir(dir); // this replaces input manager in playerController which is for keyboard movement

        if (pcon.walking == false)
        {
            pcon.movementAttempt = true;
        }
    }
}
