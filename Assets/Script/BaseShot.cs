using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseShot : MonoBehaviour
{
    [SerializeField] float Dameg;
    public GameObject hitEffect;
    [SerializeField] LayerMask layerMask;
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
/*        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)*/
        {
            if (other != null && other.GetComponent<EnemyHealth>())
            {
                var enemyHealth = other.GetComponent<EnemyHealth>();
                enemyHealth.TakeDamage(Dameg);
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
        // ����� ������ �� ������
        Vector3 explosionPosition = transform.position + explosionOffset;

        // ���� �� ���������� ����� ������
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (Collider hit in colliders)
        {

            if (hit.CompareTag("Player") || hit.CompareTag("sword") || hit.gameObject == this.gameObject || hit.transform.IsChildOf(this.gameObject.transform))
                continue;

            // ����� �� �� �������� ���� Rigidbody
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                // ����� ��� ����� �� �-Rigidbody
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }
        }
    }
}
