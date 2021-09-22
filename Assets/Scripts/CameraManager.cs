using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject Target;
    public Transform CameraPivot;
    public Transform CameraTransform;

    public float CameraFollowSpeed = 0.2f;
    public float CameraLookSpeed = 2f;

    public float CameraPivotSpeed = 2;
    public float MinimumPivotAngle = -35;
    public float MaximumPivotAngle = 35;

    public float CameraCollisionRadius = 0.2f;
    public float CameraCollisionOffset = 0.2f;
    public float MinimumCollisionOffset = 0.2f;

    private float _lookAngle;
    private float _pivotAngle;
    private Vector3 _cameraFollowVelocity;
    private Vector3 _cameraVectorPosition;

    void Update()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp
            (transform.position, Target.transform.position, ref _cameraFollowVelocity, CameraFollowSpeed);
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        _lookAngle += Input.GetAxisRaw("Mouse X") * CameraLookSpeed;
        _pivotAngle -= Input.GetAxisRaw("Mouse Y") * CameraPivotSpeed;
        _pivotAngle = Mathf.Clamp(_pivotAngle, MinimumPivotAngle, MaximumPivotAngle);

        var rotation = Vector3.zero;
        rotation.y = _lookAngle;
        var targetRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), 0.1f);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = _pivotAngle;
        targetRotation = Quaternion.Lerp(CameraPivot.localRotation, Quaternion.Euler(rotation), 0.1f);
        CameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = CameraTransform.localPosition.z;
        RaycastHit hit;
        Vector3 direction = CameraTransform.position - CameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(CameraPivot.transform.position, CameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition)))
        {
            if (hit.collider == Target)
            {
                float distance = Vector3.Distance(CameraPivot.position, hit.point);
                targetPosition = -(distance - CameraCollisionOffset);
            }
        }

        if (Mathf.Abs(targetPosition) < MinimumCollisionOffset)
        {
            targetPosition -= MinimumCollisionOffset;
        }

        _cameraVectorPosition.z = Mathf.Lerp(CameraTransform.localPosition.z, targetPosition, 0.2f);
        CameraTransform.localPosition = _cameraVectorPosition;
    }
}