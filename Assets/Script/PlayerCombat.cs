using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/*using static System.IO.Enumeration.FileSystemEnumerable<TResult>;*/

public class PlayerCombat : MonoBehaviour
{
    [Header("player informashen")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform PlayerObj;
    private PlayerMovement playerMovement;

    [Header("enemy informashen")]
    [SerializeField] LayerMask enemyLayerMask;

    [Header("enemy tracking")]
    [SerializeField] float maxDistens;
    [SerializeField] float radius;
    [SerializeField] RaycastHit hit;
    [SerializeField] Animator animator;

    [Header("sward attack info")]
    [SerializeField] float swardAttackTime = 1f;
    [SerializeField] float swardAttackCooldown;
    private bool swardAttackInCooldown = false;
    [SerializeField] bool cantMoveWhileSwardAttack = true;

    [Header("Shoot attack info")]
    public Transform handTransform;
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public LayerMask obstacleLayerMask;
    [SerializeField] float shotAttackTime = 1f;
    [SerializeField] float shotAttackCooldown;
    private bool shotAttackInCooldown = false;
    [SerializeField] bool cantMoveWhileShotAttack = true;

    [Header("shockWave attack info")]
    [SerializeField] float shockWaveAttackDamage = 1f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float waveRadius = 5f;
    [SerializeField] float shockWaveAttackTime = 1f;
    [SerializeField] float shockWaveAttackCooldown;
    private bool shockWaveAttackInCooldown = false;
    [SerializeField] bool cantMoveWhileShockWaveAttack = true;
    [SerializeField] float delayFromeAnimashenToShockwave = 3.0f;
    [SerializeField] ParticleSystem[] particleSystem = new ParticleSystem[0];
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public Vector3 explosionOffset = Vector3.zero;

    [Header("Keybinds")]
    [SerializeField] KeyCode shockWaveKey = KeyCode.Alpha1;

    //in programe atack bool
    public bool isAttackung = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // when to sward Attack
        if (Input.GetMouseButton(0) && !swardAttackInCooldown && !isAttackung)
        {
            //StartCoroutine(SwardAttack());
        }

        // when to shot Attack
        if (Input.GetMouseButtonDown(1) && !shotAttackInCooldown && !isAttackung)
        {
            //StartCoroutine(shotAttack());
        }

        // when to shock wave
        if (Input.GetKeyDown(shockWaveKey) && !shockWaveAttackInCooldown && !isAttackung)
        {
            //StartCoroutine(shockWaveAttack());
        }

        //finds the enemy that the player is loking at
        if (Physics.SphereCast(transform.position, radius, orientation.forward, out hit, maxDistens, enemyLayerMask))
        {
            
        }
    }
    IEnumerator SwardAttack()
    {
        //start attack
        isAttackung = true;
        if (cantMoveWhileSwardAttack)
            playerMovement.canMove = false;
        animator.CrossFade("attack", .2f);

        //end attack
        yield return new WaitForSeconds(swardAttackTime);
        isAttackung = false;
        if (cantMoveWhileSwardAttack)
            playerMovement.canMove = true;

        //start culdown
        swardAttackInCooldown = true;
        yield return new WaitForSeconds(swardAttackCooldown);
        swardAttackInCooldown = false;
    }

    IEnumerator shotAttack()
    {
        //start attack
        isAttackung = true;
        if (cantMoveWhileShotAttack)
            playerMovement.canMove = false;
        animator.CrossFade("shotAttack", .2f);

        //end attack
        yield return new WaitForSeconds(shotAttackTime);

        // Find the enemy the player is looking at
        if (Physics.SphereCast(transform.position, radius, orientation.forward, out hit, maxDistens, enemyLayerMask))
        {
            Debug.Log("enter 1");
            ShootAtTarget(hit.point);
        }
        else
        {
            Debug.Log("enter 2");
            // If no enemy is found, shoot in the direction the player is facing
            ShootForward();
        }

        isAttackung = false;
        if (cantMoveWhileShotAttack)
            playerMovement.canMove = true;

        //start culdown
        shotAttackInCooldown = true;
        yield return new WaitForSeconds(shotAttackCooldown);
        shotAttackInCooldown = false;
    }

    void ShootAtTarget(Vector3 targetPoint)
    {
        GameObject projectile = Instantiate(projectilePrefab, handTransform.position, Quaternion.identity);
        Vector3 direction = (targetPoint - handTransform.position).normalized;

        // Check if there is an obstacle in the way
        if (Physics.Raycast(handTransform.position, direction, out RaycastHit hit, 100f, obstacleLayerMask))
        {
            direction = (hit.point - handTransform.position).normalized;
        }

        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }

    void ShootForward()
    {
        Debug.Log("enter 3");
        GameObject projectile = Instantiate(projectilePrefab, handTransform.position, Quaternion.identity);
        Vector3 direction = PlayerObj.forward;

        // Check if there is an obstacle in the way
        if (Physics.Raycast(handTransform.position, direction, out RaycastHit hit, 100f, obstacleLayerMask))
        {
            direction = (hit.point - handTransform.position).normalized;
        }

        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }

    IEnumerator shockWaveAttack()
    {
        //start attack
        isAttackung = true;
        if (cantMoveWhileShockWaveAttack)
            playerMovement.canMove = false;
        animator.CrossFade("powerd up", .2f);

        //play particals
        yield return new WaitForSeconds(delayFromeAnimashenToShockwave);
        foreach (var particle in particleSystem)
            particle.Play();

        //do damage
        Collider[] hitEnemy = Physics.OverlapSphere(transform.position, waveRadius, enemyLayer);
        foreach (Collider enemy in hitEnemy)
        {
            EnemyBase EnemyBase = enemy.GetComponent<EnemyBase>();
            if (EnemyBase != null)
            {
                EnemyBase.TakeDamage(shockWaveAttackDamage);
            }
            else
            {
                Debug.Log("EnemyBase component is null");
            }
        }
        Explode();

        //end attack
        yield return new WaitForSeconds(shockWaveAttackTime);
        isAttackung = false;
        if (cantMoveWhileShockWaveAttack)
            playerMovement.canMove = true;


        //start culdown
        swardAttackInCooldown = true;
        yield return new WaitForSeconds(shockWaveAttackCooldown);
        shockWaveAttackInCooldown = false;
    }

    void Explode()
    {
        // הגדרת המיקום של הפיצוץ
        Vector3 explosionPosition = transform.position + explosionOffset;

        // קבלת כל הקוליידרים באזור הפיצוץ
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (Collider hit in colliders)
        {

            if (hit.gameObject == this.gameObject || hit.transform.IsChildOf(this.gameObject.transform))
                continue;

            // בדיקה אם יש לאובייקט רכיב Rigidbody
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                Debug.Log(hit.gameObject.name);
                // הוספת כוח פיצוץ על ה-Rigidbody
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (hit.transform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hit.transform.position, radius);
        }
    }
/*    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, waveRadius);
    }*/
}
