using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]

public class BringerEnemy : MonoBehaviour
{
    public float walkSpeed = 3f; // how fast the Enemy moves - 3 is medium speed, lower is slower, higher is faster
    public float walkStopRate = 0.6f; // how quick the Enemy slows down to a stop before attacking
    public DetectionZone attackZone; // reference to the DetectionZone object that detects when the Player is in range
    public DetectionZone cliffDetectionZone;


    Rigidbody2D rb; // reference to the Rigidbody2D component
    TouchingDirections touchingDirections; // reference to TouchingDirections script so we know when the Enemy touches a Wall or the Ground
    Animator animator; // reference to the Animator component
    Damageable damageable;

    public enum WalkableDirection { Right, Left } // Enemy can ONLY be going Right or Left

    private WalkableDirection _walkDirection = WalkableDirection.Left; // stores which direction the Enemy is currently walking (left)
    private Vector2 walkDirectionVector = Vector2.left; // the Enemy movement starts as left to match the sprite's natural direction

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; } // return the stored value
        set
        {
            if (_walkDirection != value) // only does something if the Enemy direction is actually changing (no point in flipping if already going the same way)
            {
                // flip the Enemy sprite (positive scale faces right & negative scale faces left)
                gameObject.transform.localScale = new Vector2(
                    gameObject.transform.localScale.x * -1,
                    gameObject.transform.localScale.y
                );

                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right; // set movement vector to positive X so the Enemy moves right
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left; // set movement vector to negative X so the Enemy moves left
                }
            }
            _walkDirection = value; // stores the new direction
        }
    }

    public bool _hasTarget = false; // stores whether the Enemy currently sees the Player or not

    public bool HasTarget
    {
        get { return _hasTarget; } // return the stored value
        private set
        {
            _hasTarget = value; // store the new value (true or false)
            animator.SetBool("hasTarget", value); // tell the Animator if the Enemy sees the Player or not
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat("attackCooldown");
        }
        private set
        {
            animator.SetFloat("attackCooldown", Mathf.Max(value, 0));
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool("canMove"); // check the Animator to see if the Enemy is allowed to move
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // reference to the Rigidbody2D component
        touchingDirections = GetComponent<TouchingDirections>(); // reference to TouchingDirections script
        animator = GetComponent<Animator>(); // reference to the Animator component
        damageable = GetComponent<Damageable>();

    }

    private void Update()
    {
        // check it's area box if the Player is inside the detection zone
        // if the list has more than 0 colliders then the Enemy has a target (aka the Player)
        HasTarget = attackZone.detectedColliders.Count > 0;

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }

    }

    private bool hasFlipped = false; // prevents the Enemy from rapidly flipping back and forth while still touching the Wall

    private void FixedUpdate()
    {
        if (!hasFlipped && touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection(); // flip only if Enemy touches a wall
            hasFlipped = true; // mark down that the Enemy have flipped so it dont flip again immediately
        }
        else if (!touchingDirections.IsOnWall || !touchingDirections.IsGrounded)
        {
            hasFlipped = false; // reset the flip flag once the Enemy has moved away from the wall
        }

        if (!damageable.LockVelocity) // only move if NOT in hit stun
        {
            if (CanMove)
            {
                rb.linearVelocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            }
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left; // if the Enemy was going right and then flips, they go left
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right; // if the Enemy was going left and then flips, they go right
        }
    }

    // LockVelocity = true is NOT set here — Damageable.Hit() already handles it automatically
    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

    public void OnCliffDetected()
    {
        // only flip if the enemy is on the ground — prevents flipping mid-air
        if (touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }
}