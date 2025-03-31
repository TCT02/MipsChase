using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // External tunables.
    static public float m_fMaxSpeed = 0.10f;
    public float m_fSlowSpeed = m_fMaxSpeed * 0.66f;
    public float m_fIncSpeed = 0.0025f;
    public float m_fMagnitudeFast = 0.6f;
    public float m_fMagnitudeSlow = 0.06f;
    public float m_fFastRotateSpeed = 0.2f;
    public float m_fFastRotateMax = 10.0f;
    public float m_fDiveTime = 0.3f;
    public float m_fDiveRecoveryTime = 0.5f;
    public float m_fDiveDistance = 3.0f;

    // Internal variables.
    public Vector3 m_vDiveStartPos;
    public Vector3 m_vDiveEndPos;
    public float m_fAngle;
    public float m_fSpeed;
    public float m_fTargetSpeed;
    public float m_fTargetAngle;
    public eState m_nState;
    public float m_fDiveStartTime;

    public enum eState : int
    {
        kMoveSlow,
        kMoveFast,
        kDiving,
        kRecovering,
        kNumStates
    }

    private Color[] stateColors = new Color[(int)eState.kNumStates]
    {
        new Color(0,     0,   0),
        new Color(255, 255, 255),
        new Color(0,     0, 255),
        new Color(0,   255,   0),
    };

    public bool IsDiving()
    {
        return (m_nState == eState.kDiving);
    }

    void CheckForDive()
    {
        if (Input.GetMouseButton(0) && (m_nState != eState.kDiving && m_nState != eState.kRecovering))
        {         
            // Start the dive operation
            m_nState = eState.kDiving;
            m_fSpeed = 0.0f;

            // Store starting parameters.
            m_vDiveStartPos = transform.position;
            m_vDiveEndPos = m_vDiveStartPos - (transform.up * m_fDiveDistance); //changed right to up
            m_fDiveStartTime = Time.time;

        }
    }

    void Start()
    {
        // Initialize variables.
        m_fAngle = 0;
        m_fSpeed = 0;
        m_nState = eState.kMoveSlow;
    }

    void UpdateDirectionAndSpeed()
    {
        // Get relative positions between the mouse and player
        Vector3 vScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 vScreenSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Vector2 vOffset = new Vector2(transform.position.x - vScreenPos.x, transform.position.y - vScreenPos.y);

        // print(vScreenPos + " " + vScreenSize + " " + vOffset);
        // Find the target angle being requested.
        m_fTargetAngle = Mathf.Atan2(vOffset.y, vOffset.x) * Mathf.Rad2Deg;

        // Calculate how far away from the player the mouse is.
        float fMouseMagnitude = vOffset.magnitude / vScreenSize.magnitude;

        // Based on distance, calculate the speed the player is requesting.
        if (fMouseMagnitude > m_fMagnitudeFast)
        {
            m_fTargetSpeed = m_fMaxSpeed;
        }
        else if (fMouseMagnitude > m_fMagnitudeSlow)
        {
            m_fTargetSpeed = m_fSlowSpeed;
        }
        else
        {
            m_fTargetSpeed = 0.0f;
        }
    }

    void Update() //Fill this in for the State Machine Assignment
    {
        UpdateDirectionAndSpeed();
        CheckForDive();
        Vector3 vScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float rotateSpeed = 2;
        //print(m_nState);
        //print(m_fDiveStartTime);
        switch (m_nState)
        {
            case eState.kDiving:
                //The player does a dive towards its current direction with no control.
                transform.position = Vector3.Lerp(transform.position, m_vDiveEndPos, 5 * Time.deltaTime);

                if (transform.position == m_vDiveEndPos)
                {
                    print("recovered");

                    //if the current time is greater than the dive start time + a duration, set state.
                    if (Time.time >= m_fDiveStartTime + 1f)
                    {
                        m_nState = eState.kRecovering;
                        if (Time.time >= m_fDiveStartTime + 2f)
                        {
                            m_nState = eState.kMoveSlow;
                        }
                    }
                }
                break;
            default:
                //The player will follow the mouse in normal conditions.

                if (m_fTargetSpeed == m_fSlowSpeed)
                {
                    m_fSpeed = m_fTargetSpeed * 200;
                    m_nState = eState.kMoveSlow;
                    rotateSpeed = m_fFastRotateMax/2;
                }
                if (m_fTargetSpeed == m_fMaxSpeed)
                {
                    m_fSpeed = m_fTargetSpeed * 60;
                    m_nState = eState.kMoveFast;
                    rotateSpeed = m_fFastRotateSpeed;
                }

                if (!(transform.position.x > -11 && transform.position.x < 11 && transform.position.y > -6 && transform.position.y < 6))
                {
                    //If the player slides out of the map, reset it.
                    transform.position = new Vector3(0, 0, 0);
                }

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, m_fTargetAngle - 90f), rotateSpeed * Time.deltaTime);
                transform.position += -transform.up * (m_fSpeed) * Time.deltaTime;
                break;
        }
           

    }
    void FixedUpdate()
    {
        GetComponent<Renderer>().material.color = stateColors[(int)m_nState];
    }

    

}
