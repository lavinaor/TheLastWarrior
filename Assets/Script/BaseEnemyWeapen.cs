using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyWeapen : MonoBehaviour
{
    [SerializeField] float Dameg;
    [SerializeField] float delay;
    private bool inDelay;

    [Header("enemy info")]
    [SerializeField] GameObject enemy;
    [HideInInspector] EnemyCombatController enemyCombatController;

    // Start is called before the first frame update
    void Start()
    {
        enemyCombatController = enemy.GetComponent<EnemyCombatController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.GetComponent<PlayerStats>() && enemyCombatController.isAttacking)
        {
            if (!inDelay)
            {
                var PlayerStats = other.GetComponent<PlayerStats>();
                PlayerStats.TakeDamage(Dameg);
            }
            StartCoroutine(oneShotDelay());
        }
    }

    IEnumerator oneShotDelay()
    {
        inDelay = true;
        yield return new WaitForSeconds(delay);
        inDelay = false;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
