using UnityEngine;
using Unity.Cinemachine;

public class OutOfBoundsKill : MonoBehaviour
{
    // for the CinemachineCamera object
    public CinemachineCamera cinemachineCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // specfically for the Player and no one elses
        if (!collision.CompareTag("Player"))
            return;

        Damageable damageable = collision.GetComponent<Damageable>();

        // if the Player is already dead and still touch the OutOfBounds, then it does nothing (this to prevent double GameOver SFX playing)
        if (damageable == null || !damageable.IsAlive)
            return;

        // cut the camera panning from the Player immediately so it freezes at the last position while Player keeps falling off screen
        if (cinemachineCamera != null)
            cinemachineCamera.Target.TrackingTarget = null;

        // guaranteed death no matter the current health for Player
        // plays the death animation + death SFX + fires the damageableDeath event
            damageable.Health = 0;
    }
}