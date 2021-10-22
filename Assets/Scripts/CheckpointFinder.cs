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
    public CutScenePlayer CutScenePlayer;
    public GameObject ObjectivePanel;

    public GameObject GreenGoblin;

    private bool _waitForVidToFinish;
    private float _timeSinceVideo;

    void Update()
    {
        MinimapDistanceText.text = $"{Mathf.Round(Vector3.Distance(Checkpoint.position, Spiderman.transform.position))}m";
        transform.position = new Vector3(Spiderman.transform.position.x, transform.position.y, Spiderman.transform.position.z);

        NavMeshPath path = new NavMeshPath();
        NavMeshAgent.CalculatePath(Checkpoint.position, path);
        var corners = path.corners;

        LineRenderer.positionCount = corners.Length;
        for (int i = 0; i < corners.Length; i++)
        {
            LineRenderer.SetPosition(i, corners[i] + Vector3.up * 5);
        }

        if (_waitForVidToFinish)
        {
            if (!CutScenePlayer.VideoPlayer.isPlaying && Time.time - _timeSinceVideo > 1)
            {
                MinimapDistanceText.text = "";
                var goblin = Instantiate(GreenGoblin, Checkpoint.position, Quaternion.identity);
                var powerGenerators = FindObjectsOfType<PowerGenerator>();
                foreach (PowerGenerator powerGenerator in powerGenerators)
                {
                    powerGenerator.Activate();
                    powerGenerator.GreenGoblin = goblin.GetComponentInChildren<GreenGoblin>();
                }
                ObjectivePanel.SetActive(true);
                Spiderman.transform.position = new Vector3(80, 0, 85);
                Destroy(CheckpointObjective);
            }
        }
        else
        {
            if (Vector3.Distance(Spiderman.transform.position, Checkpoint.position) < 5)
            {
                CutScenePlayer.PlayGoblinClip();
                _waitForVidToFinish = true;
                _timeSinceVideo = Time.time;
            }
        }
    }
}
