using UnityEngine;
using UnityEngine.AI;

public class GreenGoblin : MonoBehaviour
{
    public NavMeshAgent NavMeshAgent;
    public Transform NavChild;
    public GameObject Grenade;
    public float GrenadeFireIntervalSeconds = 7.5f;
    public int Health = 4;

    public AudioSource LaughAS;
    public AudioSource HurtAS;

    private SpiderMan _spiderMan;
    private float _timeSinceLastGrenade;
    private float _timeSinceLastLaugh;

    void Awake()
    {
        _spiderMan = FindObjectOfType<SpiderMan>();
        _timeSinceLastGrenade = Time.time;
    }

    void Update()
    {
        if (Time.time - _timeSinceLastLaugh > 5)
        {
            LaughAS.Play();
            _timeSinceLastLaugh = Time.time;
        }
    }

    void LateUpdate()
    {
        NavMeshAgent.destination = new Vector3(_spiderMan.transform.position.x, NavChild.position.y, _spiderMan.transform.position.z);
        var spidermanY = _spiderMan.transform.position.y;
        transform.position = new Vector3(NavChild.position.x, Mathf.Lerp(transform.position.y, spidermanY + 4f, Time.deltaTime), NavChild.position.z);
        transform.rotation = NavChild.rotation;

        if (Vector3.Distance(transform.position, _spiderMan.transform.position) <= 6f && Time.time - _timeSinceLastGrenade > GrenadeFireIntervalSeconds)
        {
            _timeSinceLastGrenade = Time.time;
            Instantiate(Grenade, transform.position - Vector3.down, transform.rotation);
        }
    }

    public void TakeDamage()
    {
        Health -= 1;
        LaughAS.Stop();
        _timeSinceLastLaugh = Time.time;
        HurtAS.Play();

        if (Health <= 0)
        {
            //die
        }
    }
}