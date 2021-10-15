using UnityEngine;
using UnityEngine.AI;

public class GreenGoblin : MonoBehaviour
{
    public NavMeshAgent NavMeshAgent;
    public Transform NavChild;

    private SpiderMan _spiderMan;

    void Awake()
    {
        _spiderMan = FindObjectOfType<SpiderMan>();
    }

    void LateUpdate()
    {
        NavMeshAgent.destination = new Vector3(_spiderMan.transform.position.x, NavChild.position.y, _spiderMan.transform.position.z);
        var spidermanY = _spiderMan.transform.position.y;
        transform.position = new Vector3(NavChild.position.x, Mathf.Lerp(transform.position.y, spidermanY + 4, Time.deltaTime), NavChild.position.z);
        transform.rotation = NavChild.rotation;
    }
}