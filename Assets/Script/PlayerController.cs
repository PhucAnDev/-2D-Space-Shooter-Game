using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;

    [Header("Missile")]
    public GameObject missile;
    public Transform missileSpawnPoint;
    public float destroyTime = 5f;

    [Header("Shield")]
    public GameObject shieldPrefab;     // kéo prefab Shield vào đây
    public float shieldDuration = 2f;   // bật trong 2 giây
    public float shieldCooldown = 5f;   // chờ 5 giây mới bật lại
    private GameObject shieldInstance;
    private bool shieldReady = true;

    void Update()
    {
        PlayerMovement();
        //PlayerShoot();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryActivateShield(); // bật shield cùng lúc
        }
    }

    void PlayerMovement()
    {
        float xpos = Input.GetAxis("Horizontal");
        float ypos = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(xpos, ypos, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    void PlayerShoot()
    {
        GameObject gm = Instantiate(missile, missileSpawnPoint.position, missileSpawnPoint.rotation);
        Destroy(gm, destroyTime);
    }

    void TryActivateShield()
    {
        if (!shieldReady) return;
        StartCoroutine(ShieldRoutine());
    }

    IEnumerator ShieldRoutine()
    {
        shieldReady = false;

        // tạo 1 lần rồi tái sử dụng
        if (shieldInstance == null)
        {
            shieldInstance = Instantiate(shieldPrefab, transform);
            shieldInstance.transform.localPosition = Vector3.zero; // ôm sát player
            // nếu shield là object 3D, có thể cần chỉnh localScale cho vừa
        }
        else
        {
            shieldInstance.transform.SetParent(transform);
            shieldInstance.transform.localPosition = Vector3.zero;
        }

        shieldInstance.SetActive(true);
        yield return new WaitForSeconds(shieldDuration);

        shieldInstance.SetActive(false);
        yield return new WaitForSeconds(shieldCooldown);

        shieldReady = true;
    }

    // Game Over khi đụng Enemy (nếu không dùng shield để chặn)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            Debug.Log("Game Over");
        }
    }
}
