using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Target : MonoBehaviour
{
    public Player m_player;
    public enum eState : int
    {
        kIdle,
        kHopStart,
        kHop,
        kCaught,
        kNumStates
    }

    private Color[] stateColors = new Color[(int)eState.kNumStates]
   {
        new Color(255, 0,   0),
        new Color(0,   255, 0),
        new Color(0,   0,   255),
        new Color(255, 255, 255)
   };

    // External tunables.
    public float m_fHopTime = 0.2f;
    public float m_fHopSpeed = 6.5f;
    public float m_fScaredDistance = 3.0f;
    public int m_nMaxMoveAttempts = 50;

    // Internal variables.
    public eState m_nState;
    public float m_fHopStart;
    public Vector3 m_vHopStartPos;
    public Vector3 m_vHopEndPos;

    void Start()
    {      
        // Setup the initial state and get the player GO.
        m_nState = eState.kIdle;
        //m_player = GameObject.FindObjectOfType(typeof(Player)) as Player;
        m_player = GameObject.FindFirstObjectByType(typeof(Player)) as Player; //The previous line is outdated and doesn't work
    }

    private void Update() //Fill this in for the State Machine Assignment
    {
        
        if (m_nState != eState.kCaught)
        {
            //calulate distance between player and rabbit
            float distance = Vector3.Distance(gameObject.transform.position, m_player.transform.position);
            
            //int moveDist = 1;
            Vector2 moveDist = new Vector2(
                Mathf.Abs(gameObject.transform.position.x - m_player.transform.position.x)
                , Mathf.Abs(gameObject.transform.position.y - m_player.transform.position.y)
                );

            //If the distance between the player and rabbit is within the scare distance...
            if (distance <= m_fScaredDistance) // && m_nMaxMoveAttempts > 0
            {
                m_nState = eState.kHopStart;
                //Calculate the new position to run
                m_vHopStartPos = transform.position; //Start position

                //if (transform.position.x > -9.5 && transform.position.x < 9.5 && transform.position.y > -4.5 && transform.position.y < 4.5)
                //{
                    if (m_player.transform.position.x < transform.position.x)
                    {
                        m_vHopEndPos.x = m_vHopStartPos.x + moveDist.x;
                        print("Running right");

                        

                    }
                    if (m_player.transform.position.x > transform.position.x)
                    {
                        m_vHopEndPos.x = m_vHopStartPos.x - moveDist.x;
                        print("Running left");

 
                    }
                    if (m_player.transform.position.y < transform.position.y)
                    {
                        m_vHopEndPos.y = m_vHopStartPos.y + moveDist.y;
                        print("Running up");


                    }
                    if (m_player.transform.position.y > transform.position.y)
                    {
                        m_vHopEndPos.y = m_vHopStartPos.y - moveDist.y;
                        print("Running down");

                    }
                //}
                //else
                //{
                    //reset the rabbits' direction back inside the map.
                   // m_vHopEndPos = new Vector3(0, 0, 0);
                //}

                //If the end position is outside the bounds, limit them to the bound.
                if (m_vHopEndPos.x > 9.5)
                {
                    m_vHopEndPos.x = 9.5f;
                }
                if (m_vHopEndPos.x < -9.5)
                {
                    m_vHopEndPos.x = -9.5f;
                }
                if (m_vHopEndPos.y > 4.5)
                {
                    m_vHopEndPos.y = 4.5f;
                }
                if (m_vHopEndPos.y < -4.5)
                {
                    m_vHopEndPos.y = -4.5f;
                }

                // Turn the rabbit towards the new location
                Vector2 vOffset = new Vector2(transform.position.x - m_vHopEndPos.x, transform.position.y - m_vHopEndPos.y);             
                float dirAngle = Mathf.Atan2(vOffset.y, vOffset.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, dirAngle + 90f);
                
                //Run away to the new position
                transform.position = Vector3.Lerp(transform.position, m_vHopEndPos, m_fHopSpeed * Time.deltaTime);
                m_nMaxMoveAttempts -= 1;

            }
            else //Otherwise, do nothing and stay idle.
            {
                m_nState = eState.kIdle;
            }

        }
        else
        {
            print("Rabbit was caught!");
        }

    }

    void FixedUpdate()
    {
        GetComponent<Renderer>().material.color = stateColors[(int)m_nState];
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // Check if this is the player (in this situation it should be!)
        if (collision.gameObject == GameObject.Find("Player"))
        {
            // If the player is diving, it's a catch!
            if (m_player.IsDiving())
            {
                m_nState = eState.kCaught;
                transform.parent = m_player.transform;
                transform.localPosition = new Vector3(0.0f, -0.5f, 0.0f);
            }
        }
    }
}