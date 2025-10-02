using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    [HideInInspector] public int hitsToDie = 2;

    [Header("SFX")]
    public AudioClip hitSfx;      // tiếng trúng đạn (khi chưa chết)
    public AudioClip explodeSfx;  // tiếng nổ (khi hết máu)
    [Range(0f, 1f)] public float sfxVolume = 0.9f;
    void Update()
    {
        
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Missile"))
        {
            hitsToDie -= 1;          // trúng 1 phát
            Destroy(other.gameObject); // tên lửa luôn biến mất khi chạm

            if (hitsToDie <= 0)
            {
                if (explodeSfx)
                    AudioSource.PlayClipAtPoint(explodeSfx, Camera.main.transform.position, sfxVolume);
                Destroy(gameObject);  // Enemy chỉ chết khi đủ số hit
            }
            else
            {
                if (hitSfx)
                    AudioSource.PlayClipAtPoint(hitSfx, Camera.main.transform.position, sfxVolume);
            }
        }
    }

}
