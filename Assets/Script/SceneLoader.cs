using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string gameSceneName = "GameScene";

    void OnMouseUpAsButton()   // g?i khi nh? chu?t trên collider
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
