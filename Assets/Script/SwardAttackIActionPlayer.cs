using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwardAttackIActionPlayer : MonoBehaviour, IAction
{
    [SerializeField] playerCombatController playerCombatController;

    [Header("sward attack info")]
    [SerializeField] float damage;
    [SerializeField] float damageRadius = 5f;
    [SerializeField] float coneAngle = 45f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float swardAttackTime = 1f;
    [SerializeField] bool cantMoveWhileSwardAttack = true;
    [SerializeField] AudioClip audioClip;

    public void ExecuteAction()
    {
        StartCoroutine(SwardAttack());
    }

    IEnumerator SwardAttack()
    {
        //start attack
        playerCombatController.isAttackung = true;
        if (cantMoveWhileSwardAttack)
            playerCombatController.playerMovement.canMove = false;
        playerCombatController.animator.CrossFade("attack", .01f);
        DealDamageToEnemiesInCone();

        // play sound FX 
        SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform, 0.5f);

        //end attack
        yield return new WaitForSeconds(swardAttackTime);
        playerCombatController.isAttackung = false;
        if (cantMoveWhileSwardAttack)
            playerCombatController.playerMovement.canMove = true;
    }

    void DealDamageToEnemiesInCone()
    {
        // מוצא את כל הקוליידרים ברדיוס עם שכבת האויבים
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + Vector3.up, damageRadius, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy != null)
            {
                Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
                float angleToEnemy = Vector3.Angle(playerCombatController.PlayerObj.transform.forward, directionToEnemy);

                // אם האויב נמצא בזווית המשולש מול השחקן
                if (angleToEnemy <= coneAngle / 2f)
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        Debug.Log("dameg  " + enemy);
                        enemyHealth.TakeDamage(damage);
                        Debug.Log("Damage dealt to: " + enemy.name);
                    }
                }
            }
        }
    }

    // Gizmo to visualize the attack area
    private void OnDrawGizmos()
    {
        if (playerCombatController == null) return;

        // קו קדמי של המתקפה
        Vector3 attackDirection = playerCombatController.PlayerObj.transform.forward;
        Vector3 startPosition = playerCombatController.transform.position;

        // ציור קווים המייצגים את קצוות הקונוס
        Vector3 rightBound = Quaternion.Euler(0, coneAngle / 2f, 0) * attackDirection * damageRadius;
        Vector3 leftBound = Quaternion.Euler(0, -coneAngle / 2f, 0) * attackDirection * damageRadius;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(startPosition, rightBound);
        Gizmos.DrawRay(startPosition, leftBound);
    }
}
