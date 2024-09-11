using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] EnemyCombatController enemyCombatController;

    [Header("player info")]
    [SerializeField] GameObject player;
    [HideInInspector] PlayerCombat PlayerCombat;
    [SerializeField] float visionRange = 10f;

    [Header("Movement")]
    [SerializeField] float timeJumping = .7f;
    public bool hasLineOfSight = false;
    private EnemyCombatController combatController;
    private NavMeshAgent agent;
    private Animator animator;
    private EnemyHealth enemyHealth;
    [SerializeField] bool isOfMash;
    [SerializeField] LayerMask raycastLayers;
    public bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        PlayerCombat = player.GetComponent<PlayerCombat>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        combatController = GetComponent<EnemyCombatController>();
    }
    private void OnValidate()
    {
        if (agent != null) agent = GetComponent<NavMeshAgent>();
        if (animator != null) animator = GetComponent<Animator>();
    }

    public static bool CheckLineOfSight(Vector3 startPosition, Vector3 targetPosition, float visionRange, LayerMask layers, GameObject target )
    {
        Vector3 directionToTarget = targetPosition - startPosition;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget <= visionRange)
        {
            Ray ray = new Ray(startPosition, directionToTarget);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, visionRange, layers))
            {
                if (hit.transform == target.transform)
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
            if (CheckLineOfSight(transform.position + (Vector3.up * 2), player.transform.position + (Vector3.up * 2), visionRange, raycastLayers, player))
            {
                hasLineOfSight = true;
                combatController.isAttacking = false;
                if (agent.isActiveAndEnabled && canMove)
                    agent.SetDestination(player.transform.position);
            }
        }
        if (agent.hasPath && !combatController.isAttacking && hasLineOfSight && canMove)
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
        if (/*!combatController.isAttacking &&*/ agent.enabled && hasLineOfSight && canMove)
        {
            agent.SetDestination(player.transform.position);
        }
        animator.SetFloat("Horizontal", agent.velocity.magnitude, .5f, Time.deltaTime);
    }

    public void SetDestanashn()
    {
        if (!enemyHealth.deade && hasLineOfSight)
            agent.SetDestination(player.transform.position);
    }

    IEnumerator DoOfMashLink(OffMeshLinkData link)
    {
        if (!enemyHealth.deade)
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
/*        Gizmos.DrawLine(transform.position + (Vector3.up * 2), player.transform.position + (Vector3.up * 2));
        if (agent.hasPath != null && agent.isActiveAndEnabled)
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
