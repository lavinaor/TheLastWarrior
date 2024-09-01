using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ShootAttackIActionEnemy : MonoBehaviour, IAction
{
    [SerializeField] EnemyCombatController enemyCombatController;

    [Header("Shoot attack info")]
    [SerializeField] float shotAttackDamage = 20f;
    [SerializeField] float shotAttackTimeToTrow = 1f;
    [SerializeField] float shotAttackTime = 1f;
    [SerializeField] float shotAttackCooldown = 5f;
    private float shotAttackInCooldown;
    [SerializeField] GameObject shotAttackPrefab;
    [SerializeField] Transform handPosition;
    [SerializeField] float shotAttackForce = 20f;
    [SerializeField] float playerHith;

    private void Update()
    {
        if (shotAttackInCooldown > 0f)
        {
            shotAttackInCooldown -= Time.deltaTime;
        }
    }
    public void ExecuteAction()
    {
        if (shotAttackInCooldown <= 0f && !enemyCombatController.isAttacking)
        {
            Debug.Log("in");
            enemyCombatController.isAttacking = true;
            StartCoroutine(shotAttack());
        }
    }

    IEnumerator shotAttack()
    {
        if (shotAttackInCooldown <= 0f)
        {
            Debug.Log("in");
            enemyCombatController.animator.CrossFade("Attack01", 0.1f);
            enemyCombatController.isAttacking = true;
            yield return new WaitForSeconds(shotAttackTimeToTrow);

            // crates the rock
            GameObject rock = Instantiate(shotAttackPrefab, handPosition.position, handPosition.rotation);
            Rigidbody rb = rock.GetComponent<Rigidbody>();
            rb.velocity = (handPosition.position - (enemyCombatController.player.transform.position + Vector3.up * playerHith)).normalized * -1 * shotAttackForce;

            yield return new WaitForSeconds(shotAttackTime);
            enemyCombatController.isAttacking = false;
            //destroy rock after time
            Destroy(rock, 5f);

            //start culdown
            shotAttackInCooldown = shotAttackCooldown;
        }
    }
}

