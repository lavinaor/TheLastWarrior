using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] float regenTime = 1f;
    [SerializeField] Animator animator;

    [Header("player Stamina")]
    public float Stamina;
    [SerializeField] float StaminaRegenRate = 1f;
    private float maxStamina;
    public Image StaminaBarFill;
    public TMP_Text StaminaText;

    [Header("player health")]
    [SerializeField] float health;
    private float maxHealth;
    public Image healthBarFill;
    public TMP_Text healthText;
    [SerializeField] float healthRegenRate = 1f;
    [SerializeField] UIManager uiManager;

    [Header("player healing Potions")]
    [SerializeField] float healingPotions;
    [SerializeField] float boostedRegenTime;
    [SerializeField] float boostedRegenRate = 2f;
    [SerializeField] bool isBoostedRegenActive = false;
    public TMP_Text healingPotionsText;
    [SerializeField] KeyCode healingKey = KeyCode.Q;
    [SerializeField] float healingAmount;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        maxStamina = Stamina;
        StartCoroutine(regenAndStamina());
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(healingKey))
        {
            TakehHealingPotion(healingAmount);
        }
    }

    IEnumerator regenAndStamina()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenTime);
            float regenAmount = isBoostedRegenActive ? boostedRegenRate : healthRegenRate;
            if (health < (maxHealth - regenAmount))
                health += regenAmount;

            if (Stamina < (maxStamina - StaminaRegenRate))
                Stamina += StaminaRegenRate;
            UpdateHealthUI();
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        animator.CrossFade("hit", .2f);
        if (health <= 0f)
        {
            Die();
        }
        UpdateHealthUI();
    }
    private void Die()
    {
        // Handle player death
        animator.CrossFade("death", .2f);
        Debug.Log("Player Died");
        uiManager.ChangeToDeath();
    }
    public void TakehHealingPotion(float amount)
    {
        if (healingPotions > 0 && health < maxHealth)
        {
            healingPotions -= 1;
            health += amount;
            animator.CrossFade("healing", .2f);
            StartCoroutine(boostedRegen());
        }
        if (health <= 0f)
        {
            Die();
        }
        UpdateHealthUI();
    }

    public void AddHealthPotion(float amount)
    {
        healingPotions += amount;
        UpdateHealthUI();
    }

    IEnumerator boostedRegen()
    {
        isBoostedRegenActive = true;
        yield return new WaitForSeconds(boostedRegenTime);
        isBoostedRegenActive = false;
    }


    public void UpdateHealthUI()
    {
        healthBarFill.fillAmount = health / maxHealth;
        healthText.text = health.ToString("0");

        StaminaBarFill.fillAmount = Stamina / maxStamina;
        StaminaText.text = Stamina.ToString("0");

        healingPotionsText.text = healingPotions.ToString("0");
    }
}
