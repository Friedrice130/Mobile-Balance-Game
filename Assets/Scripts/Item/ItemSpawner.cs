using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Prefab")]
    public GameObject itemPrefab;

    [Header("Spawn Position")]
    public Transform spawnPoint;

    private void Start()
    {
        SpawnAssignedItem();
    }

    public void SpawnAssignedItem()
    {
        if (itemPrefab == null)
        {
            Debug.LogError("No item prefab has been assigned in the Inspector for this scene!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point Transform is not assigned!");
            return;
        }

        // Spawn the assigned item at the spawn point
        Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"{itemPrefab.name} spawned onto tray");
    }
}