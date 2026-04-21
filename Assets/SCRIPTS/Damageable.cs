using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    Animator animator; // reference to the Animator component

    // fires when the character (either the Player or the Enemies) takes damage (passes the damage amount done and knockback direction)
    public UnityEvent<int, Vector2> damageableHit;

    // fires when the character dies
    public UnityEvent damageableDeath;


    // fires whenever the Player's health changes — HealthBar listens to this to update the slider and text
    public UnityEvent<int, int> healthChanged;

    [SerializeField]
    private int _maxHealth = 100; // the maximum health the character can have

    public int MaxHealth
    {
        get { return _maxHealth; } // return the max health value
        set { _maxHealth = value; } // set the max health value
    }

    [SerializeField]
    private int _health = 100; // the current health of the character

    public int Health
    {
        get { return _health; } // return the current health value
        set
        {
            _health = Mathf.Clamp(value, 0, _maxHealth); // store the new health value (make sure it never goes below 0 or above max health)


            // tell HealthBar that the health just changed which pass the new health and max health
            healthChanged?.Invoke(_health, MaxHealth);

            // if health drops to 0 or below, the character is dead
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true; // stores whether the character is alive or not

    [SerializeField]
    private bool isInvincible = false; // stores whether the character is currently invincible or not (after getting hit)

    private float timeSinceHit = 0; // tracks how long it has been since the character was last hit
    public float invincibilityTime = 0.25f; // how long the character stays invincible after getting hit

    public bool IsAlive
    {
        get { return _isAlive; } // return whether the character is alive or not
        private set
        {
            _isAlive = value; // store the new value
            animator.SetBool("isAlive", value); // tell the Animator whether the character is alive or not

            if (value == false)
            {
                damageableDeath.Invoke(); // fires the death event
            }
        }
    }

    // if true, the character's velocity is frozen a bit during a hit stun
    public bool LockVelocity
    {
        get { return animator.GetBool("lockVelocity"); } // get the lockVelocity bool from the Animator
        set { animator.SetBool("lockVelocity", value); } // set the lockVelocity bool in the Animator
    }

    private void Awake()
    {
        animator = GetComponent<Animator>(); // get the Animator component
    }

    private void Update()
    {
        if (isInvincible) // only run if the character is currently invincible
        {
            if (timeSinceHit > invincibilityTime) // if the invincibility window has passed
            {
                isInvincible = false; // remove invincibility
                timeSinceHit = 0; // reset the timer
            }

            timeSinceHit += Time.deltaTime; // count the timer since the last hit
        }
    }

    // called when something tries to deal damage (returns true if the hit was successful, false if not)
    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible) // only take damage if alive and not currently invincible (or dead)
        {
            Health -= damage; // reduce health by the damage amount
            isInvincible = true; // make the character temporarily invincible so they cant be hit again immediately
            animator.SetTrigger("hit"); // tell the Animator to play the hit animation

            LockVelocity = true; // freeze the character's velocity during hit stun (stops them from moving for a bit)

            // fires the damageableHit event (this triggers OnHit on whatever character was hit to apply the knockback)
            damageableHit?.Invoke(damage, knockback);

            // fires the global damage event so the UIManager script can show the floating damage text
            CharacterEvents.characterDamaged?.Invoke(gameObject, damage);

            return true; // hit was successful
        }

        return false; // hit failed (either dead or invincible)
    }

    public void Heal(int healthRestore)
    {
        if (IsAlive) // only heal if the character is still alive
        {
            // figure out how much health is left before hitting max health
            // Mathf.Max makes sure health never goes below 0 (meaning once the health drops all the way, 0 is the only number shown)
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);

            // prevents health from ever going over the max health cap
            int actualHeal = Mathf.Min(maxHeal, healthRestore);

            Health += actualHeal; // add the capped heal amount to the current health

            // fires the global heal event so the UIManager script can show the floating heal text
            CharacterEvents.characterHealed?.Invoke(gameObject, actualHeal);
        }
    }
}