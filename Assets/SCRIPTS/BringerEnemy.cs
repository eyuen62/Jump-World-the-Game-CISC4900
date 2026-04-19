using UnityEngine;

// A safety feature where it checks to make sure to have the Rigidbody2D component, TouchingDirections script, and Damageable script
// on the Bringer Enemy object
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class BringerEnemy : MonoBehaviour
{
    // how fast it moves
    public float walkSpeed = 0f;

    // how quick it slows down to a stop before attacking
    public float walkStopRate = 0.0f;

    // reference to the DetectionZone child that detects when the Player is in attack range (so it prepares to attack)
    public DetectionZone attackZone;

    // reference to the DetectionZone child that detects when it's near a cliff edge (so it flips its direction)
    public DetectionZone cliffDetectionZone;

    Rigidbody2D rb; // reference to the Rigidbody2D component
    TouchingDirections touchingDirections; // reference to the TouchingDirections script
    Animator animator; // reference to the Animator component
    Damageable damageable; // reference to the Damageable component
    private Animator playerAnimator; // reference to the Player's Animator to check if the Player is currently attacking

    public enum WalkableDirection { Right, Left } // can only walk Right or Left

    private WalkableDirection _walkDirection = WalkableDirection.Left; // starts walking left to match the facing direction
    private Vector2 walkDirectionVector = Vector2.left; // the movement vector (starts as left)

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; } // return the current walk direction
        set
        {
            if (_walkDirection != value) // only flip if the direction is actually changing
            {
                // flip direction by inverting the X scale (positive faces right & negative faces left)
                gameObject.transform.localScale = new Vector2(
                    gameObject.transform.localScale.x * -1,
                    gameObject.transform.localScale.y
                );

                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right; // move right
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left; // move left
                }
            }
            _walkDirection = value; // store the new direction
        }
    }

    public bool _hasTarget = false; // stores whether it sees the Player or not

    public bool HasTarget
    {
        get { return _hasTarget; } // return the stored value
        private set
        {
            _hasTarget = value; // store the new value
            animator.SetBool("hasTarget", value); // tell the Animator whether the Bringer sees the Player or not
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat("attackCooldown"); // get the current attack cooldown value from the Animator
        }
        private set
        {
            animator.SetFloat("attackCooldown", Mathf.Max(value, 0)); // set the cooldown but never let it go below 0
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool("canMove"); // check the Animator to see if it's allowed to move
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // get the Rigidbody2D component
        touchingDirections = GetComponent<TouchingDirections>(); // get the TouchingDirections component
        animator = GetComponent<Animator>(); // get the Animator component
        damageable = GetComponent<Damageable>(); // get the Damageable component

        // find the Player in the scene by name and grab their Animator so it can check if the Player is attacking
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>(); // store the Player's Animator for attack state checking
        }
    }

    private void Update()
    {
        // check if the Player is currently attacking (if so, it blocks the Bringer from triggering its own attack)
        bool playerIsAttacking = playerAnimator != null && !playerAnimator.GetBool("canMove");

        // only set HasTarget to true if the Player is in range, alive, and not currently attacking
        HasTarget = attackZone.detectedColliders.Count > 0 && IsPlayerAlive() && !playerIsAttacking;

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime; // count down the attack cooldown
        }
    }

    private bool IsPlayerAlive()
    {
        // check everything inside the attack zone (if any of them have a Damageable component, check if they are still alive)
        foreach (Collider2D collider in attackZone.detectedColliders)
        {
            Damageable damageable = collider.GetComponent<Damageable>();
            if (damageable != null)
            {
                return damageable.IsAlive; // return whether the detected target is alive or not
            }
        }
        return false; // if no Damageable component found in the zone, then there is no target
    }

    private bool hasFlipped = false; // prevents it from rapidly flipping back and forth while touching a wall

    private void FixedUpdate()
    {
        if (!hasFlipped && touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection(); // flip only when touching a wall
            hasFlipped = true; // mark the flip every time it happened so it doesnt flip again immediately
        }
        else if (!touchingDirections.IsOnWall || !touchingDirections.IsGrounded)
        {
            hasFlipped = false; // reset the flip flag once moved away from the wall
        }

        if (!damageable.LockVelocity) // only move if NOT in hit stun
        {
            if (CanMove)
            {
                // move left or right at walkSpeed while keeping the current velocity unchanged
                rb.linearVelocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.linearVelocity.y);
            }
            else
            {
                // smoothly slow down to a stop using Lerp when it cant move (for example during an attack)
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            }
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left; // was going right, now go left
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right; // was going left, now go right
        }
    }

    // called by Damageable component when the Bringer gets hit (applies the knockback force to push him back)
    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y); // push the Bringer back using the knockback values
    }

    // cliff detection so it makes sure it doesn't fall off the edge
    public void OnCliffDetected()
    {
        if (touchingDirections.IsGrounded) // only flip if on the ground
        {
            FlipDirection();
        }
    }
}