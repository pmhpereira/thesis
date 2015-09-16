using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class PlayerController : MonoBehaviour {

    public bool isJumping;
    public bool isMultipleJumping;
    public bool isDashing;
    public bool isSliding;
    public bool isColliding;
    public bool isIdling;
    public bool isFalling;

    [Range (-10, 10)]
    public float fixedPlayerPositionX;

    [Range(5, 10)]
    public float maxVerticalVelocity;

    [Range(0, 1000)]
    public float jumpForce;
    private int currentConsecutiveJumps;
    public int maxConsecutiveJumps;

    private new Rigidbody2D rigidbody;
    private new Renderer renderer;

    public List<Collider2D> colliding;

    public Color idlingColor;
    public Color collidingColor;
    public Color fallingColor;

    private float collisionDelta = 0.05f;

    public bool stopOnCollision;

    private float defaultTimeScale;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<Renderer>();

        colliding = new List<Collider2D>();

        defaultTimeScale = Time.timeScale;
    }

    void Update ()
    {
        CheckStatus();
        ProcessInput();
        UpdateGame();
    }

    void CheckStatus()
    {
        isColliding = false;
        isIdling = false;
        isJumping = Mathf.Abs(rigidbody.velocity.y) > Mathf.Epsilon * 1e3;

        colliding.Clear();
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, transform.localScale + new Vector3(collisionDelta, collisionDelta, collisionDelta), 0, Vector2.zero);

        foreach(RaycastHit2D hit in hits)
        {
            Collider2D collider = hit.collider;

            if (collider == null || collider.tag == "Untagged")
            {
                continue;
            }

            colliding.Add(collider);

            if (collider.tag == "Obstacle")
            {
                isColliding = true;
            }
            else if(collider.tag == "Ground")
            {
                isIdling = true;
            }
        }

        if(isJumping && isColliding)
        {
            isIdling = false;
        }
        else if (isIdling && !isJumping)
        {
            currentConsecutiveJumps = 0;
            isMultipleJumping = false;
        }
    }

    void ProcessInput()
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

        if(Input.GetKeyDown(KeyCode.C))
        {
            stopOnCollision = !stopOnCollision;
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


    void UpdateGame()
    {
        if (isFalling)
        {
            renderer.material.color = fallingColor;
        }
        else if (isColliding)
        {
            renderer.material.color = collidingColor;
        }
        else
        {
            renderer.material.color = idlingColor;
        }

        Time.timeScale = (isColliding && stopOnCollision) ? 0 : defaultTimeScale;

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Obstacle" && other.transform.parent.tag == "Ground")
        {
            isFalling = true;
        }
    }
}
