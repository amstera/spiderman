using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CheckpointFinder : MonoBehaviour
{
    public NavMeshAgent NavMeshAgent;
    public Transform Checkpoint;
    public LineRenderer LineRenderer;
    public GameObject Spiderman;
    public GameObject CheckpointObjective;
    public Text MinimapDistanceText;

    public GameObject GreenGoblin;

    void Update()
    {
        MinimapDistanceText.text = $"{Mathf.Round(Vector3.Distance(Checkpoint.position, Spiderman.transform.position))}m";
        transform.position = Spiderman.transform.position;

        NavMeshPath path = new NavMeshPath();
        NavMeshAgent.CalculatePath(Checkpoint.position, path);
        var corners = path.corners;

        LineRenderer.positionCount = corners.Length;
        for (int i = 0; i < corners.Length; i++)
        {
            LineRenderer.SetPosition(i, corners[i] + Vector3.up * 5);
        }

        if (Vector3.Distance(Spiderman.transform.position, Checkpoint.position) < 3)
        {
            MinimapDistanceText.text = "";
            Instantiate(GreenGoblin, Checkpoint.position - Vector3.back * 5, Quaternion.identity);
            Destroy(CheckpointObjective);
        }
    }
}
