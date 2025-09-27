using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text scoreText;

    public int Score { get; private set; } 
    public int HighestScore { get; private set; }
    public bool isCounting = true;

    void Start()
    {
        HighestScore = PlayerPrefs.GetInt("HighestScore", 0); // load high score đã lưu
        UpdateUI();
        StartCoroutine(AddScoreEverySecond());
    }

    IEnumerator AddScoreEverySecond()
    {
        while (true)
        {
            if (isCounting)
            {
                Score += 1;          // +1 ði?m m?i giây
                UpdateUI();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void AddPoints(int amount)
    {
        Score += amount;
        UpdateUI();
    }

    public void StopCounting() => isCounting = false;    // g?i khi Game Over
    public void ResumeCounting() => isCounting = true;

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {Score}";
    }
    public void OnGameOver()
    {
        StopCounting();
        PlayerPrefs.SetInt("LastScore", Score);
        if (Score > HighestScore)
        {
            HighestScore = Score;
            PlayerPrefs.SetInt("HighestScore", HighestScore);
        }
        PlayerPrefs.Save();

        Debug.Log($"[ScoreManager] Save: Last={Score}, High={HighestScore}");
        SceneManager.LoadScene("GameOverScene");
    }

}

