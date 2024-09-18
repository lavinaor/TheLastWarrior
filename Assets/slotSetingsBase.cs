using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "slotSetingsBase", menuName = "ScriptableObjects/slotSetingsBase")]
public class slotSetingsBase : ScriptableObject
{
    public string Name;
    [SerializeField] KeyCode attackA = KeyCode.Alpha0;
    public float AttackTime = 1f;
    public float AttackCooldown;
    public Image CooldownImageFill;
    public TMP_Text CooldownText;
    public bool cantEnterWhileAttack = true;
    public bool cantMoveWhileAttack = true;
}
