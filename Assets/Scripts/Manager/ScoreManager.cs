using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int totalItems;
    private int droppedItems;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        totalItems = Object.FindObjectsByType<TrayItem>().Length;
    }

    public void ItemDropped()
    {
        droppedItems++;
    }

    public int RemainingItems => totalItems - droppedItems;

    public int TotalItems => totalItems;

    public int FinalScore
    {
        get
        {
            float itemScore = ((float)RemainingItems / TotalItems) * 60f;
            float timeScore = (GameManager.Instance.CurrentTime / GameManager.Instance.MaxTime) * 40f;

            return Mathf.RoundToInt(itemScore + timeScore);
        }
    }

    public string GetRank(int score)
    {
        if (score >= 80)
            return "S+";

        if (score >= 60)
            return "A";

        return "B";
    }
}