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

    public void ExecuteAction()
    {

        //problem it cant enter while is attacing


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

        while (isSpinning && playerCombatController.playerStats.Stamina > spinAttackStaminaCostPerSecond)
        {
            // Reduce stamina
            playerCombatController.playerStats.Stamina -= spinAttackStaminaCostPerSecond * Time.deltaTime;

            // Rotate the player
            playerCombatController.PlayerObj.transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);

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
        playerCombatController.animator.Play("Blend Tree 2", 0);
        playerCombatController.animator.Play("Blend Tree 2", 1);
        playerCombatController.isAttackung = false;
        //playerCombatController.animator.CrossFade("Idle", .2f); // Transition back to idle or any other state
    }
}
