using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter; // this is to check what layers to look for (either the Ground or the Wall)
    public float groundDistance = 0.05f; // this is to check how close or far the Player is from the Ground (0.05 is enough to detect contact)
    public float wallDistance = 0.2f; // this is to check how close or far the Player is from the Wall (0.2 is bigger than groundDistance because Walls need more range to detect properly)
    public float ceilingDistance = 0.05f; // this is to check how close or far the Player is from the Ceiling (0.05 is  enough to detect contact same as groundDistance)
    public bool invertWallCheck = false; // if checked, flips the wall check direction (used for Enemy sprites that faces left originally)


    CapsuleCollider2D touchingCol; // this will hold the Player's Capsule Collider so we can use it to cast and check directions
    Animator animator; // this will hold the Animator so we can update animation states when Grounded/Wall/Ceiling changes

    RaycastHit2D[] groundHits = new RaycastHit2D[5]; // stores up to 5 results when checking the Ground
    RaycastHit2D[] wallHits = new RaycastHit2D[5]; // stores up to 5 results when checking the Wall
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5]; // stores up to 5 results when checking the Ceiling

    // checks which way the character is facing based on scale X direction
    // if invertWallCheck is OFF (Player): positive scale = check right, negative scale = check left
    // if invertWallCheck is ON (Enemy): positive scale = check left, negative scale = check right
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0
        ? (invertWallCheck ? Vector2.left : Vector2.right)
        : (invertWallCheck ? Vector2.right : Vector2.left);

    [SerializeField] // this lets us see _isGrounded in the Inspector while playing so we can watch it update in real time (useful for debugging)
    private bool _isGrounded; // stored value for whether the Player is on the Ground or not

    public bool IsGrounded
    {
        get { return _isGrounded; } // when something asks for the IsGrounded property, it gives them the stored _isGrounded value
        private set
        {
            _isGrounded = value; // store the new value (true or false)
            animator.SetBool("isGrounded", value); // immediately tell the Animator the new Grounded state so the animations update
        }
    }

    [SerializeField]
    private bool _isOnWall; // stored value for whether the Player is touching a Wall or not

    public bool IsOnWall
    {
        get { return _isOnWall; } // when something asks for the IsOnWall property, it gives them the stored _isOnWall value
        private set
        {
            _isOnWall = value;
            animator.SetBool("isOnWall", value); // immediately tell the Animator the new Wall state so the animations update
        }
    }

    [SerializeField]
    private bool _isOnCeiling; // stored value for whether the Player is touching a Ceiling or not

    public bool IsOnCeiling
    {
        get { return _isOnCeiling; } // when something asks for the IsOnCeiling property, it gives them the stored _isOnCeiling value
        private set
        {
            _isOnCeiling = value;
            animator.SetBool("isOnCeiling", value); // immediately tell the Animator the new Ceiling state so the animations update
        }
    }

    private void Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>(); // find and store the Capsule Collider on the Player
        animator = GetComponent<Animator>(); // find and store the Animator on the Player
    }

    void FixedUpdate()
    {
        // shoot the Collider downward by groundDistance and if it hits anything on the Ground layer then IsGrounded becomes true, otherwise false
        IsGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;

        // shoot the Collider sideways in the direction the Player is facing and if it hits anything on the Ground layer then IsOnWall becomes true, otherwise false
        IsOnWall = touchingCol.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;

        // shoot the Collider upward by ceilingDistance and if it hits anything on the Ground layer then IsOnCeiling becomes true, otherwise false
        IsOnCeiling = touchingCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
    }
}