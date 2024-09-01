using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrikeAttackIActionEnemy : MonoBehaviour, IAction
{
    [SerializeField] EnemyCombatController enemyCombatController;

    [SerializeField] float cooldownDuration = 10f;
    [SerializeField] float lightningDelay = 2f;
    [SerializeField] float damageRadius = 3f;
    [SerializeField] float damageAmount = 20f;
    [SerializeField] int numberOfStrikes = 3;
    [SerializeField] float timeBetweenStrikes = 1f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject lightningPrefab;

    private bool isOnCooldown = false;

    public void ExecuteAction()
    {
        if (!enemyCombatController.isAttacking && !isOnCooldown)
        {
            StartCoroutine(LightningStrikeRoutine());
        }
    }

    IEnumerator LightningStrikeRoutine()
    {
        enemyCombatController.isAttacking = true;

        for (int i = 0; i < numberOfStrikes; i++)
        {
            var strikePosition = enemyCombatController.player.transform.position;
            // התחלת הפרטיקל סיסטם של הברק
            GameObject lightning = Instantiate(lightningPrefab, strikePosition, Quaternion.identity);
            yield return new WaitForSeconds(lightningDelay);

            // בדיקת פגיעה והורדת חיים
            Collider[] hitPlayers = Physics.OverlapSphere(strikePosition, damageRadius, playerLayer);
            foreach (Collider player in hitPlayers)
            {
                if (player.GetComponent<PlayerStats>() != null)
                    player.GetComponent<PlayerStats>().TakeDamage(damageAmount);
            }

            Destroy(lightning); // סיום הפרטיקל סיסטם

            yield return new WaitForSeconds(timeBetweenStrikes);
        }

        enemyCombatController.isAttacking = false;

        StartCoroutine(CooldownRoutine());
    }

    IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        float cooldownTimer = cooldownDuration;

        while (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }

        isOnCooldown = false;
    }

/*    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(enemyCombatController.player.transform.position, damageRadius);
    }*/
}
