using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    // fires when nothing is left inside the zone (for example: the CliffDetectionZone uses this to flip the Bringer when it reaches a cliff edge)
    public UnityEvent noCollidersRemain;

    // keeps a list of everything currently inside the detection zone
    // for example: when the Player enters the zone, the Player gets added to this list
    public List<Collider2D> detectedColliders = new List<Collider2D>();

    Collider2D col; // reference to the Collider2D component

    private void Awake()
    {
        col = GetComponent<Collider2D>(); // get the Collider2D component
    }

    private void OnTriggerEnter2D(Collider2D collision) // runs when something enters the detection zone
    {
        detectedColliders.Add(collision); // add whatever entered the zone to the list
    }

    private void OnTriggerExit2D(Collider2D collision) // runs when something leaves the detection zone
    {
        detectedColliders.Remove(collision); // remove whatever left the zone from the list

        if (detectedColliders.Count <= 0) // if the list is now empty
        {
            noCollidersRemain.Invoke(); // nothing left in the zone (triggers the Bringer to flip direction)
        }
    }
}