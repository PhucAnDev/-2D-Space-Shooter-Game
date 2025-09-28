using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float lastShieldTime = -999f;
    private float shieldLastUsedTime;

    [Header("Sound")]
    public AudioClip impactSound;   // thêm clip khi va chạm
    public AudioClip coinPickSound;
    private AudioSource audioSource;


    // cache layer ids
    int playerLayer, enemyLayer, enemyBulletLayer;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet"); // nếu chưa có thì sẽ là -1

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
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

        // Đợi khiên chạy xong
        yield return new WaitForSeconds(shieldDuration);

        // ➋ Tắt khiên
        isShieldActive = false;
        shieldInstance.SetActive(false);

        // Lưu thời điểm bắt đầu cooldown
        shieldLastUsedTime = Time.time;

        // Bật lại va chạm
        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        if (playerLayer >= 0 && enemyBulletLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyBulletLayer, false);

        // Đợi cooldown
        yield return new WaitForSeconds(shieldCooldown);
        shieldReady = true;
    }

    // Nếu vì lý do nào đó va chạm vẫn lọt, phòng thủ thêm ở đây:
    void OnCollisionEnter2D(Collision2D col)
    {
        if (isShieldActive) return; // đang có khiên thì không chết
        if (col.collider.CompareTag("Enemy"))
        {
            if (impactSound != null && audioSource != null)
                audioSource.PlayOneShot(impactSound);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            float delay = impactSound != null ? impactSound.length : 0.5f;
            StartCoroutine(LoadGameOverAfterDelay(delay));
        }
    }


    IEnumerator LoadGameOverAfterDelay(float delay)
    {
        var sm = FindObjectOfType<ScoreManager>();
        if (sm != null)
            sm.OnGameOver();
        else
            SceneManager.LoadScene("GameOverScene");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GameOverScene");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Coin"))
        {
            if (audioSource != null && coinPickSound != null)
                audioSource.PlayOneShot(coinPickSound);
            Destroy(col.gameObject);

            // cộng điểm
            ScoreManager score = FindObjectOfType<ScoreManager>();
            if (score != null)
            {
                score.AddPoints(5);
            }
            return;
        }

        if ((col.CompareTag("Enemy") || col.CompareTag("EnemyBullet")) && !isShieldActive)
        {
            var sm = FindObjectOfType<ScoreManager>();
            if (sm != null)
                sm.OnGameOver();
            else
                SceneManager.LoadScene("GameOverScene");
        }
    }


    public bool IsShieldReady() => shieldReady;




    public float GetShieldCooldownProgress()
    {
        if (shieldReady) return 1f; // đã sẵn sàng
        float elapsed = Time.time - shieldLastUsedTime;
        return Mathf.Clamp01(elapsed / shieldCooldown);
    }
}

