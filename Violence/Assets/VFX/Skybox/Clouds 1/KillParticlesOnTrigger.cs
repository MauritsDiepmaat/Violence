using UnityEngine;

public class LightningSpawning : MonoBehaviour
{
    public GameObject prefabToSpawn; // Assign your custom shader prefab here.
    public int maxSpawnCount = 100; // Maximum number of objects to spawn.
    public Vector3 spawnArea = new Vector3(10f, 2f, 10f); // Define the area where objects can spawn.
    public float spawnInterval = 1.0f; // Time interval between spawns.

    private float timer = 0.0f;
    private int spawnCount = 0;

    private void Update()
    {
        if (spawnCount < maxSpawnCount)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnCustomObject();
                timer = 0.0f;
            }
        }
    }

    private void SpawnCustomObject()
    {
        if (prefabToSpawn != null)
        {
            Vector3 randomPosition = transform.position + new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y),
                Random.Range(-spawnArea.z, spawnArea.z)
            );

            Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
            spawnCount++;
        }
    }
}
