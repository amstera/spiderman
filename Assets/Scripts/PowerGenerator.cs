using UnityEngine;

public class PowerGenerator : MonoBehaviour
{
    public GameObject Spiderman;
    public GameObject Explosion;
    public GameObject CheckpointMarker;

    public GreenGoblin GreenGoblin;
    public bool IsActive;

    public void Activate()
    {
        IsActive = true;
        CheckpointMarker.SetActive(true);
    }

    void Update()
    {
        if (IsActive && Vector3.Distance(Spiderman.transform.position, transform.position) < 2f && Input.GetKeyDown(KeyCode.E))
        {
            GreenGoblin.TakeDamage();
            Instantiate(Explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
