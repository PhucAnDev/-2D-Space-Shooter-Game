using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundController : MonoBehaviour
{
    public float scrollSpeed = 2f;
    float height;

    void Start()
    {
        height = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);
        if (transform.position.y <= -height)
            transform.position += new Vector3(0, height * 2f, 0);
    }
}
