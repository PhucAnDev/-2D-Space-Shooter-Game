using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    [Header("Missile")]
    public GameObject missile;
    public Transform missileSpawnPoint;
    public float destroyTime = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Update()
    {
        PlayerMovement();
        PlayerShoot();
    }
    public void PlayerMovement()
    {
        float xpos = Input.GetAxis("Horizontal");
        float ypos = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(xpos, ypos, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    void PlayerShoot()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            
            GameObject gm = Instantiate(missile, missileSpawnPoint); // true = giữ world position
            gm.transform.SetParent(null);
            Destroy(gm, destroyTime);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            SceneManager.LoadScene("GameOver");
        }
    }
}
