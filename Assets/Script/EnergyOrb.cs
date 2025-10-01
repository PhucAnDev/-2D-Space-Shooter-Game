using UnityEngine;

// 10/1/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.


public class EnergyOrb : MonoBehaviour
{
    public float fuelAmount = 20f; // Lượng nhiên liệu nạp thêm

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.RefillFuel(fuelAmount); // Nạp nhiên liệu
            }
            Destroy(gameObject); // Xóa EnergyOrb sau khi thu thập
        }
    }
}
