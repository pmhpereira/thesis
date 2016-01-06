using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private bool isJumping;
    private bool isMultipleJumping;
    private bool isDashing;
    private bool isSliding;
    private bool isColliding;
    private bool isIdling;
    private bool isFalling;

    public float moveSpeed;

    public string state;

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

    private float slideTime;
    [Range(0.1f, 3f)]
    public float slideMaxDuration;
    [Range(0f, 1f)]
    public float slideScalingDuration;

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
        isJumping = Mathf.Abs(rigidbody.velocity.y) > 1e-3;
        isJumping = isJumping && !(slideTime < slideScalingDuration); // when sliding (scale down), the player has negative vertical velocity

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
        if(GameManager.instance.isPaused)
        {
            return;
        }

        if (!isColliding && !isFalling)
        {
            if (Input.GetButton("Fire1") && isIdling && !isJumping)
            {
                SingleJump();
            }
            else if (Input.GetButtonDown("Fire1") && currentConsecutiveJumps < maxConsecutiveJumps)
            {
                MultipleJump();
            }
            else if(Input.GetButton("Fire2") && isIdling && !isSliding)
            {
                SlideSetup();
            }
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            stopOnCollision = !stopOnCollision;
        }
    }

    void SingleJump()
    {
        if(!TreeManager.instance.IsMechanicEnabled(Tag.Jump))
        {
            return;
        }

        Jump();
    }

    void MultipleJump()
    {
        if (!TreeManager.instance.IsMechanicEnabled(Tag.Double_Jump))
        {
            return;
        }

        Jump();
    }
    
    void SlideSetup()
    {
        if (!TreeManager.instance.IsMechanicEnabled(Tag.Slide))
        {
            return;
        }

        isSliding = true;
        slideTime = 0;
    }

    void Slide()
    {
        if(slideTime > slideMaxDuration)
        {
            isSliding = false;
        }
        else
        {
            Vector3 newScale = transform.localScale;
            float newHeight = transform.localScale.y;

            if(slideTime < slideScalingDuration) // scaling down
            {
                newHeight = Mathf.Lerp(1, 0.5f, slideTime / slideScalingDuration);
            }
            else if(slideTime > slideMaxDuration - slideScalingDuration) // scaling up
            {
                newHeight = Mathf.Lerp(0.5f, 1, (slideTime - slideMaxDuration + slideScalingDuration) / slideScalingDuration);
            }

            newScale.x = 1 / newHeight;
            newScale.y = newHeight;
            transform.localScale = newScale;
        }

        slideTime += Time.deltaTime;
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

        if(isSliding)
        {
            Slide();
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

        state = ResolveState().name;
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
        if(isSliding) return PlayerState.SLIDING;
        if(isIdling) return PlayerState.IDLING;

        return PlayerState.IDLING;
    }
}
