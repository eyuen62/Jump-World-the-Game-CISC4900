using UnityEngine;

public class GoalZone : MonoBehaviour
{
    // for the VictoryController object
    public VictoryController victoryController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // only triggers for the Player and no one else
        if (!collision.CompareTag("Player"))
            return;

        Damageable damageable = collision.GetComponent<Damageable>();

        // in case if Player is already dead when they touch the goal/finish line, then it ignore it (death wins over victory)
        if (damageable == null || !damageable.IsAlive)
            return;

        // if Player is alive and touched the goal (shows the victory screen)
        victoryController.ShowVictory();
    }
}