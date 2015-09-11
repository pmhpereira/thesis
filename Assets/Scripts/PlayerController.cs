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

    [Range(5, 10)]
    public float maxVerticalVelocity;

    [Range(0, 1000)]
    public float jumpForce;
    public int currentConsecutiveJumps;
    public int maxConsecutiveJumps;

    private new Rigidbody2D rigidbody;
    private new Renderer renderer;
    public List<Collider2D> colliding;

    public Color idlingColor;
    public Color collidingColor;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<Renderer>();

        colliding = new List<Collider2D>();
    }

    void Update ()
    {
        CheckStatus();
        UpdateVisuals();
        PlayerMovement();
    }

    void CheckStatus()
    {
        isColliding = false;
        isIdling = false;
        isJumping = Mathf.Abs(rigidbody.velocity.y) > Mathf.Epsilon * 1e3;

        foreach (Collider2D collider in colliding)
        {
            if(collider == null)
            {
                continue;
            }

            if (collider.tag == "Obstacle")
            {
                isColliding = true;
            }
            else if(collider.tag == "Ground")
            {
                isIdling = true;
            }
        }

        if (isIdling && !isJumping)
        {
            currentConsecutiveJumps = 0;
            isMultipleJumping = false;
        }
    }

    void UpdateVisuals()
    {
        renderer.material.color = isColliding ? collidingColor : idlingColor;
    }

    void PlayerMovement()
    {
        if (!isColliding)
        {
            if (Input.GetButton("Fire1") && !isJumping)
            {
                Jump();
            }
            else if (Input.GetButtonDown("Fire1") && currentConsecutiveJumps < maxConsecutiveJumps)
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

    void Jump()
    {
        if (isJumping && !isMultipleJumping)
        {
            isMultipleJumping = true;
        }

        rigidbody.AddForce(new Vector2(0, jumpForce));
        currentConsecutiveJumps++;
        isJumping = true;
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
}
