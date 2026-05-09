using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class MushroomEnemy : MonoBehaviour
{
    public float walkSpeed = 0f;

    public float walkStopRate = 0.0f;

    public DetectionZone detectionZone;

    public DetectionZone cliffDetectionZone;

    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;
    private Animator playerAnimator;

    public enum WalkableDirection { Right, Left }

    private WalkableDirection _walkDirection = WalkableDirection.Left;
    private Vector2 walkDirectionVector = Vector2.left;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(
                    gameObject.transform.localScale.x * -1,
                    gameObject.transform.localScale.y
                );

                if (value == WalkableDirection.Right)
                    walkDirectionVector = Vector2.right;
                else if (value == WalkableDirection.Left)
                    walkDirectionVector = Vector2.left;
            }
            _walkDirection = value;
        }
    }

    public bool _hasTarget = false;

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool("hasTarget", value);
        }
    }

    public float AttackCooldown
    {
        get { return animator.GetFloat("attackCooldown"); }
        private set { animator.SetFloat("attackCooldown", Mathf.Max(value, 0)); }
    }

    public bool CanMove
    {
        get { return animator.GetBool("canMove"); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();

        GameObject player = GameObject.Find("Player");
        if (player != null)
            playerAnimator = player.GetComponent<Animator>();
    }

    private void Update()
    {
        bool playerIsAttacking = playerAnimator != null && !playerAnimator.GetBool("canMove");
        HasTarget = detectionZone.detectedColliders.Count > 0 && IsPlayerAlive() && !playerIsAttacking;

        if (AttackCooldown > 0)
            AttackCooldown -= Time.deltaTime;
    }

    private bool IsPlayerAlive()
    {
        foreach (Collider2D collider in detectionZone.detectedColliders)
        {
            Damageable damageable = collider.GetComponent<Damageable>();
            if (damageable != null)
                return damageable.IsAlive;
        }
        return false;
    }

    private bool hasFlipped = false;

    private void FixedUpdate()
    {
        if (!hasFlipped && touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
            hasFlipped = true;
        }
        else if (!touchingDirections.IsOnWall || !touchingDirections.IsGrounded)
        {
            hasFlipped = false;
        }

        if (!damageable.LockVelocity)
        {
            if (CanMove)
                rb.linearVelocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
            WalkDirection = WalkableDirection.Left;
        else if (WalkDirection == WalkableDirection.Left)
            WalkDirection = WalkableDirection.Right;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

    public void OnCliffDetected()
    {
        if (touchingDirections.IsGrounded)
            FlipDirection();
    }
}