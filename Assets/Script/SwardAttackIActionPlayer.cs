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
    [SerializeField] float swardAttackCooldown;
    [SerializeField] private float swardAttackInCooldown = 0f;
    public Image CooldownImageFill;
    public TMP_Text CooldownText;
    [SerializeField] bool cantMoveWhileSwardAttack = true;
    [SerializeField] AudioClip audioClip;

    private void Start()
    {
        UpdateCooldownUI();
    }

    private void Update()
    {
        if (swardAttackInCooldown > 0f)
        {
            swardAttackInCooldown -= Time.deltaTime;
            UpdateCooldownUI();
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

        //start culdown
        swardAttackInCooldown = swardAttackCooldown;
        UpdateCooldownUI();
    }
    public void UpdateCooldownUI()
    {
        CooldownImageFill.fillAmount = swardAttackCooldown * 10 - swardAttackInCooldown * 10;
        if (swardAttackInCooldown > 0 && swardAttackCooldown > 1)
            CooldownText.text = swardAttackInCooldown.ToString("0");
        else
            CooldownText.text = new string(" ");
    }
}
