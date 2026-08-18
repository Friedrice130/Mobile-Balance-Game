using UnityEngine;

public class RedLightTrigger : MonoBehaviour
{
    public RedLightManager manager;
    public bool isStartLine = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && manager != null)
        {
            if (isStartLine)
            {
                manager.StartMiniGame();
                Debug.Log("Red Light Mini-Game Started!");
            }
            else
            {
                manager.WinMiniGame();
                Debug.Log("Safe Zone Reached! Mini-Game Beaten.");
            }
        }
    }
}
