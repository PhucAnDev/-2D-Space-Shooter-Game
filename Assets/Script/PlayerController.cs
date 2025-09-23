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
    public GameObject shieldPrefab;
    public float shieldDuration = 2f;
    public float shieldCooldown = 5f;

    private GameObject shieldInstance;
    private bool shieldReady = true;
    private bool isShieldActive = false;

    // cache layer ids
    int playerLayer, enemyLayer, enemyBulletLayer;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet"); // nếu chưa có thì sẽ là -1
    }

    void Update()
    {
        PlayerMovement();

        if (Input.GetKeyDown(KeyCode.Space))
            TryActivateShield();
        // nếu muốn Space vừa bắn vừa bật khiên: gọi thêm PlayerShoot();
    }

    void PlayerMovement()
    {
        float xpos = Input.GetAxis("Horizontal");
        float ypos = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(xpos, ypos, 0) * speed * Time.deltaTime);
    }

    void PlayerShoot()
    {
        var gm = Instantiate(missile, missileSpawnPoint.position, missileSpawnPoint.rotation);
        Destroy(gm, destroyTime);
    }

    void TryActivateShield()
    {
        if (!shieldReady || shieldPrefab == null) return;
        StartCoroutine(ShieldRoutine());
    }

    IEnumerator ShieldRoutine()
    {
        shieldReady = false;
        isShieldActive = true;

        if (shieldInstance == null)
        {
            shieldInstance = Instantiate(shieldPrefab, transform);
            shieldInstance.transform.localPosition = Vector3.zero;
        }
        else
        {
            shieldInstance.transform.SetParent(transform);
            shieldInstance.transform.localPosition = Vector3.zero;
        }

        shieldInstance.SetActive(true);

        // ➊ bỏ qua va chạm Player↔Enemy (và Player↔EnemyBullet nếu có)
        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        if (playerLayer >= 0 && enemyBulletLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyBulletLayer, true);

        yield return new WaitForSeconds(shieldDuration);

        // tắt khiên
        isShieldActive = false;
        shieldInstance.SetActive(false);

        // ➋ bật lại va chạm
        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        if (playerLayer >= 0 && enemyBulletLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyBulletLayer, false);

        yield return new WaitForSeconds(shieldCooldown);
        shieldReady = true;
    }

    // Nếu vì lý do nào đó va chạm vẫn lọt, phòng thủ thêm ở đây:
    void OnCollisionEnter2D(Collision2D col)
    {
        if (isShieldActive) return; // đang có khiên thì không chết
        if (col.collider.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            Debug.Log("Game Over");
        }
    }
}
