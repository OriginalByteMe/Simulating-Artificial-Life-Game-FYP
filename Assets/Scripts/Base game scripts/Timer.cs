using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding; // A* pathfinding by arongranberg

public class Timer : MonoBehaviour
{
    public float CountDownTimeLeft = 3.0f;
    public float SimTimeLeft = 3.0f;
    public bool CountDownTimerOn = false;
    public bool SimTimerOn = false;

    private float CountDownTracker;
    private float SimActivationTime = 3f;

    public Text TimerText; // used for showing countdown from 3, 2, 1 

    void Start()
    {
        CountDownTimerOn = true;
        TimerText = GetComponent<Text>();
        CountDownTracker = CountDownTimeLeft;
    }
    // Update is called once per frame
    void Update()
    {
        if (CountDownTimerOn)
        {
            if(CountDownTimeLeft > 0)
            {
                CountDownTimeLeft -= Time.deltaTime;
                updateTimer(CountDownTimeLeft);
            }
            else
            {
                Debug.Log("Timer is up, beginning simulation.");
                CountDownTimeLeft = 0;
                CountDownTimerOn = false;
                SimTimerOn = true;
            }
        }

        if (SimTimerOn)
        {
            if (SimTimeLeft > 0)
            {
                SimTimeLeft -= Time.deltaTime;
                
            }
            else
            {
                Debug.Log("Timer 2 is up, stopping simulation, starting new counter.");
                SimTimeLeft = SimActivationTime;
                if (CountDownTracker <= 25)
                {
                    CountDownTimeLeft = 25;
                }
                else
                {
                    CountDownTracker /= 2;
                    CountDownTimeLeft = CountDownTracker;
                }
                CountDownTimerOn = true;
                SimTimerOn = false;
                AstarPath.active.Scan();
            }
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        TimerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    private void StartSimTime()
    {
        SimTimerOn = true ;
        CountDownTimeLeft = 10;

    }

    public bool getCountDownTimeFinished { 
        get
        {
            return CountDownTimerOn;
        }
        set
        {
            CountDownTimerOn = value;
        }
    }

    public bool getSimTimeFinished { 
        get
        {
            return SimTimerOn;
        }
        set
        {
            SimTimerOn = value;
        }
    }
}
