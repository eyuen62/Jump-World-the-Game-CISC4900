using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f; // how fast/slow the Player moves on the Ground
    public float airWalkSpeed = 3f; // how fast/slow the Player moves in the Air
    public float jumpImpulse = 10f; // how high the Player jumps
    public float crouchWalkSpeed = 2f; // how fast/slow the Player moves while crouching

    Vector2 moveInput; // stores which direction the Player wants to move (left or right)

    TouchingDirections touchingDirections; // reference to the TouchingDirections script to know if the Player is on the Ground, Wall, or Ceiling
    Damageable damageable; // reference to the Damageable component
    CapsuleCollider2D capsuleCollider; // reference to the Capsule Collider so we can resize it when crouching

    // stores the original collider values so we can restore them when standing back up
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    public float CurrentMoveSpeed
    {
        get
        {
            // check if the Player is moving, not touching a Wall, and allowed to move
            if (IsMoving && !touchingDirections.IsOnWall && animator.GetBool("canMove"))
            {
                if (touchingDirections.IsGrounded) // check if the Player is on the Ground
                {
                    if (IsCrouching) // if crouching, use the slower crouch walk speed
                    {
                        return crouchWalkSpeed;
                    }
                    else
                    {
                        return walkSpeed; // Player on the Ground = walk speed
                    }
                }
                else
                {
                    return airWalkSpeed; // Player in the Air = air speed
                }
            }
            else
            {
                return 0; // this is either standing still or pressed against a Wall = no movement
            }
        }
    }

    // tracks if the Player is moving (true) or standing still (false)
    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving
    {
        get { return _isMoving; } // when something asks for the IsMoving property, it gives them the stored _isMoving value
        private set
        {
            _isMoving = value; // store the new value (true or false)
            animator.SetBool("isMoving", value); // tell the Animator if the Player is moving or not
        }
    }

    // tracks which way the Player is facing (true = right, false = left)
    [SerializeField]
    private bool _isFacingRight = true;

    public bool IsFacingRight
    {
        get { return _isFacingRight; } // when something asks for the IsFacingRight property, it gives them the stored _isFacingRight value
        private set
        {
            if (_isFacingRight != value) // only flip if the direction is actually changing
            {
                // flip the Player by inverting the X scale (for example, 0.6 becomes -0.6)
                transform.localScale = new Vector3(
                    transform.localScale.x * -1,
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
            _isFacingRight = value; // store the new facing direction
        }
    }

    // tracks whether the Player is currently crouching or not
    [SerializeField]
    private bool _isCrouching = false;

    public bool IsCrouching
    {
        get { return _isCrouching; } // return the stored value
        private set
        {
            _isCrouching = value; // store the new value (true or false)
            animator.SetBool("isCrouching", value); // tell the Animator if the Player is crouching or not

            if (value) // if crouching, shrink the collider to match the crouch pose
            {
                // reduce the collider height by half and shift it down so the feet stay on the ground
                capsuleCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
                capsuleCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y * 0.25f));
            }
            else // if standing back up, restore the original collider size and position
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

    Rigidbody2D rb; // reference to the Rigidbody2D component
    Animator animator; // reference to the Animator component

    private void Awake() // runs once when the game starts, before everything else
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
        if (!damageable.LockVelocity) // only allow movement if the Player is NOT in a hit stun
        {
            // move the Player left or right at the correct speed but keep the current up and down speed
            rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        }

        animator.SetFloat("yVelocity", rb.linearVelocity.y); // send the vertical speed to the Animator so it knows when the Player is rising or falling
    }

    public void OnMove(InputAction.CallbackContext context) // this is for the Player's movement keys (A or D key)
    {
        moveInput = context.ReadValue<Vector2>(); // get the direction from the input (either A or D key)

        if (IsAlive) // only allow movement and facing direction changes if the Player is alive
        {
            IsMoving = moveInput != Vector2.zero; // check if the Player is moving. if input is NOT zero then the Player IS moving. if it is zero then the Player ISN'T moving
            SetFacingDirection(moveInput); // flip the Player to face the direction they are moving
        }
        else
        {
            IsMoving = false; // if the Player is dead, force them to stop moving
        }
    }

    public void OnJump(InputAction.CallbackContext context) // this is for the Player's jump key (Spacebar or W key)
    {
        // only allow jumping if the Player is on the Ground AND not crouching
        if (context.started && touchingDirections.IsGrounded && !IsCrouching)
        {
            animator.SetTrigger("jump"); // tell the Animator to trigger the Jump animation
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse); // apply force to make the Player jump
        }
    }

    public void OnAttack(InputAction.CallbackContext context) // this is for the Player's attack key (left mouse button)
    {
        if (context.started && touchingDirections.IsGrounded) // only allow attacking when on the Ground
        {
            animator.SetTrigger("attack"); // tell the Animator to trigger the Attack animation
        }
    }

    public void OnCrouch(InputAction.CallbackContext context) // this is for the Player's crouch key (S key)
    {
        if (context.started && IsAlive) // only crouch if alive and on the Ground
        {
            IsCrouching = true; // player pressed S - start crouching
        }
        else if (context.canceled) // S key was released
        {
            IsCrouching = false; // stop crouching and return to normal
        }
    }

    public void OnHit(int damage, Vector2 knockback) // called by the Damageable UnityEvent when the Player gets hit
    {
        // apply knockback force to push the Player away from the attacker
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

    private void SetFacingDirection(Vector2 moveInput) // flips the Player to face whichever direction they are moving
    {
        if (moveInput.x > 0 && !IsFacingRight) // if Player moving right but currently facing left
        {
            IsFacingRight = true; // result = face right
        }
        else if (moveInput.x < 0 && IsFacingRight) // if Player is moving left but currently facing right
        {
            IsFacingRight = false; // result = face left
        }
    }
}