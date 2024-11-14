using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    private TMP_Text txtScore;

    int score = 0;

    private static UIScore _instance;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        txtScore = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        txtScore.text = "Score: " + score.ToString();
    }

    // Called when a house is successfully placed either in Dirt tile or Desert tile
    public static void AddScore(int score)
    {
        _instance.score += score;
    }
}
