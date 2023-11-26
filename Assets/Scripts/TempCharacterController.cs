using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCharacterController : MonoBehaviour
{
    public Rigidbody2D characterBody;
    public float moveSpeed;
    public float jumpForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            characterBody.velocity = new Vector2(-moveSpeed, characterBody.velocity.y);
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            characterBody.velocity = new Vector2(moveSpeed, characterBody.velocity.y);
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            characterBody.AddForce(new Vector2(0, jumpForce));
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
