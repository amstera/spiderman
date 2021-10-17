using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public GameObject Spiderman;

    void Update()
    {
        transform.position = Spiderman.transform.position + Vector3.up * 50;
    }
}
