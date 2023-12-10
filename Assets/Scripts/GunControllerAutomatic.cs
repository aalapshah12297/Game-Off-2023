using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GunControllerAutomatic : MonoBehaviour
{
    public float gunFiringSpeed;
    public float maxDistance;
    public float growStartTime, growEndTime, shrinkStartTime, shrinkEndTime, cycleLength;
    // Following two intervals must be non-overlapping and contained within [0, cycleLength]
    // [growStartTime, growEndTime]
    // [shrinkStartTime, shrinkEndTime]
    // Set growStartTime or shrinkStartTime > cycleLength to disable grow or shrink
    public SpriteRenderer blueLaser, redLaser;
    public AudioSource blueLaserFire, redLaserFire, blueLaserGrow, redLaserShrink, laserError;
    // Note that both lasers should be children of the gun gameobject,
    // have same origin, transform.position.y = 0 and global scale of 1

    enum ControlState { ready, firingGrow, lockedGrowing, firingShrink, lockedShrinking, wait }
    private ControlState currentState;
    private Vector2 gunDirection;
    private float gunFiringStartTime;
    RaycastHit2D laserHitPoint;
    private GameObject laserHitCube;
    private float nextReadyTime;

    // Start is called before the first frame update
    void Start()
    {
        currentState = ControlState.ready;
        gunDirection = transform.rotation * Vector2.right;
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
            Time.time % cycleLength >= growStartTime &&
            Time.time % cycleLength < growEndTime)
        {
            gunFiringStartTime = Time.time;
            currentState = ControlState.firingGrow;
            blueLaserFire.Play();
        }
        if (currentState == ControlState.lockedGrowing &&
            Time.time % cycleLength >= growEndTime)
            timeout = true;

        // Timed shrink
        if (currentState == ControlState.ready &&
            Time.time % cycleLength >= shrinkStartTime &&
            Time.time % cycleLength < shrinkEndTime)
        {
            gunFiringStartTime = Time.time;
            currentState = ControlState.firingShrink;
            redLaserFire.Play();
        }
        if (currentState == ControlState.lockedShrinking &&
            Time.time % cycleLength >= shrinkEndTime)
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
                    if (currentState == ControlState.lockedGrowing)
                        blueLaserGrow.Play();
                    else if (currentState == ControlState.lockedShrinking)
                        redLaserShrink.Play();
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
            blueLaserGrow.Stop();
            redLaserShrink.Stop();
            if (Time.time % cycleLength >= growStartTime &&
                Time.time % cycleLength < growEndTime)
                nextReadyTime = Time.time + growEndTime - (Time.time % cycleLength);
            else if (Time.time % cycleLength >= shrinkStartTime &&
                Time.time % cycleLength < shrinkEndTime)
                nextReadyTime = Time.time + shrinkEndTime - (Time.time % cycleLength);
            else
                nextReadyTime = Time.time;
            currentState = ControlState.wait;
        }
        else if (currentState == ControlState.wait && Time.time > nextReadyTime)
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
            blueLaserGrow.Stop();
            redLaserShrink.Stop();
            laserError.Play();
            if (Time.time % cycleLength >= growStartTime &&
                Time.time % cycleLength < growEndTime)
                nextReadyTime = Time.time + growEndTime - (Time.time % cycleLength);
            else if (Time.time % cycleLength >= shrinkStartTime &&
                Time.time % cycleLength < shrinkEndTime)
                nextReadyTime = Time.time + shrinkEndTime - (Time.time % cycleLength);
            else
                nextReadyTime = Time.time;
            currentState = ControlState.wait;
        }
    }
}
