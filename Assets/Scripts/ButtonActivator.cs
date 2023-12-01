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
    public AudioSource buttonDown, buttonUp;

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
                buttonDown.Play();
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
                buttonUp.Play();
            }
        }
        else
            timerStarted = false;
    }
}
