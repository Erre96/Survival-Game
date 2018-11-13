using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButtonController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    int fingerId = -1;

    PlayerController pcon;


    // Use this for initialization
    void Start()
    {
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
                if (hit.collider.gameObject.tag == "AttackButton")
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            pcon.Attack(true);
                            fingerId = touch.fingerId;
                            break;


                        case TouchPhase.Stationary:
                            if (fingerId == touch.fingerId)
                            {
                                pcon.Attack(true);
                            }
                            break;

                        case TouchPhase.Ended:
                            if (fingerId == touch.fingerId)
                            {
                                fingerId = -1;
                            }
                            break;
                    }
                }
            }
        }
    }
}