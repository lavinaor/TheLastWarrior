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
    [SerializeField] int poishenIncris = 1;
    [SerializeField] bool textBool = false;
    [SerializeField] AudioClip audioClip;

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
        if (playerInRange)
        {
            if (!opend)
            {
                if (textBool)
                {
                   interactionText.gameObject.SetActive(true);
                }
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
                if (textBool)
                {
                    interactionText.gameObject.SetActive(false);
                }
                a = false;
            }
        }
    }

    IEnumerator InteractebalText()
    {
        interactionText.gameObject.SetActive(true);
        interactionText.text = "Press 'E' to open";

        yield return new WaitForSeconds(3f);

        if (!playerInRange)
            interactionText.gameObject.SetActive(false);
    }
    
    void OpenLootBox()
    {
        Debug.Log("open2");
        animator.CrossFade("open", 1f);
        interactionText.gameObject.SetActive(false);
        playerStats.AddHealthPotion(poishenIncris);
        opend = true;

        // play sound FX 
        SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform, 0.5f);

        interactionText.text = "'G'";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}
