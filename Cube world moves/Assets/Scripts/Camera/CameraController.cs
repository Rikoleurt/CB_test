using NaughtyAttributes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private SplineFollower _posFollower;
    [SerializeField] private Transform _pivotTransform;
    [SerializeField] private CameraLook _camLook;

    [SerializeField] private float _sensitivityAngle = 1f;
    [SerializeField] private float _sensitivityHeight = 1f;

    [SerializeField] private float _moveSmoothSpeed = 12f;
    [SerializeField] private float _rotationSmoothSpeed = 15f;

    [SerializeField] private float _collisionRadius = 0.25f;

    private Controller _controller;
    private StateMachine _stateMachine;
    
    [ShowNonSerializedField] private float angle;
    [ShowNonSerializedField] private float height = .5f;

    private bool _cameraBlocked;
    private bool isWallRunning;
    void Start()
    {
        _controller = Controller.Instance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (!isWallRunning)
        {
            UpdateInput();
            UpdateCameraRig();
        }
        
        Vector3 desiredPosition = _posFollower.transform.position;
        Vector3 safePosition = ResolveBlockedPosition(desiredPosition);
        Quaternion desiredRotation = GetLookRotationFromPosition(safePosition);
        ApplySmoothTransform(safePosition, desiredRotation);
    }

    public void UpdateWallRunCamera(Vector3 cameraPosition, Vector3 cameraLook)
    {
        if (!isWallRunning)
        {
            return;
        }

        _camLook.transform.position = cameraLook;
        _posFollower.transform.position = cameraPosition;
    }

    public void SetIsWallRunning(bool value)
    {
        isWallRunning = value;

        if (!value)
        {
            _camLook.transform.localPosition = Vector3.zero;
            Debug.Break();
        }
    }

    private void UpdateInput()
    {
        Vector2 deltaLook = _controller.DeltaLook;

        angle += _sensitivityAngle * 100f * deltaLook.x;
        angle %= 360f;

        height += _sensitivityHeight * deltaLook.y;
        height = Mathf.Clamp01(height);
    }

    private void UpdateCameraRig()
    {
        _pivotTransform.eulerAngles = new Vector3(0f, angle, 0f);
        _posFollower.UpdatePos(height);
    }

    private Quaternion GetLookRotationFromPosition(Vector3 cameraPosition)
    {
        Vector3 lookDirection = _camLook.transform.position - cameraPosition;

        if (lookDirection.sqrMagnitude < 0.0001f)
        {
            return transform.rotation;
        }

        return Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
    }

    private void ApplySmoothTransform(Vector3 desiredPosition, Quaternion desiredRotation)
    {
        float rotT = 1f - Mathf.Exp(-_rotationSmoothSpeed * Time.deltaTime);

        if (_cameraBlocked)
        {
            transform.position = desiredPosition;
        }
        else
        {
            float moveT = 1f - Mathf.Exp(-_moveSmoothSpeed * Time.deltaTime);
            transform.position = Vector3.Slerp(transform.position, desiredPosition, moveT);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotT);
    }

    private Vector3 ResolveBlockedPosition(Vector3 desiredPosition)
    {
        _cameraBlocked = false;

        Vector3 origin = _camLook.transform.position;
        Vector3 toCamera = desiredPosition - origin;

        float distance = toCamera.magnitude;

        if (distance < 0.0001f)
        {
            return desiredPosition;
        }

        Vector3 direction = toCamera / distance;

        bool blocked = Physics.SphereCast(
            origin,
            _collisionRadius,
            direction,
            out RaycastHit hit,
            distance
        );

        if (!blocked)
        {
            return desiredPosition;
        }

        _cameraBlocked = true;

        float safeDistance = Mathf.Max(hit.distance, 0f);

        return origin + direction * safeDistance;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _collisionRadius);
    }
}