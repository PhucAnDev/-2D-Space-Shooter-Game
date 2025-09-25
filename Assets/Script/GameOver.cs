using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void ITryAgainAndAgain()
    {         
        SceneManager.LoadScene("GameScene");
    }
}