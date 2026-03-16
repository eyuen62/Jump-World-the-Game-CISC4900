using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    // fires when the last collider leaves the zone (used by CliffDetectionZone to trigger a flip)
    public UnityEvent noCollidersRemain;

    // keeps a list of everything currently inside the detection zone (for example: when the Player touches the detection zone, the list adds the Player)
    public List<Collider2D> detectedColliders = new List<Collider2D>();

    Collider2D col; // reference to the Collider2D component

    private void Awake()
    {
        col = GetComponent<Collider2D>(); // find and store the Collider2D component
    }

    private void OnTriggerEnter2D(Collider2D collision) // runs when something enters the detection zone
    {
        detectedColliders.Add(collision); // add whatever entered the zone to the list
    }

    private void OnTriggerExit2D(Collider2D collision) // runs when something leaves the detection zone
    {
        detectedColliders.Remove(collision); // remove whatever left the zone from the list

        // if the list is now empty, fire the noCollidersRemain event
        if (detectedColliders.Count <= 0)
        {
            noCollidersRemain.Invoke();
        }
    }
}