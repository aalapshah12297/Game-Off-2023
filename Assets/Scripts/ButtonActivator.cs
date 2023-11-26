using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonActivator : MonoBehaviour
{
    public bool buttonActivated { get; private set; }
    public float activationThresholdY;
    public float activationTime, deactivationTime;
    public SpriteRenderer indicator;

    private bool timerStarted = false;
    private float thresholdCrossTime;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.localPosition.y <= activationThresholdY)
        {
            buttonActivated = true;
            indicator.color = Color.red;
        }
        else
        {
            buttonActivated = false;
            indicator.color = Color.black;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!buttonActivated && transform.localPosition.y <= activationThresholdY)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                thresholdCrossTime = Time.time;
            }
            else if (Time.time - thresholdCrossTime >= activationTime)
            {
                timerStarted = false;
                buttonActivated = true;
                indicator.color = Color.red;
            }
        }
        else if (buttonActivated && transform.localPosition.y > activationThresholdY)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                thresholdCrossTime = Time.time;
            }
            else if (Time.time - thresholdCrossTime >= deactivationTime)
            {
                timerStarted = false;
                buttonActivated = false;
                indicator.color = Color.black;
            }
        }
        else
            timerStarted = false;
    }
}
