using UnityEngine;

public enum Rank
{
    B,
    A,
    SPlus
}

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

    public int FinalScore //for Rank
    {
        get
        {
            float itemScore = ((float)RemainingItems / TotalItems) * 50f;
            float timeScore = (GameManager.Instance.CurrentTime / GameManager.Instance.MaxTime) * 50f;

            return Mathf.RoundToInt(itemScore + timeScore);
        }
    }

    public int DisplayScore //for Score 4 digits
    {
        get
        {
            float itemScore = ((float)RemainingItems / TotalItems) * 50f;
            float timeScore = (GameManager.Instance.CurrentTime / GameManager.Instance.MaxTime) * 50f;

            float totalScore = itemScore + timeScore;

            return Mathf.RoundToInt(totalScore * 10f);
        }
    }

    public Rank FinalRank
    {
        get
        {
            if (FinalScore >= 90)
                return Rank.SPlus;

            if (FinalScore >= 70)
                return Rank.A;

            return Rank.B;
        }
    }
}