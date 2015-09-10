using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public bool isJumping;
    public bool isMultipleJumping;
    public bool isDashing;
    public bool isSliding;
    public bool isColliding;
    public bool isIdling;

    [Range (-10, 10)]
    public float fixedPlayerPositionX;

    [Range(1, 5)]
    public float gravityMultiplier;

    [Range(5, 10)]
    public float maxVerticalVelocity;

    [Range(0, 1000)]
    public float jumpForce;
    public int maxConsecutiveJumps;
    private int currentConsecutiveJumps = 0;

    private new Rigidbody2D rigidbody;
    private new BoxCollider2D collider;

    public List<Collider2D> colliding;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        colliding = new List<Collider2D>();
    }

    void Update () {
        PlayerMovement();
	}

    void FixedUpdate()
    {
        rigidbody.AddForce(Physics2D.gravity * (gravityMultiplier - 1));

        isColliding = false;
        isIdling = false;

        foreach (Collider2D collider in colliding)
        {
            if(collider.tag == "Obstacle")
            {
                isColliding = true;
            }
            else if(collider.tag == "Ground")
            {
                isIdling = true;
            }
        }

        if(isColliding)
        {
            isIdling = false;
        }

        if (isIdling)
        {
            currentConsecutiveJumps = 0;
            isJumping = false;
            isMultipleJumping = false;
        }
    }

    void PlayerMovement()
    {
        if (!isColliding)
        {
            if (Input.GetButtonDown("Fire1") && currentConsecutiveJumps < maxConsecutiveJumps)
            {
                Jump();
            }
            else if (Input.GetButton("Fire1") && !isJumping)
            {
                Jump();
            }
        }

        var newPosition = transform.position;
        newPosition.x = fixedPlayerPositionX;
        transform.position = newPosition;

        if (rigidbody.velocity.y > maxVerticalVelocity)
        {
            Vector2 newVelocity = rigidbody.velocity;
            newVelocity.y = maxVerticalVelocity;
            rigidbody.velocity = newVelocity;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        colliding.Add(other.collider);
    }

    void OnCollisionExit2D(Collision2D other)
    {
        colliding.Remove(other.collider);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        colliding.Add(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        colliding.Remove(other);
    }

    void Jump()
    {
        if (!isJumping)
        {
            isJumping = true;
        }
        else if (!isMultipleJumping)
        {
            isMultipleJumping = true;
        }

        rigidbody.AddForce(new Vector2(0, jumpForce));
        currentConsecutiveJumps++;
        isJumping = true;
    }
}
