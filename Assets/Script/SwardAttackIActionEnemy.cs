using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwardAttackIActionEnemy : MonoBehaviour, IAction
{
    [SerializeField] EnemyCombatController enemyCombatController;

    [Header("sward attack info")]
    [SerializeField] float swardAttackTime = 1f;
    [SerializeField] float swardAttackCooldown;
    [SerializeField] private float swardAttackInCooldown = 0f;
    [SerializeField] AudioClip audioClip;

    private void Update()
    {
        if (swardAttackInCooldown > 0f)
        {
            swardAttackInCooldown -= Time.deltaTime;
        }
    }

    public void ExecuteAction()
    {
        if (swardAttackInCooldown <= 0f)
        {
            StartCoroutine(SwardAttack());
        }
    }

    IEnumerator SwardAttack()
    {
        //start attack
        enemyCombatController.isAttacking = true;
        enemyCombatController.animator.CrossFade("attack", .2f);

        // play sound FX 
        SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform, 0.5f);

        //end attack
        yield return new WaitForSeconds(swardAttackTime);
        enemyCombatController.isAttacking = false;

        //start culdown
        swardAttackInCooldown = swardAttackCooldown;
    }
}
