using UnityEngine;

public class Endpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.Playing)
            {
                GameManager.Instance.TriggerGameWin();
            }
        }
    }
}
