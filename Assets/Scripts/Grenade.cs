using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject Explosion;
    public Rigidbody Rb;
    public float GrenadeForce = 10;

    private SpiderMan _spiderMan;

    void Start()
    {
        _spiderMan = FindObjectOfType<SpiderMan>();
        transform.LookAt(_spiderMan.transform.position);
        Rb.AddForce(transform.forward * GrenadeForce, ForceMode.Impulse);
        Physics.IgnoreCollision(GetComponent<BoxCollider>(), FindObjectOfType<GreenGoblin>().GetComponent<BoxCollider>());
        Destroy(gameObject, 10);
    }

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}