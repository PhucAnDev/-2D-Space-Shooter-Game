using UnityEngine;

public class MissileController : MonoBehaviour
{
    public float missileSpeed = 25f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * missileSpeed * Time.deltaTime);
    }
}
