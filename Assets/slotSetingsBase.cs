using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "slotSetingsBase", menuName = "custom/slotSetingsBase")]
public class slotSetingsBase : ScriptableObject
{
    public string Name;
    [SerializeField] float Cooldown;
    public Image CooldownImageFill;
    public TMP_Text CooldownText;
    [SerializeField] bool canMoveWhileAttack = true;
}
