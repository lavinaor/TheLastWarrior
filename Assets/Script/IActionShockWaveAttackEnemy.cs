using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class IActionShockWaveAttackEnemy : MonoBehaviour, IAction
{
    [SerializeField] EnemyCombatController enemyCombatController;

    [Header("shock Wave Attack")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpTime = 1f;
    [SerializeField] float waveRadius = 5f;
    [SerializeField] float shockWaveAttackDamage = 20f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask ground;
    [SerializeField] float delaytosound = 3.0f;
    [SerializeField] float delaytoefect = 3.0f;
    [SerializeField] float shockWaveAttackCooldown = 5f;
    private float shockWaveAttackInCooldown;
    [SerializeField] ParticleSystem[] particleSystem = new ParticleSystem[0];
    [SerializeField] float startDelay = 2;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public Vector3 explosionOffset = Vector3.zero;
    [SerializeField] AudioClip audioClip;

    private bool isGrounded;

    void Start()
    {
        foreach (var particle in particleSystem)
            particle.Stop();
        shockWaveAttackInCooldown = startDelay;
    }

    private void Update()
    {
        if (shockWaveAttackInCooldown > 0f)
        {
            shockWaveAttackInCooldown -= Time.deltaTime;
        }
        //check if ther is ground under it
        isGrounded = Physics.Raycast(enemyCombatController.transform.position, Vector3.down, 3f, ground);
    }

    public void ExecuteAction()
    {
        if (shockWaveAttackInCooldown <= 0f)
        {
            Debug.Log("as enterd");
            enemyCombatController.isAttacking = true;
            StartCoroutine(shockWaveAttack());
        }
    }

    IEnumerator shockWaveAttack()
    {
        if (isGrounded && shockWaveAttackInCooldown <= 0f)
        {
            enemyCombatController.isAttacking = true;

            // Trigger ground hit animation
            enemyCombatController.animator.CrossFade("GroundHit", 0f);

            // Re-enable the NavMeshAgent
            enemyCombatController.agent.enabled = true;

            yield return new WaitForSeconds(delaytosound);

            // play sound FX 
            SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform, 0.5f);

            yield return new WaitForSeconds(delaytoefect);

            // Call method to create the energy wave
            CreateEnergyWave();

            yield return new WaitForSeconds(jumpTime);
            enemyCombatController.isAttacking = false;

            shockWaveAttackInCooldown = shockWaveAttackCooldown;
        }
    }

    private void CreateEnergyWave()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, waveRadius, playerLayer);

        foreach (Collider player in hitPlayers)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(shockWaveAttackDamage);
            }
            else
            {
                Debug.Log("PlayerCombat component is null");
            }
        }

        foreach (var particle in particleSystem)
            particle.Play();
        // Visual or particle effect for the energy wave
        // Instantiate(waveEffectPrefab, transform.position, Quaternion.identity);
    }

    void Explode()
    {
        // הגדרת המיקום של הפיצוץ
        Vector3 explosionPosition = enemyCombatController.transform.position + explosionOffset;

        // קבלת כל הקוליידרים באזור הפיצוץ
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (Collider hit in colliders)
        {

            if (hit.CompareTag("Player") || hit.CompareTag("sword") || hit.gameObject == this.gameObject || hit.transform.IsChildOf(this.gameObject.transform))
                continue;

            // בדיקה אם יש לאובייקט רכיב Rigidbody
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                Debug.Log(hit.gameObject.name);
                // הוספת כוח פיצוץ על ה-Rigidbody
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemyCombatController.transform.position, waveRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(enemyCombatController.transform.position, enemyCombatController.transform.position - (Vector3.up * 3f));
    }
}
