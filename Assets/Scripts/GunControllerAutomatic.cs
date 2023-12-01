using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControllerAutomatic : MonoBehaviour
{
    public float gunFiringSpeed;
    public float maxDistance;
    public float gunTimeoutDuration;
    public SpriteRenderer blueLaser, redLaser;
    public Vector2 gunDirection;
    public float cycleLength; // > 2 * (growShrinkTime + gunTimeoutDuration)
    public float growShrinkTime;
    // Note that both lasers should be children of the gun gameobject,
    // have same origin, transform.position.y = 0 and global scale of 1

    enum ControlState { ready, firingGrow, lockedGrowing, firingShrink, lockedShrinking, wait }
    private ControlState currentState;
    private float gunFiringStartTime;
    RaycastHit2D laserHitPoint;
    private GameObject laserHitCube;
    private float previousWaitTime;

    // Start is called before the first frame update
    void Start()
    {
        currentState = ControlState.ready;
        gunDirection.Normalize();
        transform.rotation = Quaternion.Euler(
            0, 0, Mathf.Rad2Deg * Mathf.Atan2(gunDirection.y, gunDirection.x));
    }

    // Update is called once per frame
    void Update()
    {
        bool timeout = false;   // used to reset at the end of the loop

        // Timed grow
        if (currentState == ControlState.ready &&
            Time.time % cycleLength >= 0.0f &&
            Time.time % cycleLength < growShrinkTime)
        {
            gunFiringStartTime = Time.time;
            currentState = ControlState.firingGrow;
        }
        if (currentState == ControlState.lockedGrowing &&
            Time.time % cycleLength >= growShrinkTime)
            timeout = true;

        // Timed shrink
        if (currentState == ControlState.ready &&
            Time.time % cycleLength >= 0.5f * cycleLength &&
            Time.time % cycleLength < 0.5f * cycleLength + growShrinkTime)
        {
            gunFiringStartTime = Time.time;
            currentState = ControlState.firingShrink;
        }
        if (currentState == ControlState.lockedShrinking &&
            Time.time % cycleLength >= 0.5f * cycleLength + growShrinkTime)
            timeout = true;

        // Laser firing
        if (currentState == ControlState.firingGrow || currentState == ControlState.firingShrink)
        {
            laserHitPoint = Physics2D.Raycast(blueLaser.transform.position, gunDirection);
            float distance;
            if (laserHitPoint.collider == null)
                distance = maxDistance;
            else
                distance = laserHitPoint.distance;
            float fraction = gunFiringSpeed * (Time.time - gunFiringStartTime) / distance;
            if (fraction <= 1)
            {
                if (fraction * distance <= maxDistance)
                {
                    if (currentState == ControlState.firingGrow)
                        blueLaser.size = new Vector2(fraction * distance, blueLaser.size.y);
                    if (currentState == ControlState.firingShrink)
                        redLaser.size = new Vector2(fraction * distance, redLaser.size.y);
                }
                else
                    timeout = true;
            }
            else
            {
                if (laserHitPoint.collider.gameObject.GetComponent<Resizer>() != null)
                {
                    laserHitCube = laserHitPoint.collider.gameObject;
                    currentState++; // firing -> locked
                }
                else
                    timeout = true;
            }
        }
        // Laser locked
        else if (currentState == ControlState.lockedGrowing || currentState == ControlState.lockedShrinking)
        {
            laserHitPoint = Physics2D.Raycast(blueLaser.transform.position, gunDirection);
            if (laserHitPoint.distance <= maxDistance)
            {
                if (currentState == ControlState.lockedGrowing)
                    blueLaser.size = new Vector2(laserHitPoint.distance, blueLaser.size.y);
                if (currentState == ControlState.lockedShrinking)
                    redLaser.size = new Vector2(laserHitPoint.distance, redLaser.size.y);
            }
            else
                timeout = true;
        }

        // Timeout for invalid operation
        if (timeout)
        {
            blueLaser.size = new Vector2(0, blueLaser.size.y);
            redLaser.size = new Vector2(0, redLaser.size.y);
            previousWaitTime = Time.time;
            currentState = ControlState.wait;
        }
        else if (currentState == ControlState.wait &&
            Time.time - previousWaitTime >= gunTimeoutDuration)
            currentState = ControlState.ready;
    }

    // Keeping resize logic in FixedUpdate allows consistent grow/shrink speeds & behaviour
    void FixedUpdate()
    {
        bool timeout = false;   // used to reset at the end of the loop

        // Calling locked cube's resizer to attempt to grow/shrink
        if (currentState == ControlState.lockedGrowing || currentState == ControlState.lockedShrinking)
        {
            if ((laserHitPoint.collider != null && laserHitPoint.collider.gameObject == laserHitCube))
            {
                if (currentState == ControlState.lockedGrowing &&
                    !laserHitCube.GetComponent<Resizer>().Grow())
                    timeout = true;
                if (currentState == ControlState.lockedShrinking &&
                    !laserHitCube.GetComponent<Resizer>().Shrink())
                    timeout = true;
            }
            else
                timeout = true;
        }

        // Timeout for invalid operation
        if (timeout)
        {
            blueLaser.size = new Vector2(0, blueLaser.size.y);
            redLaser.size = new Vector2(0, redLaser.size.y);
            previousWaitTime = Time.time;
            currentState = ControlState.wait;
        }
    }
}
