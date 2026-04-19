using System.Collections.Generic;
using UnityEngine;

public class FlyingEyeEnemy : MonoBehaviour
{
    // how fast it moves between waypoints
    public float flightSpeed = 0f;

    // how close it needs to get before going to the next waypoint
    public float waypointReachedDistance = 0.1f;

    // reference to the AttackDetectionZone child object that detects when the Player is in bite range
    public DetectionZone biteDetectionZone;

    // reference to the DeathCollider child object that enables when it dies
    public Collider2D deathCollider;

    // list of waypoints to fly between
    public List<Transform> waypoints;

    Animator animator; // reference to the Animator component
    Rigidbody2D rb; // reference to the Rigidbody2D component
    Damageable damageable; // reference to the Damageable script
    private Animator playerAnimator; // reference to the Player's Animator to check if the Player is currently attacking

    Transform nextWaypoint; // the waypoint it is currently flying toward
    int waypointNum = 0; // tracks which waypoint in the list is currently next and active

    public bool _hasTarget = false; // stores whether it sees the Player or not

    public bool HasTarget
    {
        get { return _hasTarget; } // return the stored value
        private set
        {
            _hasTarget = value; // store the new value
            animator.SetBool("hasTarget", value); // tell the Animator whether it sees the Player or not
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool("canMove"); // check the Animator to see if it is allowed to move
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>(); // get the Animator component
        rb = GetComponent<Rigidbody2D>(); // get the Rigidbody2D component
        damageable = GetComponent<Damageable>(); // get the Damageable component

        // find the Player in the scene by name and grab their Animator so we can check if they are attacking
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>(); // store the Player's Animator for attack state checking
        }
    }

    private void Start()
    {
        nextWaypoint = waypoints[waypointNum]; // set the first waypoint as the first target when the game starts
    }

    void Update()
    {
        // check if the Player is currently attacking (if so, block it from triggering its own attack)
        bool playerIsAttacking = playerAnimator != null && !playerAnimator.GetBool("canMove");

        // only set HasTarget to true if the Player is in range, alive, and not currently attacking
        HasTarget = biteDetectionZone.detectedColliders.Count > 0 && IsPlayerAlive() && !playerIsAttacking;
    }

    private bool IsPlayerAlive()
    {
        // check everything inside the BiteDetectionZone (if any of them have a Damageable component, check if they are still alive)
        foreach (Collider2D collider in biteDetectionZone.detectedColliders)
        {
            Damageable damageable = collider.GetComponent<Damageable>();
            if (damageable != null)
            {
                return damageable.IsAlive; // return whether the detected target is alive or not
            }
        }
        return false; // if nothing with a Damageable component is found in the BiteDetectionZone, then theres no target
    }

    private void FixedUpdate()
    {
        if (damageable.IsAlive) // only move if it's still alive
        {
            if (CanMove) // only fly if the Animator allows movement
            {
                Flight(); // move towards the next waypoint
            }
            else
            {
                rb.linearVelocity = Vector2.zero; // stop all movement when it cant move (during an attack)
            }
        }
    }

    // fly between waypoints and loop back to the start when the last waypoint is reached
    private void Flight()
    {
        // calculate the direction from where it's at to the next waypoint
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        // calculate the distance between it and its next waypoint
        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        // move towards the next waypoint at flightSpeed
        rb.linearVelocity = directionToWaypoint * flightSpeed;

        // update the facing direction based on which way it's moving
        UpdateDirection();

        if (distance <= waypointReachedDistance) // if close enough to the waypoint, move to the next one
        {
            waypointNum++; // move to the next waypoint

            if (waypointNum >= waypoints.Count)
            {
                waypointNum = 0; // loop back to the first waypoint when the last one is reached
            }

            nextWaypoint = waypoints[waypointNum]; // update the target to the new waypoint
        }
    }

    private void UpdateDirection()
    {
        Vector3 locScale = transform.localScale; // store the current scale so we can flip it if needed

        if (transform.localScale.x > 0) // currently facing right
        {
            if (rb.linearVelocity.x < 0) // if moving left
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z); // flip to face left
            }
        }
        else // currently facing left
        {
            if (rb.linearVelocity.x > 0) // if moving right
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z); // flip to face right
            }
        }
    }

    // called by Damageable script when it dies (re-enables gravity so the body falls to the ground)
    public void OnDeath()
    {
        rb.gravityScale = 2f; // re-enable gravity so the body falls
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // stop horizontal movement but keep vertical so it falls straight down
        deathCollider.enabled = true; // enable the death collider so the body lands on the ground properly
    }
}