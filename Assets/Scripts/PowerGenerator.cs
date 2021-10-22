using TMPro;
using UnityEngine;

public class PowerGenerator : MonoBehaviour
{
    public GameObject Spiderman;
    public GameObject Explosion;
    public GameObject CheckpointMarker;
    public GameObject Lightbeam;
    public TextMeshProUGUI GeneratorControlText;

    public GreenGoblin GreenGoblin;
    public bool IsActive;

    private bool _showingControlText;

    void Start()
    {
        GeneratorControlText = FindObjectOfType<TextMeshProUGUI>();
    }

    public void Activate()
    {
        IsActive = true;
        CheckpointMarker.SetActive(true);
        Lightbeam.SetActive(true);
    }

    void Update()
    {
        if (IsActive && Vector3.Distance(Spiderman.transform.position, transform.position) < 2f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GeneratorControlText.enabled = false;

                GreenGoblin.TakeDamage();
                FindObjectOfType<ObjectivePanel>().UpdateCount();
                Instantiate(Explosion, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }

            if (!_showingControlText)
            {
                _showingControlText = true;
                GeneratorControlText.enabled = true;
            }
        }
        else if (_showingControlText)
        {
            _showingControlText = false;
            GeneratorControlText.enabled = false;
        }
    }
}
