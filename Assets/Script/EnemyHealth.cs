using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] EnemyCombatController enemyCombatController;

    [Header("health")]
    public float health;
    public bool invincible = false;
    [SerializeField] float hitTime;
    [SerializeField] float deathTime;
    public bool deade = false;
    private float maxHealth;
    [SerializeField] bool alwaysShowHealthBar;
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBarFill;
    [SerializeField] Image healthBarFillDelayed;
    [SerializeField] float healthBarFillDelayedDelayNumber;
    [SerializeField] TMP_Text healthText;
    [HideInInspector] Animator animator;
    private NavMeshAgent agent;
    [SerializeField] AudioClip deathAudioClip;
    [SerializeField] AudioClip damegAudioClip;

    [SerializeField] bool hasEventWhenDeade = false;
    [SerializeField] UnityEvent eventWhenDeade;

    [SerializeField] bool hasParticalEfect = false;
    public GameObject particlePrefab;
    public float particleLifetime = 2.0f;
    [SerializeField] Transform particalPosishen;

    // Start is called before the first frame update
    void Start()
    {
        enemyCombatController = GetComponent<EnemyCombatController>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        maxHealth = health;
        StartCoroutine(UpdateHealthUI());
    }

    private void OnValidate()
    {
        if (agent != null) agent = GetComponent<NavMeshAgent>();
        if (animator != null) animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        if (!invincible)
        {
            health -= amount;
            SpawnParticleEffect();
        }
        if (health <= 0f)
        {
            StartCoroutine(Die());
        }
        if (!deade && !invincible)
        {
            animator.CrossFade("hit", 3f);

            // play sound FX 
            SoundFXManager.Instance.PlaySoundFXClip(damegAudioClip, transform, 0.5f);
        }
        StartCoroutine(UpdateHealthUI());
    }

    public void SpawnParticleEffect()
    {
        if (particlePrefab != null && hasParticalEfect)
        {
            GameObject spawnedParticles = Instantiate(particlePrefab, particalPosishen.position, Quaternion.identity);

            spawnedParticles.transform.SetParent(this.transform);

            Destroy(spawnedParticles, particleLifetime);
        }
    }

    IEnumerator UpdateHealthUI()
    {
        if (health < maxHealth && health > 0f || alwaysShowHealthBar)
            healthBar.gameObject.SetActive(true);
        else
            healthBar.gameObject.SetActive(false);

        if (invincible)
        {
            healthBarFill.color = Color.green;
        }
        else
        {
            healthBarFill.color = Color.white;
        }

        healthBarFill.fillAmount = health / maxHealth;
        healthText.text = health.ToString("0");

        yield return new WaitForSeconds(healthBarFillDelayedDelayNumber);
        healthBarFillDelayed.fillAmount = health / maxHealth;
    }

    IEnumerator Die()
    {
        if (health <= 0f)
        {
            if (!deade)
                animator.CrossFade("death", 3f);
            agent.enabled = false;
            deade = true;
            if (hasEventWhenDeade)
                eventWhenDeade.Invoke();

            //remuves colider
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            if (capsuleCollider != null)
                capsuleCollider.enabled = false;

            // play sound FX 
            SoundFXManager.Instance.PlaySoundFXClip(deathAudioClip, transform, 0.5f);

            yield return new WaitForSeconds(deathTime);

            Destroy(this.gameObject);
        }
    }
}
