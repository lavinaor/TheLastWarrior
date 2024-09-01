using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ShootAttackIActionPlayer : MonoBehaviour, IAction
{
    [SerializeField] playerCombatController playerCombatController;

    [Header("Shoot attack info")]
    public Transform handTransform;
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public LayerMask obstacleLayerMask;
    [SerializeField] float shotAttackTime = 1f;
    [SerializeField] float shotAttackCooldown;
    private float shotAttackInCooldown = 0f;
    public Image CooldownImageFill;
    public TMP_Text CooldownText;
    [SerializeField] bool cantMoveWhileShotAttack = true;
    [SerializeField] AudioClip audioClip;

    private void Start()
    {
        UpdateCooldownUI();
    }
    private void Update()
    {
        if (shotAttackInCooldown > 0f)
        {
            shotAttackInCooldown -= Time.deltaTime;
            UpdateCooldownUI();
        }
    }
    public void ExecuteAction()
    {
        StartCoroutine(shotAttack());
    }
    IEnumerator shotAttack()
    {
        if (shotAttackInCooldown <= 0)
        {
            //start attack
            playerCombatController.isAttackung = true;
            if (cantMoveWhileShotAttack)
                playerCombatController.playerMovement.canMove = false;
            playerCombatController.animator.Play("shotAttack");

            //end attack
            yield return new WaitForSeconds(shotAttackTime);

            // play sound FX 
            SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform, 0.5f);

            // Find the best target to shoot at
            Vector3 targetPoint = FindBestTarget();
            if (targetPoint != Vector3.zero)
            {
                ShootAtTarget(targetPoint);
            }
            else
            {
                ShootForward();
            }

            playerCombatController.isAttackung = false;
            if (cantMoveWhileShotAttack)
                playerCombatController.playerMovement.canMove = true;

            //start culdown
            shotAttackInCooldown = shotAttackCooldown;
            UpdateCooldownUI();
        }
    }

    Vector3 FindBestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(playerCombatController.PlayerObj.position, playerCombatController.maxDistens, playerCombatController.enemyLayerMask);
        Collider bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = playerCombatController.PlayerObj.position;

        foreach (Collider potentialTarget in colliders)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            float angleToTarget = Vector3.Angle(playerCombatController.PlayerObj.forward, directionToTarget);
            if (angleToTarget <= playerCombatController.maxAngle && dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        if (bestTarget != null)
        {
            return bestTarget.transform.position;
        }

        return Vector3.zero;
    }

    void ShootAtTarget(Vector3 targetPoint)
    {
        GameObject projectile = Instantiate(projectilePrefab, handTransform.position, Quaternion.identity);
        Vector3 direction = ((targetPoint + Vector3.up) - handTransform.position).normalized;

        // Check if there is an obstacle in the way
        if (Physics.Raycast(handTransform.position, direction, out RaycastHit hit, 100f, obstacleLayerMask))
        {
            direction = (hit.point - handTransform.position).normalized;
        }

        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }

    void ShootForward()
    {
        GameObject projectile = Instantiate(projectilePrefab, handTransform.position, Quaternion.identity);
        Vector3 direction = playerCombatController.PlayerObj.forward;

        // Check if there is an obstacle in the way
        if (Physics.Raycast(handTransform.position, direction, out RaycastHit hit, 100f, obstacleLayerMask))
        {
            direction = (hit.point - handTransform.position).normalized;
        }

        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }

    public void UpdateCooldownUI()
    {
        CooldownImageFill.fillAmount = shotAttackCooldown * 10 - shotAttackInCooldown * 10;
        CooldownText.text = shotAttackInCooldown.ToString("0");

        if (shotAttackInCooldown > 0 && shotAttackCooldown > 1)
            CooldownText.text = shotAttackInCooldown.ToString("0");
        else
            CooldownText.text = new string(" ");
    }

    // Gizmo to show the shooting angle
    private void OnDrawGizmos()
    {
        if (playerCombatController != null && playerCombatController.orientation != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 forward = playerCombatController.PlayerObj.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, playerCombatController.maxAngle, 0) * forward;
            Vector3 leftBoundary = Quaternion.Euler(0, -playerCombatController.maxAngle, 0) * forward;

            Gizmos.DrawLine(playerCombatController.PlayerObj.position, playerCombatController.PlayerObj.position + rightBoundary * playerCombatController.maxDistens);
            Gizmos.DrawLine(playerCombatController.PlayerObj.position, playerCombatController.PlayerObj.position + leftBoundary * playerCombatController.maxDistens);
        }
    }
}

