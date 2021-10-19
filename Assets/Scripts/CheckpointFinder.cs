using UnityEngine;
using UnityEngine.AI;

public class CheckpointFinder : MonoBehaviour
{
    public NavMeshAgent NavMeshAgent;
    public Transform Checkpoint;
    public LineRenderer LineRenderer;
    public GameObject Spiderman;
    public GameObject CheckpointObjective;

    public GameObject GreenGoblin;

    void Update()
    {
        transform.position = Spiderman.transform.position;

        NavMeshPath path = new NavMeshPath();
        NavMeshAgent.CalculatePath(Checkpoint.position, path);
        var corners = path.corners;

        LineRenderer.positionCount = corners.Length;
        for (int i = 0; i < corners.Length; i++)
        {
            LineRenderer.SetPosition(i, corners[i] + Vector3.up * 5);
        }

        if (Vector3.Distance(Spiderman.transform.position, Checkpoint.position) < 2)
        {
            Instantiate(GreenGoblin, Checkpoint.position - Vector3.back * 5, Quaternion.identity);
            Destroy(CheckpointObjective);
        }
    }
}
