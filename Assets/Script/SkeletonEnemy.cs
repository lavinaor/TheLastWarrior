using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonEnemy : MonoBehaviour
{
    [Header("close attack")]
    [SerializeField] float closeAttackDamage = 20f;
    [SerializeField] float closeAttackTime = 1f;
    [SerializeField] float closeAttackCooldown = 5f;
    private bool closeAttackInCooldown;

    //in program enemy informashen
    private Rigidbody rb;
    private Animator animator;
    private EnemyBase enemyBase;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        enemyBase = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        if (!enemyBase.isAttacking && !enemyBase.deade)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                StartCoroutine(CloseAttacking());
            }
        }
    }

    IEnumerator CloseAttacking()
    {
        if (enemyBase.deade) yield break;
        animator.CrossFade("Attack02", 0.1f);
        enemyBase.isAttacking = true;
        yield return new WaitForSeconds(closeAttackTime);
        enemyBase.isAttacking = false;
        enemyBase.SetDestanashn();
        closeAttackInCooldown = true;
        yield return new WaitForSeconds(closeAttackCooldown);
        closeAttackInCooldown = false;
    }
}