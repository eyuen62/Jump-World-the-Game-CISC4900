using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f; // how fast/slow the Player moves on the Ground
    public float airWalkSpeed = 3f; // how fast/slow the Player moves in the Air
    public float jumpImpulse = 10f; // how high the Player jumps

    Vector2 moveInput; // stores which direction the Player wants to move (left or right)

    TouchingDirections touchingDirections; // reference to the TouchingDirections script to know if the Player is on the Ground, Wall, or Ceiling

    Damageable damageable;

    // if the Player is moving and not on a Wall, it checks if they are on the Ground or in the Air
    // if the Player is on the Ground it returns walkSpeed, if in the Air it returns airWalkSpeed
    // if the Player is not moving or is against a Wall, it returns 0 (which is no movement involved)
    public float CurrentMoveSpeed
    {
        get
        {
            // check if the Player is moving, not touching a Wall, and allowed to move
            if (IsMoving && !touchingDirections.IsOnWall && animator.GetBool("canMove"))
            {
                if (touchingDirections.IsGrounded) // check if the Player is on the Ground
                {
                    return walkSpeed; // Player on the Ground = walk speed
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
    // this is needed so TouchingDirections code knows which direction to check for Walls
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
        damageable = GetComponent<Damageable>();

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
        // TODO: add alive check here later
        if (context.started && touchingDirections.IsGrounded) // only allow jumping when the Player is on the Ground
        {
            animator.SetTrigger("jump"); // tell the Animator to trigger the Jump animation
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse); // apply force to make the Player jump
        }
    }

    public void OnAttack(InputAction.CallbackContext context) // this is for the Player's attack key (left mouse button)
    {
        if (context.started) // only trigger once when the button is first pressed
        {
            animator.SetTrigger("attack"); // tell the Animator to trigger the Attack animation
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