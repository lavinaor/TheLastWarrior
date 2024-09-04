using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotManeger : MonoBehaviour
{
    public slotSetingsBase slotSetings;
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
        if (AttackInCooldown <= 0f)
        {
            action.ExecuteAction();
            Invoke("StartCooldown", slotSetings.AttackTime);
        }
    }

    private void StartCooldown()
    {
        AttackInCooldown = slotSetings.AttackCooldown + slotSetings.AttackTime;
        UpdateCooldownUI();
    }

    public void UpdateCooldownUI()
    {
        slotSetings.CooldownImageFill.fillAmount = slotSetings.AttackCooldown * 10 - AttackInCooldown * 10;
        if (AttackInCooldown > 0 && slotSetings.AttackCooldown > 1)
            slotSetings.CooldownText.text = AttackInCooldown.ToString("0");
        else
            slotSetings.CooldownText.text = new string(" ");
    }
}
