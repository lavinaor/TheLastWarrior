using System.Collections;
using System.Collections.Generic;
using Tiny;
using Unity.Mathematics;
using UnityEngine;


public class BackstepActionPlayer : MonoBehaviour, IAction
{
    [SerializeField] playerCombatController playerCombatController;

    [SerializeField] float Distance = 5f;
    [SerializeField] float Speed = 20f;
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
        float dashTime = Distance / Speed;
        float elapsedTime = 0f;

        isActive = true;
        trail.SetActive(true);
        playerCombatController.isAttackung = true;
        playerCombatController.animator.SetBool("Swordforward", true);

        // play sound FX 
        SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform, 0.5f);

        Vector3 dashDirection = -playerCombatController.PlayerObj.transform.forward;
        Vector3 startPosition = playerCombatController.transform.position;
        Vector3 endPosition = startPosition + dashDirection * Distance;

        // Perform a raycast to check for obstacles
        RaycastHit hit;
        if (Physics.Raycast(startPosition, dashDirection, out hit, Distance, obstacleMask))
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

    // Gizmo to visualize the Raycast
    private void OnDrawGizmos()
    {
        if (playerCombatController == null) return;

        Vector3 dashDirection = playerCombatController.PlayerObj.transform.forward;
        Vector3 startPosition = playerCombatController.transform.position;
        Vector3 endPosition = startPosition + dashDirection * Distance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, endPosition);

        RaycastHit hit;
        if (Physics.Raycast(startPosition, dashDirection, out hit, Distance, obstacleMask))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.5f);
            Gizmos.DrawLine(startPosition, hit.point); // Draw line to the hit point
        }
    }
}