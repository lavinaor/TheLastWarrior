using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class IActionShockWaveAttackPlayer : MonoBehaviour, IAction
{
    [SerializeField] playerCombatController playerCombatController;

    [Header("shockWave attack info")]
    [SerializeField] float shockWaveAttackDamage = 1f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float waveRadius = 5f;
    [SerializeField] float delayFromeAnimashenToShockwave = 3.0f;
    [SerializeField] float shockWaveAttackTime = 1f;
    [SerializeField] bool cantMoveWhileShockWaveAttack = true;
    [SerializeField] ParticleSystem[] particleSystem = new ParticleSystem[0];
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public Vector3 explosionOffset = Vector3.zero;

    public void ExecuteAction()
    {
        StartCoroutine(shockWaveAttack());
    }

    IEnumerator shockWaveAttack()
    {
        //start attack
        playerCombatController.isAttackung = true;
        if (cantMoveWhileShockWaveAttack)
            playerCombatController.playerMovement.canMove = false;
        playerCombatController.animator.CrossFade("powerd up", .2f);

        //play particals
        yield return new WaitForSeconds(delayFromeAnimashenToShockwave);
        foreach (var particle in particleSystem)
            particle.Play();

        //do damage
        Collider[] hitEnemy = Physics.OverlapSphere(playerCombatController.transform.position, waveRadius, enemyLayer);
        foreach (Collider enemy in hitEnemy)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(shockWaveAttackDamage);
            }
            else
            {
                Debug.Log("enemyHealth component is null");
            }
        }
        Explode();

        //end attack
        yield return new WaitForSeconds(shockWaveAttackTime);
        playerCombatController.isAttackung = false;
        if (cantMoveWhileShockWaveAttack)
            playerCombatController.playerMovement.canMove = true;
    }

    void Explode()
    {
        // הגדרת המיקום של הפיצוץ
        Vector3 explosionPosition = playerCombatController.transform.position + explosionOffset;

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
                // הוספת כוח פיצוץ על ה-Rigidbody
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerCombatController.transform.position, waveRadius);
    }
}
