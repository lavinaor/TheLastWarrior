
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Boss1 : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float playerHith;

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
    [SerializeField] float startDelay = 2;


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
        StartCoroutine(StartDelay());
    }
    IEnumerator StartDelay()
    {
        JumpAttackInCooldown = true;
        yield return new WaitForSeconds(startDelay);
        startDelay = 0;
        JumpAttackInCooldown = false;
    }

    private void Update()
    {
        //check if ther is ground under it
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 3f, ground);
        if (!enemyBase.isAttacking && !enemyBase.deade)
        {
            if (!JumpAttackInCooldown && agent.remainingDistance <= JumpAttackDistens)
            {
                PerformJumpAttack();
            }
            else
            {
                if (agent.remainingDistance <= agent.stoppingDistance && !closeAttackInCooldown)
                {
                    StartCoroutine(CloseAttacking(closeAttackTime));
                }
                else
                {
                    if (agent.remainingDistance <= maxThrowAttackDistens && !throwAttackInCooldown)
                    {
                        StartCoroutine(ThrowAttacking(throwAttackTime));
                    }
                }
            }
        }
    }

    IEnumerator CloseAttacking(float time)
    {
        animator.CrossFade("Attack02", 0.1f);
        enemyBase.isAttacking = true;
        yield return new WaitForSeconds(time);
        enemyBase.isAttacking = false;
        enemyBase.SetDestanashn();
        closeAttackInCooldown = true;
        yield return new WaitForSeconds(closeAttackCooldown);
        closeAttackInCooldown = false;
    }

    IEnumerator ThrowAttacking(float time)
    {
        throwAttackInCooldown = true;
        animator.CrossFade("Attack01", 0.1f);
        enemyBase.isAttacking = true;
        yield return new WaitForSeconds(time);

        // crates the rock
        GameObject rock = Instantiate(rockPrefab, handPosition.position, handPosition.rotation);
        Rigidbody rb = rock.GetComponent<Rigidbody>();
        rb.velocity = (handPosition.position - (player.transform.position + Vector3.up * playerHith)).normalized * -1 * throwForce;

        //destroy rock after time
        Destroy(rock, 5f);

        enemyBase.isAttacking = false;
        yield return new WaitForSeconds(throwAttackCooldown);
        throwAttackInCooldown = false;
    }

    public void PerformJumpAttack()
    {
        Debug.Log("why");
        if (isGrounded)
        {
            // Disable the NavMeshAgent
            agent.enabled = false;

            // Make the boss jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Trigger jump animation
            animator.CrossFade("jump", 0f);
            enemyBase.isAttacking = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") && enemyBase.isAttacking)
        {

            // Trigger ground hit animation
            animator.CrossFade("GroundHit", 0f);

            StartCoroutine(PlayParticleSystemWithDelay());

            // Re-enable the NavMeshAgent
            agent.enabled = true;

        }
    }
/*    private void OnTriggerEnter(Collision other)
    {
        Debug.Log("ground");
        if (collision.gameObject.CompareTag("Ground") && enemyBase.isAttacking)
        {

            // Trigger ground hit animation
            animator.CrossFade("GroundHit", 0f);

            StartCoroutine(PlayParticleSystemWithDelay());

            // Re-enable the NavMeshAgent
            agent.enabled = true;

        }
    }*/
    IEnumerator PlayParticleSystemWithDelay()
    {
        yield return new WaitForSeconds(delay);

        // Call method to create the energy wave
        CreateEnergyWave();

        yield return new WaitForSeconds(jumpTime);
        enemyBase.isAttacking = false;

        JumpAttackInCooldown = true;
        yield return new WaitForSeconds(JumpAttackCooldown);
        JumpAttackInCooldown = false;
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

        foreach (var particle in particleSystem)
            particle.Play();
        // Visual or particle effect for the energy wave
        // Instantiate(waveEffectPrefab, transform.position, Quaternion.identity);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, waveRadius);
    }
}


