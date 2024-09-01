using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;


public class EnemyBase : MonoBehaviour
{
    [Header("health")]
    [SerializeField] float health;
    [SerializeField] float hitTime;
    [SerializeField] float deathTime;
    public bool deade = false;
    private float maxHealth;
    [SerializeField] Image healthBarFill;
    [SerializeField] TMP_Text healthText;


    public bool hasLineOfSight = false;

    [Header("player info")]
    [SerializeField] GameObject player;
    [HideInInspector] PlayerCombat PlayerCombat;
    [SerializeField] float visionRange = 10f;

    [Header("Movement")]
    [SerializeField] NavMeshAgent agent;
    [HideInInspector] Animator animator;   
    [SerializeField] bool isOfMash;
    [SerializeField] float timeJumping = .7f;
    public bool isAttacking = false;

    private void Start()
    {
        PlayerCombat = player.GetComponent<PlayerCombat>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        maxHealth = health;
        UpdateHealthUI();
    }

    private void OnValidate()
    {
        if (agent != null) agent = GetComponent<NavMeshAgent>();
        if (animator != null) animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            StartCoroutine(Die());
        }
        if (!deade)
            animator.CrossFade("hit", 3f);
        UpdateHealthUI();
    }
    private void UpdateHealthUI()
    {
        healthBarFill.fillAmount = health / maxHealth;
        healthText.text = health.ToString("0");
    }

    IEnumerator Die()
    {
/*        yield return new WaitForSeconds(hitTime);
*/
        if (health <= 0f)
        {
            if (!deade)
                animator.CrossFade("death", 3f);
            agent.enabled = false;
            deade = true;

            yield return new WaitForSeconds(deathTime);

            Destroy(this.gameObject);
        }
    }

    public static bool CheckLineOfSight(Vector3 startPosition, Vector3 targetPosition, float visionRange)
    {
        Vector3 directionToTarget = targetPosition - startPosition;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget <= visionRange)
        {
            Ray ray = new Ray(startPosition, directionToTarget);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, visionRange))
            {
                if (hit.transform.position == targetPosition)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasLineOfSight)
        {
            isAttacking = true;
            if (CheckLineOfSight(transform.position + Vector3.up, player.transform.position, visionRange))
            {
                Debug.Log("as line of sith " + this.name);
                hasLineOfSight = true;
                isAttacking = false;
                agent.SetDestination(player.transform.position);
            }
        }
        if (agent.hasPath && !isAttacking && hasLineOfSight)
        {
            if (agent.isOnOffMeshLink)
            {
                if (!isOfMash)
                {
                    isOfMash = true;
                    var link = agent.currentOffMeshLinkData;
                    StartCoroutine(DoOfMashLink(link));
                }
            }
            else
            {
                isOfMash = false;
                var dir = (agent.steeringTarget - transform.position).normalized;
                var animDir = transform.InverseTransformDirection(dir);
                var isFacingMoveDirection = Vector3.Dot(dir, transform.forward) > .5f;

/*                animator.SetFloat("Horizontal", agent.velocity.magnitude, .5f, Time.deltaTime);
                Debug.Log(agent.velocity.magnitude);*/

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 180 * Time.deltaTime);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.ResetPath();
                }
            }
            
        }
        if (!isAttacking && agent.enabled && hasLineOfSight)
        {
            agent.SetDestination(player.transform.position);
        }
        animator.SetFloat("Horizontal", agent.velocity.magnitude, .5f, Time.deltaTime);
    }
    public void SetDestanashn()
    {
        if(!deade && hasLineOfSight)
            agent.SetDestination(player.transform.position);
    }

    IEnumerator DoOfMashLink(OffMeshLinkData link)
    {
        if (!deade)
            animator.CrossFade("jump", 3f);
        var time = timeJumping;
        var totalTime = time;

        while (time > 0)
        {
            time = Mathf.Max(0, time - Time.deltaTime);
            transform.position = Vector3.Lerp(link.startPos, link.endPos, 1 - time / totalTime);
            yield return new WaitForSeconds(0);
        }
        agent.CompleteOffMeshLink();
    }

    private void OnDrawGizmos()
    {
/*        Gizmos.DrawLine(transform.position + Vector3.up, player.transform.position);
        if (agent.hasPath != null)
        {
            if (agent.hasPath)
            {
                for (var i = 0; i < agent.path.corners.Length - 1; i++)
                {
                    Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.blue);
                }
            }
        }*/
    }
}

