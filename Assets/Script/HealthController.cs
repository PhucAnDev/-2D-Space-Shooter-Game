using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class HealthController : MonoBehaviour
{
    public Image healthBar; // Reference to the UI Image representing the health bar
    public float healthAmount = 100f; // Maximum health value
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // N?u máu <= 0 th? load l?i scene hi?n t?i
        if (healthAmount <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Nh?n phím Enter (Return) ð? m?t máu 20
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeDamage(20);
        }

        // Nh?n phím Space ð? h?i máu 5
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Heal(5);
        }
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f; // Update the health bar fill amount
        if (healthAmount <= 0)
        {
            // Handle player death (e.g., restart game, show game over screen, etc.)
            Debug.Log("Player is dead!");
        }
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        healthBar.fillAmount = healthAmount / 100f;
    }
}
