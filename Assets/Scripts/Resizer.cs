using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using UnityEditor.Build;
using UnityEngine;

public class Resizer : MonoBehaviour
{
    public float scaleIncrement;    // only isotropic scaling supported (x = y)
    public float minRelativeScale, maxRelativeScale;
    public float overlapThreshold;

    private BoxCollider2D boxCollider;
    private Rigidbody2D objectBody;
    private Collider2D[] overlappingColliders = new Collider2D[64];
    private float initialMass;
    private float initialScale, minScale, maxScale;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        objectBody = GetComponent<Rigidbody2D>();
        initialMass = objectBody.mass;
        initialScale = transform.localScale.x;
        minScale = initialScale * minRelativeScale;
        maxScale = initialScale * maxRelativeScale;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool Grow()
    {
        // Revert last grow operation if it resulted in an overlap
        int overlapCount = boxCollider.OverlapCollider(new ContactFilter2D().NoFilter(), overlappingColliders);
        for (int i = 0; i < overlapCount; i++)
        {
            float overlapDistance = boxCollider.Distance(overlappingColliders[i]).distance;
            if (overlapDistance < -overlapThreshold)
            {
                transform.localScale = new Vector3(transform.localScale.x + overlapDistance,
                    transform.localScale.y + overlapDistance, transform.localScale.z);
                UpdateMass();
                return false;
            }
        }

        // Normal grow operation
        if (transform.localScale.x + scaleIncrement <= maxScale)
        {
            transform.localScale = new Vector3(transform.localScale.x + scaleIncrement,
                transform.localScale.y + scaleIncrement, transform.localScale.z);
            UpdateMass();
            return true;
        }
        else
            return false;
    }

    public bool Shrink()
    {
        // Shrink operation
        if (transform.localScale.x - scaleIncrement >= minScale)
        {
            transform.localScale = new Vector3(transform.localScale.x - scaleIncrement,
                transform.localScale.y - scaleIncrement, transform.localScale.z);
            UpdateMass();
            return true;
        }
        else
            return false;
    }

    void UpdateMass()
    {
        // Mass modification based on scale
        objectBody.mass = initialMass * (transform.localScale.x / initialScale) * (transform.localScale.x / initialScale);
    }
}
