using UnityEngine;
using TMPro;

public class GameOverScore : MonoBehaviour
{
    [SerializeField] TMP_Text Score;
    [SerializeField] TMP_Text HighestScore;

    void Start()
    {
        int last = PlayerPrefs.GetInt("LastScore", 0);
        int high = PlayerPrefs.GetInt("HighestScore", 0);

        if (Score != null)
            Score.text = $"Your Score: {last}";

        if (HighestScore != null)
            HighestScore.text = $"High Score: {high}";
    }
}
