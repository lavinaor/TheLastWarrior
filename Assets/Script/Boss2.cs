
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Boss2 : MonoBehaviour
{
    [Header("Jump attack")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpTime = 1f;
    [SerializeField] float JumpAttackDistens;
    [SerializeField] float waveRadius = 5f;
    [SerializeField] float JumpAttackDamage = 20f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask ground;
    [SerializeField] float delay = 3.0f;
    [SerializeField] float JumpAttackCooldown = 5f;
    private bool JumpAttackInCooldown;
    [SerializeField] ParticleSystem[] particleSystem = new ParticleSystem[0];


    [Header("close attack")]
    [SerializeField] float closeAttackDamage = 20f;
    [SerializeField] float closeAttackTime = 1f;
    [SerializeField] float closeAttackCooldown = 5f;
    private bool closeAttackInCooldown;

    [Header("throw attack")]
    [SerializeField] float maxThrowAttackDistens;
    [SerializeField] float throwAttackDamage = 20f;
    [SerializeField] float throwAttackTime = 1f;
    [SerializeField] float throwAttackCooldown = 5f;
    private bool throwAttackInCooldown;
    [SerializeField] GameObject rockPrefab;
    [SerializeField] Transform handPosition;
    [SerializeField] float throwForce = 20f;

    //in program enemy informashen
    private Rigidbody rb;
    private Animator animator;
    private EnemyBase enemyBase;
    private bool isGrounded;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var particle in particleSystem)
            particle.Stop();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        enemyBase = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>();
        JumpAttackInCooldown = false;
    }

    private void Update()
    {
        //check if ther is ground under it
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 3f, ground);
        if (!enemyBase.isAttacking)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                PerformCloseAttack();
            }
            else
            {
                if (agent.remainingDistance <= maxThrowAttackDistens)
                {
                    PerformThrowAttack();
                }
            } 
        }
    }
    public void PerformCloseAttack()
    {
        StartCoroutine(CloseAttacking(closeAttackTime));
        if (isGrounded)
        {
            /*            // Disable the NavMeshAgent
                        agent.enabled = false;*/

            // Trigger CloseAttack animation
            animator.SetBool("Attack02", true);
        }
    }

    IEnumerator CloseAttacking(float time)
    {
        enemyBase.isAttacking = true;
        yield return new WaitForSeconds(time);
        enemyBase.isAttacking = false;
        animator.SetBool("Attack02", false);
        closeAttackInCooldown = true;
        yield return new WaitForSeconds(closeAttackCooldown);
        closeAttackInCooldown = false;
    }
    public void PerformThrowAttack()
    {
        StartCoroutine(ThrowAttacking(throwAttackTime));
        if (isGrounded)
        {
            /*            // Disable the NavMeshAgent
                        agent.enabled = false;*/

            // Trigger CloseAttack animation
            animator.CrossFade("Attack01", 0.1f);
        }
    }

    IEnumerator ThrowAttacking(float time)
    {
        enemyBase.isAttacking = true;
        yield return new WaitForSeconds(time);
        enemyBase.isAttacking = false;

        // crates the rock
        GameObject rock = Instantiate(rockPrefab, handPosition.position, handPosition.rotation);
        Rigidbody rb = rock.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * throwForce;

        //destroy rock after time
        Destroy(rock, 5f);

        throwAttackInCooldown = true;
        yield return new WaitForSeconds(throwAttackCooldown);
        throwAttackInCooldown = false;
    }

    IEnumerator PlayParticleSystemWithDelay()
    {
        yield return new WaitForSeconds(delay);

        foreach (var particle in particleSystem)
            particle.Play();
        
        // Call method to create the energy wave
        CreateEnergyWave();
    }

    private void CreateEnergyWave()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, waveRadius, playerLayer);

        foreach (Collider player in hitPlayers)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(JumpAttackDamage);
            }
            else
            {
                Debug.Log("PlayerCombat component is null");
            }
        }

        // Visual or particle effect for the energy wave
        // Instantiate(waveEffectPrefab, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, waveRadius);
    }
}

