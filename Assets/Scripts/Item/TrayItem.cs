using UnityEngine;
using CandyCoded.HapticFeedback;


public class TrayItem : MonoBehaviour
{
    [Header("Item Properties")]
    public string itemName;
    public bool isImportant = false;

    [Header("Tutorial")]
    public bool stickToTray = false;
    public Transform trayAnchor;   // Assign TrayVisual here

    private bool isDestroyed = false;

    void LateUpdate()
    {
        if (stickToTray && trayAnchor != null)
        {
            transform.position = trayAnchor.position;
            transform.rotation = trayAnchor.rotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log($"{itemName} hit the ground.");

            HapticManager.Instance?.Default();
            ScoreManager.Instance?.ItemDropped();

            if (isImportant)
            {
                TriggerGameOver($"Game Over: {itemName} (Important) fell off the tray!");
            }
            else
            {
                isDestroyed = true;
                //Destroy(gameObject, 2f);
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

        
        //Destroy(gameObject, 2f);
    }
}