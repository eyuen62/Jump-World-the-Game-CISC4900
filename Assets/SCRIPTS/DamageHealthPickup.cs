using UnityEngine;

public class DamageHealthPickup : MonoBehaviour
{
    // how much damage this pickup does when the Player touches it
    public int damageCaused = 10;

    // how fast the bad food spins - Y 180 means it rotates 180 degrees per second on the Y axis
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    private void OnTriggerEnter2D(Collider2D collision) // runs when something enters the pickup's trigger zone
    {
        // check if whatever entered the trigger has a Damageable component
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable) // if a Damageable was found, deal damage and destroy the pickup
        {
            // deliver the damage to whatever touched it
            bool gotHit = damageable.Hit(damageCaused, Vector2.zero);

            if (gotHit)
            {
                Destroy(gameObject); // remove the bad food from the scene so it cant damage again
            }
        }
    }

    private void Update()
    {
        // spin the bad food item every frame to make it visually distinct from good food
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}