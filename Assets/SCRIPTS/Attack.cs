using UnityEngine;

public class Attack : MonoBehaviour
{
    // how much damage the attack deals
    public int attackDamage = 0;

    // how hard and in which direction the hit target gets pushed back in X,Y format (X is left/right & Y is up/down)
    public Vector2 knockback = new Vector2(0, 0);

    private void OnTriggerEnter2D(Collider2D collision) // runs when the Polygon/Capsule Collider 2D touches something
    {
        // check if whatever was hit has a Damageable component (if not, then it ignores it)
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null) // only continue if the thing hit can actually take damage
        {
            // if the parent object is facing left (negative X scale), then it flips the knockback X so it pushes in the correct direction
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback); // deliver the damage and knockback to the target

            if (gotHit)
                Debug.Log(collision.name + " hit for " + attackDamage); // for debugging for the console to show it hit something
        }
    }
}