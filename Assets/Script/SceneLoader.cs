using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string gameSceneName = "GameScene";

    void OnMouseUpAsButton()   // g?i khi nh? chu?t tr�n collider
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
