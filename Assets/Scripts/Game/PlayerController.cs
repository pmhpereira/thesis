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

    public enum Dashing
    {
        IDLE,
        BACKWARD,
        FORWARD,
    }

    [HideInInspector]
    public Dashing dashingState = Dashing.IDLE;

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

    private float dashTime;
    private float dashDeltaDistance;
    [Range(0.1f, 3f)]
    public float dashMaxDuration;
    [Range(0f, 1f)]
    public float dashingDuration;
    [Range(0, 12)]
    public float dashingMaxDistance;
    private float dashingCurrentDistance;
    private float oldGravityScale;

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

        if(isSliding && slideTime < slideScalingDuration) // when sliding (scale down), the player has negative vertical velocity
        {
            isJumping = false;
        }

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

        if(isJumping || isSliding || isDashing)
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
            if (Input.GetButton("Fire1") && isIdling)
            {
                SingleJump();
            }
            else if (Input.GetButtonDown("Fire1") && currentConsecutiveJumps < maxConsecutiveJumps)
            {
                MultipleJump();
            }
            else if(Input.GetButton("Fire2") && !isJumping && !isDashing)
            {
                DashSetup();
            }
            else if(Input.GetButton("Fire3") && !isJumping && !isSliding)
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

    void DashSetup()
    {
        if (!TreeManager.instance.IsMechanicEnabled(Tag.Dash))
        {
            return;
        }

        isDashing = true;
        dashTime = 0;
        dashDeltaDistance = 0;
        dashingCurrentDistance = 0;
        oldGravityScale = rigidbody.gravityScale;
    }

    void Slide()
    {
        if(slideTime > slideMaxDuration)
        {
            isSliding = false;
            slideTime = 0;
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

    void Dash()
    {
        rigidbody.gravityScale = oldGravityScale;

        if(dashTime >= dashMaxDuration)
        {
            isDashing = false;
            dashDeltaDistance = -dashingCurrentDistance;
            transform.position -= Vector3.left * dashDeltaDistance;
            dashTime = 0;
            dashingCurrentDistance = 0;
            dashingState = Dashing.IDLE;
        }
        else
        {
            Vector3 newPosition = transform.position;

            if(dashTime < dashingDuration) // dashing forward
            {
                dashDeltaDistance = (dashingMaxDistance / dashingDuration);
                rigidbody.gravityScale = 0;
                dashingState = Dashing.BACKWARD;
            }
            else if(dashTime > dashMaxDuration - dashingDuration) // dashing backward
            {
                dashDeltaDistance = -(dashingMaxDistance / dashingDuration);
                dashingState = Dashing.FORWARD;
            }
            else
            {
                dashingState = Dashing.IDLE;

                if(dashingCurrentDistance > dashingMaxDistance)
                {
                    dashDeltaDistance = dashingMaxDistance - dashingCurrentDistance;
                    dashDeltaDistance /= (dashTime - dashingDuration) / (dashMaxDuration - 2*dashingDuration);
                }
                else
                {
                    dashDeltaDistance = dashingMaxDistance - dashingCurrentDistance;
                    dashDeltaDistance /= (dashTime - dashingDuration) / (dashMaxDuration - 2*dashingDuration);
                }
            }

            dashDeltaDistance *= Time.deltaTime;
            dashingCurrentDistance += dashDeltaDistance;
        }

        dashTime += Time.deltaTime;
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
        else if (isColliding && dashingState == Dashing.IDLE)
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

        if (isDashing)
        {
            Dash();
        }

        if(!GameManager.instance.isPaused)
        {
            Time.timeScale = (((isColliding && dashingState == Dashing.IDLE) || isFalling) && stopOnCollision) ? 0 : 1;

            if(isDashing)
            {
                transform.position += Vector3.right * dashDeltaDistance;
            }
        }

        if (rigidbody.velocity.y > maxVerticalVelocity)
        {
            Vector2 newVelocity = rigidbody.velocity;
            newVelocity.y = maxVerticalVelocity;
            rigidbody.velocity = newVelocity;
        }

        transform.position += new Vector3(Time.deltaTime * moveSpeed, 0, 0);

        PlayerState[] states = ResolveState();
        state = states[states.Length - 1].name;
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

    public PlayerState[] ResolveState()
    {
        List<PlayerState> states = new List<PlayerState>();

        if(isColliding) states.Add(PlayerState.COLLIDING);
        else if(isFalling) states.Add(PlayerState.FALLING);
        else
        {
            if(isMultipleJumping) states.Add(PlayerState.DOUBLE_JUMPING);
            else if(isJumping) states.Add(PlayerState.JUMPING);

            if(isSliding) states.Add(PlayerState.SLIDING);
            if(isDashing)  states.Add(PlayerState.DASHING);
        }

        if(states.Count == 0 || isIdling) states.Add(PlayerState.IDLING);

        return states.ToArray();
    }
}
