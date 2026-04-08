using System.Collections.Generic;
using UnityEngine;

public class FlyingEyeEnemy : MonoBehaviour
{
    public float flightSpeed = 2f; // how fast the Flying Eye moves between waypoints
    public float waypointReachedDistance = 0.1f; // how close the Flying Eye needs to get before switching to the next waypoint
    public DetectionZone biteDetectionZone; // reference to the DetectionZone child that detects when the Player is in bite range
    public Collider2D deathCollider; // reference to the DeathCollider child that enables when the Flying Eye dies
    public List<Transform> waypoints; // the list of waypoints the Flying Eye will fly between

    Animator animator; // reference to the Animator component
    Rigidbody2D rb; // reference to the Rigidbody2D component
    Damageable damageable; // reference to the Damageable component

    Transform nextWaypoint; // the waypoint the Flying Eye is currently flying toward
    int waypointNum = 0; // tracks which waypoint in the list is currently active

    public bool _hasTarget = false; // stores whether the Flying Eye currently sees the Player or not

    public bool HasTarget
    {
        get { return _hasTarget; } // return the stored value
        private set
        {
            _hasTarget = value; // store the new value (true or false)
            animator.SetBool("hasTarget", value); // tell the Animator if the Flying Eye sees the Player or not
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool("canMove"); // check the Animator to see if the Flying Eye is allowed to move
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>(); // find and store the Animator component
        rb = GetComponent<Rigidbody2D>(); // find and store the Rigidbody2D component
        damageable = GetComponent<Damageable>(); // find and store the Damageable component
    }

    private void Start()
    {
        nextWaypoint = waypoints[waypointNum]; // set the first waypoint as the target when the game starts
    }

    void Update()
    {
        // check the bite detection zone and only set HasTarget to true if the Player is inside AND still alive
        // this prevents the Flying Eye from continuing to attack after the Player dies
        HasTarget = biteDetectionZone.detectedColliders.Count > 0 && IsPlayerAlive();
    }

    private bool IsPlayerAlive()
    {
        // go through everything currently inside the bite detection zone
        // if any of them have a Damageable component, check if they are still alive
        foreach (Collider2D collider in biteDetectionZone.detectedColliders)
        {
            Damageable damageable = collider.GetComponent<Damageable>();
            if (damageable != null)
            {
                return damageable.IsAlive; // return whether the detected target is alive or not
            }
        }
        return false; // nothing with a Damageable found in the zone so treat it as no alive target
    }

    private void FixedUpdate()
    {
        if (damageable.IsAlive) // only move if the Flying Eye is still alive
        {
            if (CanMove) // only fly if the Animator allows movement
            {
                Flight(); // move toward the next waypoint
            }
            else
            {
                rb.linearVelocity = Vector2.zero; // stop all movement when not allowed to move (during attack)
            }
        }
    }

    // fly between waypoints and loop back to start when final waypoint is reached
    private void Flight()
    {
        // calculate the direction from the Flying Eye to the next waypoint and normalize it to get a unit vector
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        // calculate the actual distance between the Flying Eye and the next waypoint
        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        // move the Flying Eye toward the next waypoint at flightSpeed
        rb.linearVelocity = directionToWaypoint * flightSpeed;

        // update the facing direction based on which way the Flying Eye is moving
        UpdateDirection();

        // check if the Flying Eye is close enough to the waypoint to switch to the next one
        if (distance <= waypointReachedDistance)
        {
            // switch to the next waypoint
            waypointNum++;

            if (waypointNum >= waypoints.Count)
            {
                // loop back to the first waypoint when the last one is reached
                waypointNum = 0;
            }

            nextWaypoint = waypoints[waypointNum]; // update the target to the new waypoint
        }
    }

    private void UpdateDirection()
    {
        Vector3 locScale = transform.localScale; // store the current scale so we can flip it if needed

        if (transform.localScale.x > 0) // currently facing right
        {
            if (rb.linearVelocity.x < 0) // moving left
            {
                // flip to face left
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else // currently facing left
        {
            if (rb.linearVelocity.x > 0) // moving right
            {
                // flip to face right
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
    }

    public void OnDeath()
    {
        // dead Flying Eye falls to the ground
        rb.gravityScale = 2f; // re-enable gravity so the body falls
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // stop horizontal movement but keep vertical so it falls straight down
        deathCollider.enabled = true; // enable the death collider so the body lands on the ground properly
    }
}