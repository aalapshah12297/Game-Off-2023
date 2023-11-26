using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor.Build;
using UnityEngine;

public class Resizer : MonoBehaviour
{
    public float scaleIncrement;
    public float minRelativeScale, maxRelativeScale;
    public float overlapThreshold;
    public float resizerTimeoutDuration;

    enum ControlState { Ready, Grow, Shrink, Wait }
    private ControlState currentState;
    private float previousWaitTime;
    private BoxCollider2D boxCollider;
    private Rigidbody2D objectBody;
    private Camera mainCamera;
    private Collider2D[] overlappingColliders = new Collider2D[64];
    private float initialMass;
    private float initialScale, minScale, maxScale;

    // Start is called before the first frame update
    void Start()
    {
        currentState = ControlState.Ready;
        boxCollider = GetComponent<BoxCollider2D>();
        objectBody = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        initialMass =  objectBody.mass;
        initialScale = transform.localScale.x;
        minScale = initialScale * minRelativeScale;
        maxScale = initialScale * maxRelativeScale;
    }

    // Update is called once per frame
    private void Update()
    {
        if (currentState == ControlState.Ready && Input.GetMouseButtonDown(0))
            if (boxCollider.OverlapPoint(mainCamera.ScreenToWorldPoint(Input.mousePosition)))
                currentState = ControlState.Grow;
        if (currentState == ControlState.Grow && Input.GetMouseButtonUp(0))
                currentState = ControlState.Ready;

        if (currentState == ControlState.Ready && Input.GetMouseButtonDown(1))
            if (boxCollider.OverlapPoint(mainCamera.ScreenToWorldPoint(Input.mousePosition)))
                currentState = ControlState.Shrink;
        if (currentState == ControlState.Shrink && Input.GetMouseButtonUp(1))
            currentState = ControlState.Ready;
    }

    // Keeping resize logic in fixedupdate allows predictable grow/shrink speeds & behaviour
    void FixedUpdate()
    {
        // Revert last grow operation if it resulted in an overlap
        if (currentState == ControlState.Grow)
        {
            int overlapCount = boxCollider.OverlapCollider(new ContactFilter2D().NoFilter(), overlappingColliders);
            for (int i = 0; i < overlapCount; i++)
            {
                float overlapDistance = boxCollider.Distance(overlappingColliders[i]).distance;
                if (overlapDistance < -overlapThreshold)
                {
                    transform.localScale = new Vector3(transform.localScale.x - scaleIncrement,
                        transform.localScale.y - scaleIncrement, transform.localScale.z);
                    previousWaitTime = Time.time;
                    currentState = ControlState.Wait;
                    break;
                }
            }
        }

        // Normal grow operation
        if (currentState == ControlState.Grow)
        {
            if (transform.localScale.x + scaleIncrement <= maxScale)
                transform.localScale = new Vector3(transform.localScale.x + scaleIncrement,
                    transform.localScale.y + scaleIncrement, transform.localScale.z);
            else
            {
                previousWaitTime = Time.time;
                currentState = ControlState.Wait;
            }
        }

        // Shrink operation
        if (currentState == ControlState.Shrink)
        {
            if (transform.localScale.x - scaleIncrement >= minScale)
                transform.localScale = new Vector3(transform.localScale.x - scaleIncrement,
                    transform.localScale.y - scaleIncrement, transform.localScale.z);
            else
            {
                previousWaitTime = Time.time;
                currentState = ControlState.Wait;
            }
        }

        // Mass modification based on scale
        objectBody.mass = initialMass * (transform.localScale.x / initialScale) * (transform.localScale.x / initialScale);

        // Timeout for invalid operation
        if (currentState == ControlState.Wait)
        {
            if (Time.time - previousWaitTime >= resizerTimeoutDuration)
                currentState = ControlState.Ready;
        }
    }
}
