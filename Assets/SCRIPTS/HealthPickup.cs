using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // how much health this pickup restores when the Player touches it
    public int healthRestore = 20;

    // how fast the food spins - Y 180 means it rotates 180 degrees per second on the Y axis
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    private void OnTriggerEnter2D(Collider2D collision) // runs when something enters the pickup's trigger zone
    {
        // check if whatever entered the trigger has a Damageable component (only the Player should have this)
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable) // if a Damageable was found, heal them and destroy the pickup
        {
            damageable.Heal(healthRestore); // restore health by the amount set in the Inspector
            Destroy(gameObject); // remove the pickup from the scene so it cant be picked up again
        }
    }

    private void Update()
    {
        // spin the food item every frame to make it look more appealing than just sitting still
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}