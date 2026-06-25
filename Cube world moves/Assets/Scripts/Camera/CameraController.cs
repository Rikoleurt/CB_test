using NaughtyAttributes;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] CameraProfile _baseProfile;
    [ShowNonSerializedField] CameraProfile _actualProfile;

    Camera _camera;

    [SerializeField] private SplineFollower _posFollower;
    [SerializeField] private Transform _pivotTransform;
    [SerializeField] private CameraLook _camLook;



    private Controller _controller;
    private StateMachine _stateMachine;
    
    [ShowNonSerializedField] private float angle;
    [ShowNonSerializedField] private float height = .5f;

    private bool _cameraBlocked;
    private bool isWallRunning;
    void Start()
    {
        _actualProfile = _baseProfile;
        _camera = GetComponent<Camera>();
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

    public void UpdateWallRunCamera(Vector3 wallNormal, Vector3 forward, Vector3 up)
    {
        if (!isWallRunning)
        {
            return;
        }

        Vector3 origin = _pivotTransform.position;

        wallNormal.Normalize();
        forward.Normalize();
        up.Normalize();

        Vector3 cameraLook =
                wallNormal * _actualProfile.CameraLookOffets.x
                + forward * _actualProfile.CameraLookOffets.z
                + up* _actualProfile.CameraLookOffets.y
            ;
        
        Vector3 cameraPosition = 
                wallNormal * _actualProfile.CameraPosOffets.x
                + -forward * _actualProfile.CameraPosOffets.z
                + up * _actualProfile.CameraPosOffets.y
            ;

         _camLook.transform.position = origin + cameraLook;
        _posFollower.transform.position = origin + cameraPosition;
    }

    public void SetIsWallRunning(bool value)
    {
        isWallRunning = value;

        if (!value)
        {
            _camLook.transform.localPosition = Vector3.zero;
        }
    }
    public void SetCameraProfile(CameraProfile _newProfile)
    {
        _actualProfile = _newProfile;
    }
    public void SetBaseCameraProfile()
    {
         _actualProfile = _baseProfile;
    }

    private void UpdateInput()
    {
        Vector2 deltaLook = _controller.DeltaLook;

        angle += _actualProfile._sensitivityAngle * 100f * deltaLook.x;
        angle %= 360f;

        height += _actualProfile._sensitivityHeight * deltaLook.y;
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
        float rotT = 1f - Mathf.Exp(-_actualProfile._rotationSmoothSpeed * Time.deltaTime);

        if (_cameraBlocked)
        {
            transform.position = desiredPosition;
        }
        else
        {
            float moveT = 1f - Mathf.Exp(-_actualProfile._moveSmoothSpeed * Time.deltaTime);
            transform.position = Vector3.Slerp(transform.position, desiredPosition, moveT);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotT);

        if(_camera.fieldOfView !=_actualProfile.FOV && Mathf.Abs(_camera.fieldOfView-_actualProfile.FOV) > float.Epsilon)
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView,_actualProfile.FOV,.4f);
        }
        else
        {
            _camera.fieldOfView =_actualProfile.FOV;
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
            _actualProfile._collisionRadius,
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
        if(_actualProfile == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _actualProfile._collisionRadius);
    }
}