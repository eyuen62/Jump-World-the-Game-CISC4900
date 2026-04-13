using UnityEngine;

public class DamageHealthPickup : MonoBehaviour
{
    public int damageCaused = 10; // how much damage this pickup does when the Player touches it
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0); // how fast the bad food spins - Y 180 means it rotates 180 degrees per second on the Y axis
    public AudioClip damageSound; // drag bad food damage sound here in the Inspector
    public float damageVolume = 1f; // volume for the damage sound

    private void OnTriggerEnter2D(Collider2D collision) // runs when something enters the pickup's trigger zone
    {
        // check if whatever entered the trigger has a Damageable component
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable) // if a Damageable was found, deal damage and destroy the pickup
        {
            bool gotHit = damageable.Hit(damageCaused, Vector2.zero); // deliver the damage to whatever touched it

            if (gotHit)
            {
                if (damageSound != null) // only play if a clip is actually assigned — prevents null reference error
                {
                    AudioSource.PlayClipAtPoint(damageSound, Camera.main.transform.position, damageVolume); // play the damage sound at the camera position
                }

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