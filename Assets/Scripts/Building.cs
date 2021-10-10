using UnityEngine;

public class Building : MonoBehaviour
{
    public float HeightPercent = 0.9f;

    private SpiderMan _spiderMan;
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _spiderMan = FindObjectOfType<SpiderMan>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (_spiderMan.transform.position.y >= _meshRenderer.bounds.max.y * HeightPercent)
        {
            tag = "Walkable";
        }
        else
        {
            tag = "Climbable";
        }
    }
}
