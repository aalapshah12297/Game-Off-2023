using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody2D playerBody;
    public EdgeCollider2D levelBounds;
    public float deadZoneThreshold;

    private Camera mainCamera;
    private float leftBound = float.MaxValue, rightBound = float.MinValue;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        foreach (Vector2 point in levelBounds.points)
        {
            if (leftBound > point.x)
                leftBound = point.x;
            if (rightBound < point.x)
                rightBound = point.x;
        }

    }

    // Update is called once per frame
    void Update()
    {
        float cameraLeftBound = leftBound +
            (mainCamera.ViewportToWorldPoint(new Vector2(0.5f, 0.5f))).x -
            (mainCamera.ViewportToWorldPoint(new Vector2(0.0f, 0.5f))).x;
        float cameraRightBound = rightBound +
            (mainCamera.ViewportToWorldPoint(new Vector2(0.5f, 0.5f))).x -
            (mainCamera.ViewportToWorldPoint(new Vector2(1.0f, 0.5f))).x;
        if (transform.position.x - deadZoneThreshold > playerBody.transform.position.x)
            transform.position = new Vector3(
                Mathf.Max(playerBody.transform.position.x + deadZoneThreshold, cameraLeftBound),
                transform.position.y,
                transform.position.z);
        if (transform.position.x + deadZoneThreshold < playerBody.transform.position.x)
            transform.position = new Vector3(
                Mathf.Min(playerBody.transform.position.x - deadZoneThreshold, cameraRightBound),
                transform.position.y,
                transform.position.z);
    }
}
