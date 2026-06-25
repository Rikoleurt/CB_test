using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private SplineFollower _posFollower;
    [SerializeField] private Transform _pivotTransform;
    [SerializeField] private CameraLook _camLook;
    [SerializeField] private CameraProfile _baseProfile;
    [ShowNonSerializedField] CameraProfile _currentProfile;

    private Camera _camera;
    private Controller _controller;
    private StateMachine _stateMachine;
    private bool _cameraBlocked;

    [ShowNonSerializedField] private float angle;
    [ShowNonSerializedField] private float height = .5f;

    void Start()
    {
        _currentProfile = _baseProfile;
        _camera = GetComponent<Camera>();
        _controller = Controller.Instance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (_currentProfile._canPlayerControlCamera)
        {
            UpdateInput();
            UpdateCameraRig();
        }

        Vector3 desiredPosition = _posFollower.transform.position;
        Vector3 safePosition = ResolveBlockedPosition(desiredPosition);
        Quaternion desiredRotation = GetLookRotationFromPosition(safePosition);
        ApplySmoothTransform(safePosition, desiredRotation);
    }

    public void UpdateWallRunCamera(Vector3 wallNormal, Vector3 forward, Vector3 up)
    {
        Vector3 origin = _pivotTransform.position;

        wallNormal.Normalize();
        forward.Normalize();
        up.Normalize();

        Vector3 cameraLook =
                wallNormal * _currentProfile.CameraLookOffets.x
                + forward * _currentProfile.CameraLookOffets.z
                + up * _currentProfile.CameraLookOffets.y
            ;

        Vector3 cameraPosition =
                wallNormal * _currentProfile.CameraPosOffets.x
                + -forward * _currentProfile.CameraPosOffets.z
                + up * _currentProfile.CameraPosOffets.y
            ;

        _camLook.transform.position = origin + cameraLook;
        _posFollower.transform.position = origin + cameraPosition;


        Vector3 flatForward = Vector3.ProjectOnPlane(forward, Vector3.up);
        _pivotTransform.rotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
        angle = _pivotTransform.eulerAngles.y;
    }

    private void UpdateInput()
    {
        Vector2 deltaLook = _controller.DeltaLook;

        angle += _currentProfile._sensitivityAngle * 100f * deltaLook.x;
        angle %= 360f;

        height += _currentProfile._sensitivityHeight * deltaLook.y;
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
        float rotT = 1f - Mathf.Exp(-_currentProfile._rotationSmoothSpeed * Time.deltaTime);

        if (_cameraBlocked)
        {
            transform.position = desiredPosition;
        }
        else
        {
            float moveT = 1f - Mathf.Exp(-_currentProfile._moveSmoothSpeed * Time.deltaTime);
            transform.position = Vector3.Slerp(transform.position, desiredPosition, moveT);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotT);

        FOVSmoothing();
    }

    private void FOVSmoothing()
    {
        if (
            _camera.fieldOfView != _currentProfile.FOV
            && Mathf.Abs(_camera.fieldOfView - _currentProfile.FOV) > float.Epsilon
        )
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _currentProfile.FOV, .4f);
        }
        else
        {
            _camera.fieldOfView = _currentProfile.FOV;
        } 
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
            _currentProfile._collisionRadius,
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

    public void SetCameraProfile(CameraProfile newProfile)
    {
        _currentProfile = newProfile;
    }

    public void SetBaseCameraProfile()
    {
        _currentProfile = _baseProfile;
    }
    
    void OnDrawGizmos()
    {
        if (_currentProfile == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _currentProfile._collisionRadius);
    }
    
}