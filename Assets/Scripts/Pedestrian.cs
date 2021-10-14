using UnityEngine;

public class Pedestrian : MonoBehaviour
{
    private Vector3 _startPos;
    private Vector3 _endPos;
    private Vector3 _desPos;

    void Start()
    {
        _startPos = transform.position;
        _endPos = _startPos + transform.forward * 85;
        _desPos = _endPos;
    }

    void Update()
    {
        transform.LookAt(new Vector3(_desPos.x, transform.position.y, _desPos.z));
        transform.position += transform.forward * 5 * Time.deltaTime;

        if (Vector3.Distance(transform.position, _desPos) < 2f)
        {
            if (_desPos == _endPos)
            {
                _desPos = _startPos;
            }
            else
            {
                _desPos = _endPos;
            }
        }
    }
}
