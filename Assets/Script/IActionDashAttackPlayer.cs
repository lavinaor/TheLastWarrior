using System.Collections;
using System.Collections.Generic;
using Tiny;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class IActionDashAttackPlayer : MonoBehaviour, IAction
{
    [SerializeField] playerCombatController playerCombatController;

    [SerializeField] float Dameg;
    [SerializeField] float dashDistance = 5f;
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float endPositionOffset = 0.1f;
    [SerializeField] GameObject trail;
    [SerializeField] float trailTime;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] AudioClip audioClip;
    [SerializeField] bool isActive;

    public void ExecuteAction()
    {
        StartCoroutine(DashAttack());
    }
    IEnumerator DashAttack()
    {
        float dashTime = dashDistance / dashSpeed;
        float elapsedTime = 0f;

        isActive = true;
        trail.SetActive(true);
        playerCombatController.isAttackung = true;
        playerCombatController.animator.SetBool("Swordforward", true);

        // play sound FX 
        SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform, 0.5f);

        Vector3 dashDirection = playerCombatController.PlayerObj.transform.forward;
        Vector3 startPosition = playerCombatController.transform.position;
        Vector3 endPosition = startPosition + dashDirection * dashDistance;

        // Perform a raycast to check for obstacles
        RaycastHit hit;
        if (Physics.Raycast(startPosition, dashDirection, out hit, dashDistance, obstacleMask))
        {
            endPosition = hit.point - (dashDirection * endPositionOffset);
        }

        while (elapsedTime < dashTime)
        {
            playerCombatController.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / dashTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerCombatController.transform.position = endPosition;
        playerCombatController.isAttackung = false;
        playerCombatController.animator.SetBool("Swordforward", false);
        isActive = false;

        new WaitForSeconds(trailTime);
        trail.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.GetComponent<EnemyHealth>() && isActive)
        {
            var enemyHealth = other.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(Dameg);
        }
    }

    // Gizmo to visualize the Raycast
    private void OnDrawGizmos()
    {
        if (playerCombatController == null) return;

        Vector3 dashDirection = playerCombatController.PlayerObj.transform.forward;
        Vector3 startPosition = playerCombatController.transform.position;
        Vector3 endPosition = startPosition + dashDirection * dashDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, endPosition);

        RaycastHit hit;
        if (Physics.Raycast(startPosition, dashDirection, out hit, dashDistance, obstacleMask))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.5f);
            Gizmos.DrawLine(startPosition, hit.point); // Draw line to the hit point
        }
    }
}
