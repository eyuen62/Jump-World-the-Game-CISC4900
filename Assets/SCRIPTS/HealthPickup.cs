using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // how much health it restores when the Player touches it
    public int healthRestore = 0;

    // how fast it spins (X is forward/backward roll, Y is left/right spin, Z is clockwise spin)
    // set Y to 180 so it spins like a coin
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    // drag the good food pickup sfx here
    public AudioClip pickupSound;

    // volume for the sfx
    public float pickupVolume = 1f;

    private void OnTriggerEnter2D(Collider2D collision) // runs when the Player touches the good food
    {
        // check if whatever touched it has a Damageable component (only the Player should have this, not the Enemies)
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable) // only continue if a Damageable was found
        {
            damageable.Heal(healthRestore); // restore health by the amount set in the Inspector

            if (pickupSound != null) // only play if a sfx is assigned (if not, then it plays nothing)
            {
                AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position, pickupVolume); // play the pickup sfx
            }

            Destroy(gameObject); // remove it from the scene so it can't be picked up again
        }
    }

    private void Update()
    {
        // spin it every frame for looks
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}