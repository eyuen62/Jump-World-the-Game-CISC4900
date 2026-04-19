using UnityEngine;
using UnityEngine.InputSystem;

// a safety feature where it checks to make sure to have the Rigidbody2D component, TouchingDirections script, and Damageable script on the Player object
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    // how fast the Player moves on the ground
    public float walkSpeed = 0f;

    // how fast the Player moves in the air
    public float airWalkSpeed = 0f;

    // how high the Player jumps
    public float jumpImpulse = 0f;

    // how fast the Player moves while crouching
    public float crouchWalkSpeed = 0f;

    Vector2 moveInput; // stores which direction the Player wants to move (left or right)

    TouchingDirections touchingDirections; // reference to the TouchingDirections script
    Damageable damageable; // reference to the Damageable component
    CapsuleCollider2D capsuleCollider; // reference to the Capsule Collider so we can resize it when crouching
    Rigidbody2D rb; // reference to the Rigidbody2D component
    Animator animator; // reference to the Animator component

    // stores the original collider size and offset so we can restore them when standing back up
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    public float CurrentMoveSpeed
    {
        get
        {
            // check if the Player is moving, not touching a wall, and allowed to move
            if (IsMoving && !touchingDirections.IsOnWall && animator.GetBool("canMove"))
            {
                if (touchingDirections.IsGrounded) // check if the Player is on the ground
                {
                    if (IsCrouching) // if crouching, use the slower crouch walk speed
                    {
                        return crouchWalkSpeed;
                    }
                    else
                    {
                        return walkSpeed; // on the ground (use walk speed)
                    }
                }
                else
                {
                    return airWalkSpeed; // in the air (use air speed)
                }
            }
            else
            {
                return 0; // standing still or pressed against a wall (no movement/idling)
            }
        }
    }

    [SerializeField]
    private bool _isMoving = false; // stores whether the Player is moving or not

    public bool IsMoving
    {
        get { return _isMoving; } // return the stored value
        private set
        {
            _isMoving = value; // store the new value
            animator.SetBool("isMoving", value); // tell the Animator if the Player is moving or not
        }
    }

    [SerializeField]
    private bool _isFacingRight = true; // stores which way the Player is facing (true = right, false = left)

    public bool IsFacingRight
    {
        get { return _isFacingRight; } // return the stored value
        private set
        {
            if (_isFacingRight != value) // only flip if the direction is actually changing
            {
                // flip the Player by inverting the X scale (for example: 0.6 becomes -0.6)
                transform.localScale = new Vector3(
                    transform.localScale.x * -1,
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
            _isFacingRight = value; // store the new facing direction
        }
    }

    [SerializeField]
    private bool _isCrouching = false; // stores whether the Player is crouching or not

    public bool IsCrouching
    {
        get { return _isCrouching; } // return the stored value
        private set
        {
            _isCrouching = value; // store the new value
            animator.SetBool("isCrouching", value); // tell the Animator if the Player is crouching or not

            if (value) // if crouching, shrink the collider to match the crouch pose
            {
                // reduce the collider height by half and shift it down so the feet stay on the ground
                capsuleCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
                capsuleCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y * 0.25f));
            }
            else // if standing back up (restore the original collider size and position)
            {
                capsuleCollider.size = originalColliderSize;
                capsuleCollider.offset = originalColliderOffset;
            }
        }
    }

    // checks the Animator directly to see if the Player is still alive
    public bool IsAlive
    {
        get { return animator.GetBool("isAlive"); } // returns true if alive, false if dead
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // get the Rigidbody2D component
        animator = GetComponent<Animator>(); // get the Animator component
        touchingDirections = GetComponent<TouchingDirections>(); // get the TouchingDirections component
        damageable = GetComponent<Damageable>(); // get the Damageable component
        capsuleCollider = GetComponent<CapsuleCollider2D>(); // get the Capsule Collider component

        // save the original collider size and offset so we can restore them when standing back up
        originalColliderSize = capsuleCollider.size;
        originalColliderOffset = capsuleCollider.offset;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    private void FixedUpdate() // runs at a fixed rate for physics updates
    {
        if (!damageable.LockVelocity) // only allow movement if the Player is NOT in hit stun
        {
            // move the Player left or right at the correct speed while keeping the current vertical speed
            rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        }

        // send the vertical speed to the Animator so it knows when the Player is rising or falling
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context) // runs when the Player presses A or D key
    {
        moveInput = context.ReadValue<Vector2>(); // get the direction from the input

        if (IsAlive) // only allow movement if the Player is alive
        {
            IsMoving = moveInput != Vector2.zero; // if input is not zero then the Player is moving
            SetFacingDirection(moveInput); // flip the Player to face the direction they are moving
        }
        else
        {
            IsMoving = false; // if dead, force stop all movements
        }
    }

    public void OnJump(InputAction.CallbackContext context) // runs when the Player presses W key or Spacebar
    {
        // only allow jumping if on the ground and not crouching
        if (context.started && touchingDirections.IsGrounded && !IsCrouching)
        {
            animator.SetTrigger("jump"); // tell the Animator to trigger the jump animation
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse); // apply upward force to jump
        }
    }

    public void OnAttack(InputAction.CallbackContext context) // runs when the Player presses Left Mouse Button
    {
        if (context.started && touchingDirections.IsGrounded) // only allow attacking when on the ground
        {
            animator.SetTrigger("attack"); // tell the Animator to trigger the attack animation
        }
    }

    public void OnCrouch(InputAction.CallbackContext context) // runs when the Player presses S key
    {
        if (context.started && IsAlive) // only crouch if alive
        {
            IsCrouching = true; // start crouching
        }
        else if (context.canceled) // S key was released
        {
            IsCrouching = false; // stop crouching
        }
    }

    // called by Damageable when the Player gets hit (applies the knockback force to push them back)
    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y); // push the Player back using the knockback values
    }

    private void SetFacingDirection(Vector2 moveInput) // flips the Player to face whichever direction they are moving
    {
        if (moveInput.x > 0 && !IsFacingRight) // moving right but currently facing left
        {
            IsFacingRight = true; // face right
        }
        else if (moveInput.x < 0 && IsFacingRight) // moving left but currently facing right
        {
            IsFacingRight = false; // face left
        }
    }
}