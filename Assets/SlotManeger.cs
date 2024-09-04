using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotManeger : MonoBehaviour
{
    public string Name;
    public float AttackTime = 1f;
    public float AttackCooldown;
    public Image CooldownImageFill;
    public TMP_Text CooldownText;
    public bool canEnterWhileAttack = false;
    public bool cantMoveWhileAttack = true;
    public bool isActive = false;
    public IAction action;

    private float AttackInCooldown;

    private void Start()
    {
        UpdateCooldownUI();
    }

    private void Update()
    {
        if (AttackInCooldown > 0f)
        {
            AttackInCooldown -= Time.deltaTime;
            UpdateCooldownUI();
        }
    }
    public void ExecuteSlotAction()
    {
        if (AttackInCooldown <= 0f && !canEnterWhileAttack)
        {
            action.ExecuteAction();
            Invoke("StartCooldown", AttackTime);
        }
        if (canEnterWhileAttack && isActive)
        {
            action.ExecuteAction();
        }
    }

    private void StartCooldown()
    {
        AttackInCooldown = AttackCooldown + AttackTime;
        UpdateCooldownUI();
    }

    public void UpdateCooldownUI()
    {
        CooldownImageFill.fillAmount = AttackCooldown * 10 - AttackInCooldown * 10;
        if (AttackInCooldown > 0 && AttackCooldown > 1)
            CooldownText.text = AttackInCooldown.ToString("0");
        else
            CooldownText.text = new string(" ");
    }
}
