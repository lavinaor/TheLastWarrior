using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwardAttackIActionPlayer : MonoBehaviour, IAction
{
    [SerializeField] playerCombatController playerCombatController;

    [Header("sward attack info")]
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

        // play sound FX 
        SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform, 0.5f);

        //end attack
        yield return new WaitForSeconds(swardAttackTime);
        playerCombatController.isAttackung = false;
        if (cantMoveWhileSwardAttack)
            playerCombatController.playerMovement.canMove = true;
    }
}
