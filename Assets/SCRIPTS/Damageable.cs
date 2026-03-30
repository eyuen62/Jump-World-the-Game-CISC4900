using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    Animator animator;

    public UnityEvent<int, Vector2> damageableHit;

    [SerializeField]
    private int _maxHealth = 100;

    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    [SerializeField]
    private int _health = 100;

    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;

            // If health drops to 0 or below, character is no longer alive
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;

    [SerializeField]
    private bool isInvincible = false;

    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    public bool IsAlive
    {
        get { return _isAlive; }
        private set
        {
            _isAlive = value;
            animator.SetBool("isAlive", value);
            Debug.Log("IsAlive set " + value);
        }
    }


    // the velocity should not be changed while this is true
    // but needs to be respected by other physics components like the player controller
    public bool LockVelocity
    {
        get { return animator.GetBool("lockVelocity"); }
        set { animator.SetBool("lockVelocity", value); }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                // Remove invincibility
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }

    }

    // Returns whether the damageable took damage or not
    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;
            animator.SetTrigger("hit"); // tell the Animator to trigger the hit animation

            LockVelocity = true;

            // notify other subscribed components that the damageable was hit to handle the knockback and such
            damageableHit?.Invoke(damage, knockback);

            CharacterEvents.characterDamaged?.Invoke(gameObject, damage);

            return true;
        }

        // unable to be hit
        return false;
    }

    public void Heal(int healthRestore)
    {
        if (IsAlive) // only heal if the character is still alive - no point healing a dead character
        {
            // figure out how much room is left before hitting max health
            // for example if health is 90 and max is 100, there is 10 room left
            // Mathf.Max makes sure this never goes below 0 just in case
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);

            // pick whichever is smaller - the room left or the food's restore value
            // this prevents the health from ever going over the max health cap
            // for example if room left is 10 but food restores 35, actualHeal becomes 10
            int actualHeal = Mathf.Min(maxHeal, healthRestore);

            Health += actualHeal; // add only the capped heal amount to the current health

            // fire the global heal event with the actual healed amount so the green floating text shows the right number
            CharacterEvents.characterHealed?.Invoke(gameObject, actualHeal);
        }
    }
}