using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject Explosion;
    public Rigidbody Rb;
    public float GrenadeForce = 10;
    public float ExplosionRadius = 5;

    private SpiderMan _spiderMan;
    private bool _hasExploded;

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
        if (_hasExploded)
        {
            return;
        }
        Instantiate(Explosion, transform.position, Quaternion.identity);
        if (Vector3.Distance(transform.position, _spiderMan.transform.position) < ExplosionRadius)
        {
            _spiderMan.TakeDamage(25);
        }
        _hasExploded = true;
        Destroy(gameObject);
    }
}