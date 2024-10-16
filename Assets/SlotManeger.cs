using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotManeger : MonoBehaviour
{
    [SerializeField] playerCombatController playerCombatController;

    public string Name;
    public KeyCode slotKeybind = KeyCode.Alpha0;
    public float AttackTime = 1f;
    public float AttackCooldown;
    public Image CooldownImageFill;
    public TMP_Text CooldownText;
    public bool canEnterWhileAttack = false;
    public bool isActive = false;
    public GameObject actionObject;
    public IAction action;

    private float AttackInCooldown;

    private void Start()
    {
        UpdateCooldownUI();
        action = actionObject.GetComponent<IAction>();
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
        Debug.Log("enterd " + Name);
        if (AttackInCooldown <= 0f && playerCombatController.isAttackung == false)
        {
            action.ExecuteAction();
            AttackInCooldown = AttackTime;
            CooldownImageFill.color = new Color(0.75f, 0.0f, 1.0f, 1.0f);
            UpdateCooldownUI();
            isActive = true;
            Invoke("StartCooldown", AttackTime);
        }
        else
        {
            if (canEnterWhileAttack && isActive)
            {
                isActive = false;
                action.ExecuteAction();
            }
        }
    }

    private void StartCooldown()
    {
        AttackInCooldown = AttackCooldown;
        isActive = false;
        UpdateCooldownUI();
    }

    public void UpdateCooldownUI()
    {
        CooldownImageFill.color = Color.white;
        CooldownImageFill.fillAmount = (AttackCooldown - AttackInCooldown) / AttackCooldown;
        if (AttackInCooldown > 0 && AttackInCooldown > 1)
        {
            CooldownText.text = AttackInCooldown.ToString("0");
        }
        else if (AttackInCooldown > 0 && AttackInCooldown < 1)
        {
            CooldownText.text = AttackInCooldown.ToString("F1");
        }
        else
        {
            CooldownText.text = new string(" ");
        }
    }
}
