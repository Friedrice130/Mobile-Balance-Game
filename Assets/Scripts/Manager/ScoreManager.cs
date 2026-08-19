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

    // Ensures remaining items never drop below 0
    public int RemainingItems => Mathf.Max(0, totalItems - droppedItems);

    public int TotalItems => totalItems;

    public int FinalScore // for Rank
    {
        get
        {
            if (TotalItems <= 0) return 172; // Floor at 100 if no items exist

            float itemScore = ((float)RemainingItems / TotalItems) * 50f;

            // Clamp CurrentTime so negative time doesn't drag the score down
            float validTime = Mathf.Max(0f, GameManager.Instance.CurrentTime);
            float timeScore = (validTime / GameManager.Instance.MaxTime) * 50f;

            // Ensures final score never drops below 100
            return Mathf.Max(100, Mathf.RoundToInt(itemScore + timeScore));
        }
    }

    public int DisplayScore // for Score 4 digits
    {
        get
        {
            if (TotalItems <= 0) return 100;

            float itemScore = ((float)RemainingItems / TotalItems) * 50f;

            float validTime = Mathf.Max(0f, GameManager.Instance.CurrentTime);
            float timeScore = (validTime / GameManager.Instance.MaxTime) * 50f;

            float totalScore = itemScore + timeScore;

            // Ensures display score never drops below 100
            return Mathf.Max(100, Mathf.RoundToInt(totalScore * 10f));
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

    public bool IsTutorialScore()
    {
        return GameManager.Instance.useTutorial;
    }
}