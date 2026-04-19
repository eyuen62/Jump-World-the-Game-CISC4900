using UnityEngine;

public class DamageHealthPickup : MonoBehaviour
{
    // how much damage the bad food does when the Player touches it
    public int damageCaused = 0;

    // how hard the bad food pushes the Player back in format of X,Y (X is left/right & Y is up/down)
    public Vector2 knockback = new Vector2(0f, 0f);

    // how fast the bad food spins (X is forward/backward roll, Y is left/right spin, Z is clockwise spin)
    // set Y to 180 so it spins like a coin on a table at 180 degrees per second
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    // drag the bad food damage sfx here
    public AudioClip damageSound;

    // volume for the damage sfx
    public float damageVolume = 1f;

    private void OnTriggerEnter2D(Collider2D collision) // runs when the Player touches the bad food
    {
        // check if whatever touched it has a Damageable component (only the Player should have this, not the Enemies)
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable) // only continue if a Damageable was found
        {
            // check which way the Player is facing based on their X scale
            // if facing right, flip the knockback X to push them left
            // if facing left, keep the knockback X to push them right
            Vector2 deliveredKnockback = collision.transform.localScale.x > 0
                ? new Vector2(-knockback.x, knockback.y) // facing right (push left)
                : new Vector2(knockback.x, knockback.y); // facing left (push right)

            bool gotHit = damageable.Hit(damageCaused, deliveredKnockback); // deliver the damage and knockback to the Player

            if (gotHit)
            {
                if (damageSound != null) // only play if a sfx is assigned (if not, then it plays nothing)
                {
                    AudioSource.PlayClipAtPoint(damageSound, Camera.main.transform.position, damageVolume); // plays the damage sfx
                }

                Destroy(gameObject); // remove the bad food from the scene (and hierarchy) so it can't do damage again
            }
        }
    }

    private void Update()
    {
        // spin the bad food every frame to make it visually stand out from the good food
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}