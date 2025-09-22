using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Update()
    {
        PlayerMovement();
    }
    public void PlayerMovement()
    {
        float xpos = Input.GetAxis("Horizontal");
        float ypos = Input.GetAxis("Vertical");

        var movement = new Vector3(xpos, ypos, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}
