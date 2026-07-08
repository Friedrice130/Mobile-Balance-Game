using UnityEngine;

public class TriggerActivateObject : MonoBehaviour
{
    [Header("Trigger Settings")]
    public GameObject objectToActivate;
    public bool triggerOnce = true;
    public bool hideOnStart = true;

    private bool hasTriggered = false;

    void Start()
    {
        if (hideOnStart && objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }
        }
    }
}
