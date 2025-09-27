using UnityEngine;
using UnityEngine.UI;

public class ShieldUIController : MonoBehaviour
{
    [SerializeField] Image cooldownCircle;   // Hình tròn fill radial
    [SerializeField] PlayerController player;

    void Update()
    {
        if (!player || !cooldownCircle) return;

        if (!player.IsShieldReady())
        {
            float progress = player.GetShieldCooldownProgress();
            cooldownCircle.fillAmount = progress; // fill dần từ 0 → 1
        }
        else
        {
            cooldownCircle.fillAmount = 0f; // reset khi sẵn sàng
        }
    }




}
