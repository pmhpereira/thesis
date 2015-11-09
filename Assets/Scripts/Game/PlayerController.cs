using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public bool isJumping;
    public bool isMultipleJumping;
    public bool isDashing;
    public bool isSliding;
    public bool isColliding;
    public bool isIdling;
    public bool isFalling;

    public float moveSpeed;

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
    private BoxCollider2D boxCollider;

    public List<Collider2D> colliding;
    private int numCollisions = 0;

    public Color idlingColor;
    public Color collidingColor;
    public Color fallingColor;

    private float collisionDelta = 0.05f;

    public bool stopOnCollision;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<Renderer>();

        boxCollider = GetComponent<BoxCollider2D>();

        colliding = new List<Collider2D>();
    }

    void Update()
    {
        CheckStatus();
        ProcessInput();
        UpdateGame();
    }

    void CheckStatus()
    {
        isColliding = (numCollisions > 0);
        isIdling = false;
        isJumping = Mathf.Abs(rigidbody.velocity.y) > Mathf.Epsilon * 1e3;

        colliding.Clear();
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, transform.localScale + Vector3.one * collisionDelta, 0, Vector2.zero);

        foreach(RaycastHit2D hit in hits)
        {
            Collider2D collider = hit.collider;

            if (collider == null || collider.tag == "Untagged")
            {
                continue;
            }

            colliding.Add(collider);

            if(collider.tag == "Ground")
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
        else if(isJumping && rigidbody.velocity.y < 0 && currentConsecutiveJumps == 0)
        {
            currentConsecutiveJumps = 1;
        }
    }

    void ProcessInput()
    {
        if (!isColliding)
        {
            if (Input.GetButton("Fire1") && isIdling && !isJumping)
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

        if(!GameManager.instance.isPaused)
        {
            Time.timeScale = ((isColliding || isFalling) && stopOnCollision) ? 0 : 1;
        }

        if (rigidbody.velocity.y > maxVerticalVelocity)
        {
            Vector2 newVelocity = rigidbody.velocity;
            newVelocity.y = maxVerticalVelocity;
            rigidbody.velocity = newVelocity;
        }

        transform.position += new Vector3(Time.deltaTime * moveSpeed, 0, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            if (other.transform.parent.tag == "Hole")
            {
                other.gameObject.GetComponentInParent<PatternController>().OnTriggerEnter2D(this.boxCollider);
                isFalling = true;
            }
            else if (other.transform.parent.tag == "Pattern")
            {
                other.gameObject.GetComponentInParent<PatternController>().OnTriggerEnter2D(this.boxCollider);
                numCollisions++;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            if (other.transform.parent.tag == "Pattern")
            {
                numCollisions--;
            }
        }
    }

    public PlayerState ResolveState()
    {
        if(isColliding) return PlayerState.COLLIDING;
        if(isFalling) return PlayerState.FALLING;
        if(isMultipleJumping) return PlayerState.DOUBLE_JUMPING;
        if(isJumping) return PlayerState.JUMPING;
        if(isIdling) return PlayerState.IDLING;

        return PlayerState.IDLING;
    }
}
