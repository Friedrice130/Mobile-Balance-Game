using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TrayItem : MonoBehaviour
{
    [Header("Item Properties")]
    public string itemName;
    public float weight = 1f;
    public bool isFragile = false;
    public float breakForceThreshold = 5f; // How hard of an impact breaks it

    private Rigidbody rb;
    private bool isDestroyed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Apply the custom weight to the Rigidbody's mass
        rb.mass = weight;
    }

    void Update()
    {
        // If the item falls below the tray
        if (transform.position.y < -5f && !isDestroyed)
        {
            TriggerGameOver("Item fell off the tray!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isFragile && !isDestroyed)
        {
            // collision.relativeVelocity tells us how hard the impact was
            if (collision.relativeVelocity.magnitude > breakForceThreshold)
            {
                TriggerGameOver($"You lose! {itemName} fragile item broke.");
            }
        }
    }

    private void TriggerGameOver(string reason)
    {
        if (isDestroyed) return;
        isDestroyed = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver(reason);
        }

        Destroy(gameObject);
    }
}