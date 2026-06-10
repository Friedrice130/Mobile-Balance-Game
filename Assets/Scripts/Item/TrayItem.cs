using UnityEngine;
using CandyCoded.HapticFeedback;


public class TrayItem : MonoBehaviour
{
    [Header("Item Properties")]
    public string itemName;
    public bool isImportant = false;

    private bool isDestroyed = false;


    void Update()
    {
        if (transform.position.y < 1f && !isDestroyed)
        {
            if (isImportant)
            {
                TriggerGameOver($"Game Over: {itemName} (Important) fell off the tray!");
                HapticFeedback.HeavyFeedback();
            }
            else
            {
                isDestroyed = true;

                Destroy(gameObject, 2f);
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

        Destroy(gameObject, 2f);
    }
}