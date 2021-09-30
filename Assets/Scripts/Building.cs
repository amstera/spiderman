using UnityEngine;

public class Building : MonoBehaviour
{
    public SpiderMan SpiderMan;

    private MeshRenderer _meshRenderer;

    void Start()
    {
        SpiderMan = FindObjectOfType<SpiderMan>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (SpiderMan.transform.position.y >= _meshRenderer.bounds.max.y * 0.9)
        {
            tag = "Walkable";
        }
        else
        {
            tag = "Climbable";
        }
    }
}
