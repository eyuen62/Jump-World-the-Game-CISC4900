using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    // what layers to check against when casting (Ground layer)
    public ContactFilter2D castFilter;

    // how close the Player needs to be to the ground before it counts as grounded
    public float groundDistance = 0.05f;

    // how close the Player needs to be to a wall before it counts as touching a wall
    public float wallDistance = 0.2f;

    // how close the Player needs to be to a ceiling before it counts as touching a ceiling
    public float ceilingDistance = 0.05f;

    // if checked, flips the wall check direction (used for enemy sprites that faces left)
    public bool invertWallCheck = false;

    CapsuleCollider2D touchingCol; // reference to the Capsule Collider component
    Animator animator; // reference to the Animator

    RaycastHit2D[] groundHits = new RaycastHit2D[5]; // stores up to 5 results when checking the ground
    RaycastHit2D[] wallHits = new RaycastHit2D[5]; // stores up to 5 results when checking the wall
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5]; // stores up to 5 results when checking the ceiling

    // checks which way the character is facing based on the X scale
    // if invertWallCheck is OFF (Player): positive scale = check right, negative scale = check left
    // if invertWallCheck is ON (Enemy): positive scale = check left, negative scale = check right
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0
        ? (invertWallCheck ? Vector2.left : Vector2.right)
        : (invertWallCheck ? Vector2.right : Vector2.left);

    [SerializeField]
    private bool _isGrounded; // stores whether the character is on the ground or not

    public bool IsGrounded
    {
        get { return _isGrounded; } // return the stored value
        private set
        {
            _isGrounded = value; // store the new value
            animator.SetBool("isGrounded", value); // tell the Animator the new grounded state
        }
    }

    [SerializeField]
    private bool _isOnWall; // stores whether the character is touching a wall or not

    public bool IsOnWall
    {
        get { return _isOnWall; } // return the stored value
        private set
        {
            _isOnWall = value; // store the new value
            animator.SetBool("isOnWall", value); // tell the Animator the new wall state
        }
    }

    [SerializeField]
    private bool _isOnCeiling; // stores whether the character is touching a ceiling or not

    public bool IsOnCeiling
    {
        get { return _isOnCeiling; } // return the stored value
        private set
        {
            _isOnCeiling = value; // store the new value
            animator.SetBool("isOnCeiling", value); // tell the Animator the new ceiling state
        }
    }

    private void Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>(); // get the Capsule Collider component
        animator = GetComponent<Animator>(); // get the Animator component
    }

    void FixedUpdate()
    {
        // cast downward (if it hits anything on the Ground layer, the character is grounded)
        IsGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;

        // cast sideways in the facing direction (if it hits anything on the Ground layer, the character is on a wall)
        IsOnWall = touchingCol.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;

        // cast upward (if it hits anything on the Ground layer, the character is touching a ceiling)
        IsOnCeiling = touchingCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
    }
}