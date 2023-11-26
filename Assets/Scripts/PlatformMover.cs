using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public ButtonActivator activator;
    public GameObject initialPosition, finalPosition;
    public float moveDuration;

    private Rigidbody2D platformBody;
    private bool buttonWasActive;
    private float lerpStartTime;
    private Vector2 lerpStartPosition;

    // Start is called before the first frame update
    void Start()
    {
        platformBody = GetComponent<Rigidbody2D>();
        buttonWasActive = activator.buttonActivated;  
        lerpStartPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!buttonWasActive && activator.buttonActivated ||
            buttonWasActive && !activator.buttonActivated)
        {
            lerpStartTime = Time.time;
            lerpStartPosition = transform.position;
        }

        if (activator.buttonActivated)
            platformBody.MovePosition(Vector2.Lerp(
                lerpStartPosition, finalPosition.transform.position, (Time.time - lerpStartTime) / moveDuration));
        else
            platformBody.MovePosition(Vector2.Lerp(
                lerpStartPosition, initialPosition.transform.position, (Time.time - lerpStartTime) / moveDuration));

        buttonWasActive = activator.buttonActivated;
    }
}
