using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShotEnemy : MonoBehaviour
{
    [SerializeField] float Dameg;
    public GameObject hitEffect;
    private bool hasBeanTrigerd;

    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public Vector3 explosionOffset = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeanTrigerd)
        {
            return;
        }

        {
            if (other != null && other.GetComponent<PlayerStats>())
            {
                var playerStats = other.GetComponent<PlayerStats>();
                playerStats.TakeDamage(Dameg);
            }

            hasBeanTrigerd = true;
            // Create hit effect
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // Destroy the effect after 2 seconds

            Explode();

            // Destroy the bullet
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        Vector3 explosionPosition = transform.position + explosionOffset;

        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (Collider hit in colliders)
        {

            if (hit.CompareTag("Player") || hit.CompareTag("sword") || hit.gameObject == this.gameObject || hit.transform.IsChildOf(this.gameObject.transform))
                continue;

            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }
        }
    }
}
