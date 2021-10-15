using UnityEngine;
using UnityEngine.AI;

public class GreenGoblin : MonoBehaviour
{
    public NavMeshAgent NavMeshAgent;
    public Transform NavChild;
    public GameObject Grenade;
    public float GrenadeFireIntervalSeconds = 7.5f;

    private SpiderMan _spiderMan;
    private float _timeSinceLastGrenade;

    void Awake()
    {
        _spiderMan = FindObjectOfType<SpiderMan>();
        _timeSinceLastGrenade = Time.time;
    }

    void LateUpdate()
    {
        NavMeshAgent.destination = new Vector3(_spiderMan.transform.position.x, NavChild.position.y, _spiderMan.transform.position.z);
        var spidermanY = _spiderMan.transform.position.y;
        transform.position = new Vector3(NavChild.position.x, Mathf.Lerp(transform.position.y, spidermanY + 4, Time.deltaTime), NavChild.position.z);
        transform.rotation = NavChild.rotation;

        if (Time.time - _timeSinceLastGrenade > GrenadeFireIntervalSeconds)
        {
            _timeSinceLastGrenade = Time.time;
            Instantiate(Grenade, transform.position - Vector3.down, transform.rotation);
        }
    }
}