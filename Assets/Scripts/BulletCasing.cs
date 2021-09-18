
using UnityEngine;

public class BulletCasing : MonoBehaviour {

    public float force;
    Vector3 direction;
    Rigidbody rb;

    public void Initialize(Vector3 direction_) {
        direction = direction_;
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        rb.AddForce(force * Time.deltaTime * direction, ForceMode.Impulse);
        rb.AddTorque(Random.Range(50, 100) * Time.deltaTime * (Vector3.up + Vector3.right), ForceMode.Impulse);
        Destroy(gameObject, 5f);
    }
}
