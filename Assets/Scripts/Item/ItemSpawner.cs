using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Prefabs")]
    public GameObject ballPrefab;
    public GameObject drinkPrefab;
    public GameObject bookPrefab;

    [Header("Spawn Position")]
    [Tooltip("Set this to a transform slightly above your tray")]
    public Transform spawnPoint;

    public void SpawnBall()
    {
        SpawnItem(ballPrefab);
    }

    public void SpawnDrink()
    {
        SpawnItem(drinkPrefab);
    }

    public void SpawnBook()
    {
        SpawnItem(bookPrefab);
    }

    private void SpawnItem(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab missing from Spawner script references!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point Transform is not assigned!");
            return;
        }

        // Spawn the item at the spawn point with no rotation
        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"Spawned {prefab.name} onto tray");
    }
}