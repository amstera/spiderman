using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public GameObject Spiderman;

    void LateUpdate()
    {
        transform.position = Spiderman.transform.position + Vector3.up * 50;
        transform.eulerAngles = new Vector3(90, Mathf.Lerp(transform.eulerAngles.y, Spiderman.transform.eulerAngles.y, Time.deltaTime * 5), Mathf.Lerp(transform.eulerAngles.z, Spiderman.transform.eulerAngles.z, Time.deltaTime * 5));
    }
}
