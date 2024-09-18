using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class SpawnCreatures : MonoBehaviour, IAction
{
    [SerializeField] EnemyCombatController enemyCombatController;

    [SerializeField] private List<GameObject> creaturesPrefabs;
    [SerializeField] private int numberOfCreaturesToSpawn;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float heightAboveGround;
    [SerializeField] private float spawnDelay;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallsAndMore;

    private List<GameObject> spawnedCreatures = new List<GameObject>();

    public void ExecuteAction()
    {
        StartCoroutine(SpawnCreaturesWithDelay());
    }

    private IEnumerator SpawnCreaturesWithDelay()
    {
        for (int i = 0; i < numberOfCreaturesToSpawn; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();
            GameObject creaturePrefab = creaturesPrefabs[Random.Range(0, creaturesPrefabs.Count)];
            GameObject spawnedCreature = Instantiate(creaturePrefab, spawnPosition, Quaternion.identity);
            spawnedCreatures.Add(spawnedCreature);

            EnemyMovement enemyMovement = spawnedCreature.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.hasLineOfSight = true;
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        // Start monitoring the creatures to see if they get destroyed
        StartCoroutine(CheckCreaturesStatus());
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPosition;
        bool validPosition = false;

        do
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            spawnPosition = new Vector3(randomPos.x, transform.position.y, randomPos.y) + transform.position;

            // Raycast to find the ground level
            if (Physics.Raycast(spawnPosition + Vector3.up * 1000f, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                spawnPosition.y = hit.point.y + heightAboveGround;
                if (!CheckLineOfSight(this.transform.position + Vector3.up, spawnPosition, spawnRadius * 2, wallsAndMore))
                    validPosition = true;
            }

        } while (!validPosition);

        return spawnPosition;
    }

    public static bool CheckLineOfSight(Vector3 startPosition, Vector3 targetPosition, float visionRange, LayerMask layers)
    {
        Vector3 directionToTarget = targetPosition - startPosition;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget <= visionRange)
        {
            Ray ray = new Ray(startPosition, directionToTarget);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, visionRange, layers))
            {
                return true;            
            }
        }
        return false;
    }

    private IEnumerator CheckCreaturesStatus()
    {
        while (spawnedCreatures.Count > 0)
        {
            for (int i = spawnedCreatures.Count - 1; i >= 0; i--)
            {
                EnemyHealth enemyHealth = spawnedCreatures[i].GetComponent<EnemyHealth>();
                if (enemyHealth != null && enemyHealth.health <= 0)
                {
                    spawnedCreatures.RemoveAt(i);
                }
            }

            if (spawnedCreatures.Count == 0)
            {
                Debug.Log("All creatures have been defeated!");
                enemyCombatController.EndProtectedSequence();
            }

            yield return new WaitForSeconds(1f); // Check every second
        }
    }

    // Gizmo to visualize spawn radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
