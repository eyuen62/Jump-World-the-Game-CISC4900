using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // reference to the Slider component that shows the health bar
    public TMP_Text healthBarText; // reference to the TMP text that shows the "100 / 100" numbers

    Damageable playerDamageable; // reference to the Player's Damageable component to read health values

    private void Awake()
    {
        // find my Player GameObject tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");


        // grab the Damageable component off the Player so we can read its health values
        playerDamageable = player.GetComponent<Damageable>();
    }

    void Start()
    {
        // set the slider and text to match the Player's current health when the game starts
        healthSlider.value = CalculateSliderPercentage(playerDamageable.Health, playerDamageable.MaxHealth);
        healthBarText.text = playerDamageable.Health + " / " + playerDamageable.MaxHealth;
    }

    private void OnEnable()
    {
        // start listening to the Player's healthChanged event so the bar updates when health changes
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnDisable()
    {
        // stop listening when this object is disabled so it doesnt try to update a bar that no longer exists
        playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    private float CalculateSliderPercentage(float currentHealth, float maxHealth)
    {
        // health is stored as a whole number but the slider only accepts values between 0 and 1
        // so we divide current health by max health to get the correct percentage (ex: 50/100 = 0.5)
        return currentHealth / maxHealth;
    }

    private void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        // update the slider and text whenever the Player's health changes
        healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
        healthBarText.text = newHealth + " / " + maxHealth;
    }
}