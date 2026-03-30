using UnityEngine;
using UnityEngine.Events;

public class CharacterEvents : MonoBehaviour
{
    // a global event that any script can call when a character takes damage
    // passes in the character GameObject and the damage amount so listeners know who got hit and how much
    public static UnityAction<GameObject, int> characterDamaged;

    // a global event that any script can call when a character gets healed
    // passes in the character GameObject and the heal amount so listeners know who got healed and how much
    public static UnityAction<GameObject, int> characterHealed;
}