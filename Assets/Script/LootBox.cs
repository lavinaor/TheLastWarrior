using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootBox : MonoBehaviour
{
    [SerializeField] bool opend = false;
    [SerializeField] Animator animator;
    [SerializeField] float activationRadius = 5f;
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] TMP_Text interactionText; // UI Text for interaction
    [SerializeField] PlayerStats playerStats; // Reference to the player's stats
    [SerializeField] Camera mainCamera; // Reference to the main camera
    [SerializeField] int poishenIncris = 1;

    private bool playerInRange = false;
    private bool a = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        interactionText.gameObject.SetActive(false); // Hide the interaction text at the start
        interactionText.text = "Press 'G' to open";
    }

    void Update()
    {
        playerInRange = Physics.CheckSphere(transform.position, activationRadius, playerLayerMask);
        if (playerInRange && IsBoxOnScreen())
        {
            if (!opend)
            {
                interactionText.gameObject.SetActive(true);
                a = true;
            }

            if (Input.GetKeyDown(KeyCode.G) && !opend)
            {
                OpenLootBox();
            }
        }
        else
        {
            if (a == true)
            {
                interactionText.gameObject.SetActive(false);
                a = false;
            }
        }
    }

    IEnumerator InteractebalText()
    {
        interactionText.gameObject.SetActive(true);
        interactionText.text = "Press 'E' to open";

        yield return new WaitForSeconds(3f);

        if (!playerInRange && !IsBoxOnScreen())
            interactionText.gameObject.SetActive(false);
    }
    
    void OpenLootBox()
    {
        Debug.Log("open2");
        animator.CrossFade("open", 1f);
        interactionText.gameObject.SetActive(false);
        playerStats.AddHealthPotion(poishenIncris);
        opend = true;
    }

    bool IsBoxOnScreen()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}
