using NaughtyAttributes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private SplineFollower _posFollower;
    [SerializeField] private Transform _pivotTransform;
    [SerializeField] private Transform _lookTransform;

    [SerializeField] private float _sensitivityAngle = 1f;
    [SerializeField] private float _sensitivityHeight = 1f;

    [SerializeField] private float _moveSmoothSpeed = 12f;
    [SerializeField] private float _rotationSmoothSpeed = 15f;

    [SerializeField] private float _boxHalfExtents;
    [SerializeField] private LayerMask _collisionMask = ~0;

    private Controller _controller;

    [ShowNonSerializedField] private float angle;
    [ShowNonSerializedField] private float height = .5f;

    void Start()
    {
        _controller = Controller.Instance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        UpdateInput();
        UpdateCameraRig();

        Vector3 desiredPosition = _posFollower.transform.position;

        desiredPosition = ResolveBlockedPosition(desiredPosition);

        Quaternion desiredRotation = GetLookRotation();

        ApplySmoothTransform(desiredPosition, desiredRotation);
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

    private Quaternion GetLookRotation()
    {
        Vector3 lookDirection = _lookTransform.position - transform.position;

        if (lookDirection.sqrMagnitude < 0.0001f)
        {
            return transform.rotation;
        }

        return Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
    }

    private void ApplySmoothTransform(Vector3 desiredPosition, Quaternion desiredRotation)
    {
        float moveT = 1f - Mathf.Exp(-_moveSmoothSpeed * Time.fixedDeltaTime);
        float rotT = 1f - Mathf.Exp(-_rotationSmoothSpeed * Time.fixedDeltaTime);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, moveT);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotT);
    }

    private Vector3 ResolveBlockedPosition(Vector3 desiredPosition)
    {
        Vector3 startPosition = transform.position;
        Vector3 move = desiredPosition - startPosition;

        float distance = move.magnitude;

        if (distance < 0.0001f)
        {
            return desiredPosition;
        }

        Vector3 direction = move / distance;

        bool blocked = Physics.BoxCast(
            startPosition,
            _boxHalfExtents * Vector3.one,
            direction,
            out RaycastHit hit,
            transform.rotation,
            distance,
            _collisionMask,
            QueryTriggerInteraction.Ignore
        );

        if (!blocked)
        {
            return desiredPosition;
        }

        float skin = 0.03f;

        float safeDistance = Mathf.Max(hit.distance - skin, 0f);
        Vector3 safePosition = startPosition + direction * safeDistance;

        Vector3 remainingMove = desiredPosition - safePosition;

        Vector3 slideMove = Vector3.ProjectOnPlane(remainingMove, hit.normal);

        if (slideMove.sqrMagnitude < 0.0001f) return safePosition;

        Vector3 slideDirection = slideMove.normalized;
        float slideDistance = slideMove.magnitude;

        bool slideBlocked = Physics.BoxCast(
            safePosition,
            _boxHalfExtents * Vector3.one,
            slideDirection,
            out RaycastHit slideHit,
            transform.rotation,
            slideDistance,
            _collisionMask,
            QueryTriggerInteraction.Ignore
        );

        if (slideBlocked)
        {
            float safeSlideDistance = Mathf.Max(slideHit.distance - skin, 0f);
            return safePosition + slideDirection * safeSlideDistance;
        }

        return safePosition + slideMove;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _boxHalfExtents * Vector3.one * 2f);
    }
}