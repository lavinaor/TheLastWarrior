using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SpinAttackActionPlayer : MonoBehaviour, IAction
{
    [SerializeField] playerCombatController playerCombatController;

    [Header("Spin attack info")]
    [SerializeField] float damage = 10f;       
    [SerializeField] float swordLength = 2f;  
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float spinAttackStaminaCostPerSecond = 10f;
    [SerializeField] float spinAttackMaxDuration = 5f; // Maximum duration if stamina is not depleted
    [SerializeField] bool isTherSpinAttackMaxDuration = true;
    private bool isSpinning = false;
    private Coroutine spinAttackCoroutine;

    [SerializeField] float spinAttackCooldown;
    [SerializeField] private float spinAttackInCooldown = 0f;
    public Image CooldownImageFill;
    public TMP_Text CooldownText;
    [SerializeField] bool cantMoveWhileSpinAttack = true;
    [SerializeField] float elapsedTime = 0f;
    [SerializeField] float spinSpeed = 360f;
    float currentRotation = 0f;
    [SerializeField] int spinsBeforeReset = 2;

    private HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>(); // סט של אויבים שנפגעו

    public void ExecuteAction()
    {
        if (!isSpinning)
        {
            spinAttackCoroutine = StartCoroutine(SpinAttack());
        }
        else
        {
            StopSpinAttack();
        }
    }

    IEnumerator SpinAttack()
    {
        isSpinning = true;
        playerCombatController.playerMovement.stoppSpining = true;
        playerCombatController.isAttackung = true;
        playerCombatController.animator.SetBool("Swordforward", true);
        playerCombatController.animator.Play("Swordforward", 0);
        playerCombatController.animator.Play("Swordforward", 1);

        elapsedTime = 0f;
        int spinCount = 0;

        while (isSpinning && playerCombatController.playerStats.Stamina > spinAttackStaminaCostPerSecond)
        {
            // Reduce stamina
            playerCombatController.playerStats.Stamina -= spinAttackStaminaCostPerSecond * Time.deltaTime;

            // Rotate the player
            float rotationStep = spinSpeed * Time.deltaTime;
            playerCombatController.PlayerObj.transform.Rotate(0f, rotationStep, 0f);
            currentRotation += rotationStep;

            // בדיקה אם השחקן השלים סיבוב שלם
            if (currentRotation >= 360f)
            {
                spinCount++;  // עדכון מספר הסיבובים שהשחקן השלים
                currentRotation = 0f;  // איפוס הסיבוב

                // בדיקה אם השחקן השלים את מספר הסיבובים הנדרש לאיפוס רשימת הנפגעים
                if (spinCount >= spinsBeforeReset)
                {
                    hitEnemies.Clear();  // איפוס רשימת הנפגעים
                    spinCount = 0;       // איפוס מספר הסיבובים
                }
            }

            // Cast Ray from the sword to detect enemies
            RaycastHit hit;
            if (Physics.Raycast(playerCombatController.transform.position + Vector3.up, playerCombatController.PlayerObj.forward, out hit, swordLength, enemyLayer))
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null && !hitEnemies.Contains(enemyHealth))
                {
                    enemyHealth.TakeDamage(damage);
                    hitEnemies.Add(enemyHealth); // הוספת האויב לרשימת הנפגעים
                    Debug.Log("Damage dealt to: " + hit.collider.name);
                }
            }

            // Check if the player has run out of stamina
            if (playerCombatController.playerStats.Stamina < spinAttackStaminaCostPerSecond)
            {
                break;
            }

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= spinAttackMaxDuration && isTherSpinAttackMaxDuration)
            {
                elapsedTime = 0f;
                break;
            }

            yield return null;
        }

        StopSpinAttack();
    }

    void StopSpinAttack()
    {
        if (spinAttackCoroutine != null)
        {
            StopCoroutine(spinAttackCoroutine);
        }

        isSpinning = false;
        playerCombatController.playerMovement.stoppSpining = false;
        playerCombatController.animator.SetBool("Swordforward", false);
        playerCombatController.isAttackung = false;
        //playerCombatController.animator.CrossFade("Idle", .2f); // Transition back to idle or any other state
    }

    // Gizmo to visualize the Raycast
    private void OnDrawGizmos()
    {
        if (playerCombatController.transform.position == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(playerCombatController.transform.position + Vector3.up, playerCombatController.PlayerObj.forward * swordLength);
    }
}
