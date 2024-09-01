using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombatController : MonoBehaviour
{
    public List<GameObject> actionObjects;
    private List<IAction> actions = new List<IAction>();

    [Header("Attacks")]
    [SerializeField] bool hasFirstAttack = false;
    [SerializeField] float firstAttackDistens = 0f;
    [SerializeField] bool firstAttackDistensIaStopingDistens = true;
    [SerializeField] bool hasSecondAttack = false;
    [SerializeField] float secondAttackDistens = 0f;
    [SerializeField] bool hasTherdAttack = false;
    [SerializeField] float therdAttackDistens = 0f;

    [Header("ProtectedSequence")]
    [SerializeField] bool hasProtectedSequence = false;
    private bool inProtectedSequence = false;
    private int curentProtectedSequenceNumber = 0;
    [SerializeField] float[] ProtectedSequenceHealthPresenteges = new float[0];
    private float StartHealth;
    public GameObject ProtectedSequenceActionObject;
    private IAction ProtectedSequenceAction;

    [Header("info")]
    public GameObject player;
    public bool isAttacking = true;
    public EnemyMovement EnemyMovement;
    public EnemyHealth EnemyHealth;
    public NavMeshAgent agent;
    public Animator animator;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        EnemyMovement = GetComponent<EnemyMovement>();
        EnemyHealth = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();


        if (firstAttackDistensIaStopingDistens)
            firstAttackDistens = agent.stoppingDistance;


        StartHealth = EnemyHealth.health;


        foreach (GameObject obj in actionObjects)
        {
            IAction action = obj.GetComponent<IAction>();
            if (action != null)
            {
                actions.Add(action);
            }
        }
        if (hasProtectedSequence)
        {
            // get Protected Sequence Action
            IAction action2 = ProtectedSequenceActionObject.GetComponent<IAction>();
            if (action2 != null)
                ProtectedSequenceAction = action2;
        }
    }
    private void Update()
    {
        //test for ProtectedSequence
        if (hasProtectedSequence && ProtectedSequenceHealthPresenteges.Length > curentProtectedSequenceNumber)
        {
            if ((100 / StartHealth) * EnemyHealth.health <= ProtectedSequenceHealthPresenteges[curentProtectedSequenceNumber])
            {
                curentProtectedSequenceNumber++;
                animator.SetBool("shilded", true);
                inProtectedSequence = true;
                EnemyHealth.invincible = true;
                EnemyHealth.TakeDamage(0f);

                // start the ackshen
                ProtectedSequenceAction.ExecuteAction();
            }
        }

        if (!isAttacking && !EnemyHealth.deade && agent.isActiveAndEnabled && !inProtectedSequence)
        {
            //activate when in therdAttackDistens frome the player
            if (DistensCheck(firstAttackDistens) && hasFirstAttack)
            {
                if (actions.Count > 0 && actions[0] != null)
                {
                    actions[0].ExecuteAction();
                }
            }
            //activate when in secondAttackDistens frome the player
            if (DistensCheck(secondAttackDistens) && hasSecondAttack)
            {
                if (actions.Count > 1 && actions[1] != null)
                {
                    actions[1].ExecuteAction();
                }
            }
            //activate when in therdAttackDistens frome the player
            if (DistensCheck(therdAttackDistens) && hasTherdAttack)
            {
                if (actions.Count > 2 && actions[2] != null)
                {
                    actions[2].ExecuteAction();
                }
            }
        }
    }

    public bool DistensCheck(float visionRange)
    {
        Vector3 directionToTarget = player.transform.position - this.transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget <= visionRange)
        {
             return true;
        }
        return false;
    }

    public void EndProtectedSequence()
    {
        animator.SetBool("shilded", false);
        inProtectedSequence = false;
        EnemyHealth.invincible = false;
        EnemyHealth.TakeDamage(0f);
    }
}
