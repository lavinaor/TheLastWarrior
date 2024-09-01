using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    [SerializeField] float Dameg;

    [Header("player info")]
    [SerializeField] GameObject player;
    [HideInInspector] playerCombatController playerCombatController;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.GetComponent<EnemyHealth>() && playerCombatController.isAttackung)
        {
            var enemyHealth = other.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(Dameg);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCombatController = player.GetComponent<playerCombatController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
