using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private CapsuleCollider2D playerCollider;
    private ProgressManager progressManager;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
        progressManager = GameObject.FindAnyObjectByType<ProgressManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider == playerCollider)
            progressManager.LoadNextLevel();
    }
}
