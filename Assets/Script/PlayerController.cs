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

    public float moveSpeed = 7f;
    private bool isBoosting = false;
    public float boostMultiplier = 2f;
    private Rigidbody2D rb;

    // cache layer ids
    int playerLayer, enemyLayer, enemyBulletLayer;


    [Header("Fuel System")]
    public float maxFuel = 100f;
    public float fuelConsumptionRate = 5f; // Nhiên liệu tiêu thụ mỗi giây
    public float currentFuel;
    public UnityEngine.UI.Slider fuelSlider; // Tham chiếu đến Slider UI hiển thị nhiên liệu

    void Start()
    {
        currentFuel = maxFuel; // Đặt nhiên liệu đầy khi bắt đầu
        if (fuelSlider != null)
        {
            fuelSlider.maxValue = 1f; // Giá trị tối đa của slider
            fuelSlider.value = 1f;   // Đặt slider đầy
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        rb = GetComponent<Rigidbody2D>();
        currentFuel = maxFuel;

        // Đảm bảo có Rigidbody2D
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // Không có trọng lực trong không gian
        }
    }

    void Update()
    {

        ConsumeFuel();

        if (currentFuel <= 0)
        {
            TriggerGameOver();
        }

        if (Input.GetKeyDown(KeyCode.Space))
            TryActivateShield();
    }

    void FixedUpdate()
    {
        PlayerMovement();
    }

    void ConsumeFuel()
    {
        currentFuel -= fuelConsumptionRate * Time.deltaTime;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);

        UpdateFuelSlider();
    }

    public void RefillFuel(float amount)
    {
        currentFuel += amount;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);

        UpdateFuelSlider();
    }

    void UpdateFuelSlider()
    {
        if (fuelSlider != null)
        {
            float targetValue = currentFuel / maxFuel;
            fuelSlider.value = targetValue;

            // Kiểm tra nếu năng lượng cạn kiệt
            if (targetValue <= 0)
            {
                // Ẩn Fill Area
                Transform fillArea = fuelSlider.transform.Find("Fill Area");
                if (fillArea != null)
                {
                    fillArea.gameObject.SetActive(false);
                }
            }
        }
    }


    void TriggerGameOver()
    {
        var scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.OnGameOver(); // Save the score
        }
        SceneManager.LoadScene("GameOverScene");
    }

    void PlayerMovement()
    {
        // Lấy input từ các phím mũi tên hoặc WASD
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Tạo vector di chuyển
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // Chuẩn hóa vector để di chuyển với tốc độ ổn định theo mọi hướng
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Áp dụng tốc độ
        float currentSpeed = moveSpeed;
        if (isBoosting)
        {
            currentSpeed *= boostMultiplier;
        }

        movement *= currentSpeed;

        // Di chuyển spaceship
        rb.linearVelocity = movement;

        // Xoay spaceship theo hướng di chuyển (tuỳ chọn)
        if (movement != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet"); // nếu chưa có thì sẽ là -1

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
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
        if (col.CompareTag("EnergyOrb"))
        {
            RefillFuel(20f); // Tăng nhiên liệu (giá trị có thể thay đổi)

            if (coinPickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(coinPickSound);
            }

            Destroy(col.gameObject); // Xóa EnergyOrb sau khi thu thập
            return;
        }

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

        if ((col.CompareTag("Enemy")) && !isShieldActive)
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

