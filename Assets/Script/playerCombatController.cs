using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class playerCombatController : MonoBehaviour
{
    //slote list
    public List<GameObject> slotObjects;
    private List<SlotManeger> slotManegers = new List<SlotManeger>();


    [Header("player informashen")]
    public Transform orientation;
    public Transform PlayerObj;
    public PlayerMovement playerMovement;
    public PlayerStats playerStats;
    public Animator animator;

    [Header("enemy tracking")]
    public float maxDistens;
    public float radius;
    public RaycastHit hit;
    public LayerMask enemyLayerMask;
    public float maxAngle = 45f;
    public GameObject markerPrefab;

    [SerializeField] KeyCode attackA = KeyCode.C;
    [SerializeField] KeyCode attackB = KeyCode.V;
    [SerializeField] KeyCode attack1 = KeyCode.Alpha1;
    [SerializeField] KeyCode attack2 = KeyCode.Alpha2;
    [SerializeField] KeyCode attack3 = KeyCode.Alpha3;

    //in programe atack bool
    public bool isAttackung = false;
    public int attackIndex = -1;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStats = GetComponent<PlayerStats>();

        // fils the slots
        foreach (GameObject obj in slotObjects)
        {
            SlotManeger slot = obj.GetComponent<SlotManeger>();
            if (slot != null)
            {
                slotManegers.Add(slot);
            }
        }
    }

    void Update()
    {
        // slot activate the slote that got prest
        foreach (SlotManeger obj in slotManegers)
        {
            // when to use attack
            if (Input.GetKeyDown(obj.slotKeybind))
            {
                obj.ExecuteSlotAction(); //activate attack
            }
        }

        FindAndMarkEnemy();
    }

    private void FindAndMarkEnemy()
    {
        // חיפוש האויב הקרוב ביותר
        Collider[] colliders = Physics.OverlapSphere(PlayerObj.position, maxDistens, enemyLayerMask);
        Collider bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = PlayerObj.position;

        foreach (Collider potentialTarget in colliders)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            float angleToTarget = Vector3.Angle(PlayerObj.forward, directionToTarget);
            if (angleToTarget <= maxAngle && dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        if (bestTarget != null)
        {
            StartCoroutine(MarkEnemyRoutine(bestTarget.transform));
        }
    }

    private IEnumerator MarkEnemyRoutine(Transform enemyTransform)
    {
        /*        Renderer renderer = enemyTransform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Color originalColor = renderer.material.color;
                    renderer.material.color = Color.red;

                    yield return new WaitForEndOfFrame();  // mark time

                    renderer.material.color = originalColor;
                }*/
        // calulat enemy hith
/*        Collider enemyCollider = enemyTransform.GetComponent<Collider>();
        float markerHeight = enemyCollider != null ? enemyCollider.bounds.size.y : 2f;

        GameObject marker = Instantiate(markerPrefab, enemyTransform.position + Vector3.up * (markerHeight + 1f), Quaternion.identity);
        marker.transform.SetParent(enemyTransform);*/

        Outline outline = enemyTransform.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = true;

        yield return new WaitForEndOfFrame();  // משך הסימון עד הפריים הבא

        if (outline != null )
            outline.enabled = false;

/*        Destroy(marker);*/
    }

    // Gizmo to show the shooting angle
    private void OnDrawGizmos()
    {
        if (orientation != null)
        {
            Gizmos.color = Color.green;
            Vector3 forward = PlayerObj.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, maxAngle, 0) * forward;
            Vector3 leftBoundary = Quaternion.Euler(0, -maxAngle, 0) * forward;

            Gizmos.DrawLine(PlayerObj.position, PlayerObj.position + rightBoundary * maxDistens);
            Gizmos.DrawLine(PlayerObj.position, PlayerObj.position + leftBoundary * maxDistens);
        }
    }
}


interface Isaveble
{
    bool GetAlive();
    void SetAlive(bool value);
}

class SaveEnemy : MonoBehaviour
{
    bool alive;

    void Start()
    {
        alive = PlayerPrefs.GetInt($"Enemy:{name}:Alive") == 1;
    }

    void Save()
    {
        PlayerPrefs.SetInt($"Enemy:{name}:Alive", !alive ? 0 : 1);
    }
}