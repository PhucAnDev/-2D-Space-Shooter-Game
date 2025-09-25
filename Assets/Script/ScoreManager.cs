using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text scoreText;

    public int Score { get; private set; } = 0;
    public int HighestScore { get; private set; } = 0;
    public bool isCounting = true;

    void Start()
    {
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
        // stop tính điểm
        StopCounting();

        // Lưu LastScore
        PlayerPrefs.SetInt("LastScore", Score);

        // Cập nhật & lưu HighestScore nếu cần
        if (Score > HighestScore)
        {
            HighestScore = Score;
            PlayerPrefs.SetInt("HighestScore", HighestScore);
        }

        // Ghi ngay xuống disk
        PlayerPrefs.Save();

        FindObjectOfType<ScoreManager>().OnGameOver();
    }
}

