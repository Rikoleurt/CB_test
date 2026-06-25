using NaughtyAttributes;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    [Header("Velocity")]
    [SerializeField] private Vector3 _acceleration;
    [SerializeField] private float _gravity = 30f;
    [SerializeField] private float _gravityFactor = 1f;
    [SerializeField, Range(0f, 1f)] private float groundFriction = 0.15f;
    [SerializeField] private float stopThreshold = 0.05f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField] private float capsuleHeight = 2f;
    [SerializeField] private float capsuleRadius = 0.45f;
    [SerializeField] private Vector3 capsuleCenterOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private float skinWidth = 0.03f;
    [SerializeField] private int maxSlideIterations = 3;

    [Header("Ground")]
    [SerializeField] private float groundCheckDistance = 0.08f;
    [SerializeField] private float maxGroundAngle = 60f;

    [Header("Wall Sensors")]
    [Tooltip("Extra distance from the capsule surface used to detect walls for wall-running.")]
    [SerializeField] private float wallCheckDistance = 0.25f;

    [Tooltip("How vertical a wall normal is allowed to be. Lower = stricter wall detection.")]
    [SerializeField] private float maxWallNormalY = 0.25f;

    private MeshModelController _meshModel;

    public Vector3 Acceleration => _acceleration;

    #region Wall / Ground Data

    private bool _isGrounded;

    private RaycastHit _wallLeft;
    private RaycastHit _wallRight;
    private RaycastHit _wallUp;
    private RaycastHit _wallDown;
    private RaycastHit _wallFront;
    private RaycastHit _wallBack;

    public RaycastHit WallLeft => _wallLeft;
    public RaycastHit WallRight => _wallRight;
    public RaycastHit WallUp => _wallUp;
    public RaycastHit WallDown => _wallDown;
    public RaycastHit WallFront => _wallFront;
    public RaycastHit WallBack => _wallBack;

    public RaycastHit WallSide => isWallRight ? _wallRight : _wallLeft;

    public bool isWallDown => _isGrounded;
    public bool isWallFront => _wallFront.collider;
    public bool isWallBack => _wallBack.collider;
    public bool isWallLeft => _wallLeft.collider;
    public bool isWallRight => _wallRight.collider;
    public bool isWallUp => _wallUp.collider;

    public bool isWallSide => isWallLeft || isWallRight;

    #endregion

    private void Awake()
    {
        _meshModel = GetComponentInChildren<MeshModelController>();
    }

    private void FixedUpdate()
    {
        CheckGround();

        HandleVelocity();

        Vector3 movement = _acceleration * Time.fixedDeltaTime;

        MoveAndSlide(movement);

        CheckGround();
        CheckWalls();
    }

    #region Velocity

    private void HandleVelocity()
    {
        if (_isGrounded)
        {
            ApplyGroundFriction();

            if (_acceleration.y < 0f)
                _acceleration.y = 0f;
        }
        else
        {
            _acceleration.y -= _gravity * _gravityFactor * Time.fixedDeltaTime;
        }
    }

    private void ApplyGroundFriction()
    {
        Vector3 horizontalVelocity = new Vector3(_acceleration.x, 0f, _acceleration.z);

        horizontalVelocity = Vector3.Lerp(
            horizontalVelocity,
            Vector3.zero,
            groundFriction
        );

        _acceleration.x = horizontalVelocity.x;
        _acceleration.z = horizontalVelocity.z;

        if (_acceleration.magnitude < stopThreshold)
        {
            _acceleration = Vector3.zero;
        }
    }

    public void SetAcceleration(Vector3 newAcceleration)
    {
        _acceleration = newAcceleration;
    }

    public void AddAcceleration(Vector3 accelerationVector)
    {
        _acceleration += accelerationVector;
    }

    public void SetFactorGravity(float newGravityFactor)
    {
        _gravityFactor = newGravityFactor;
    }

    #endregion

    #region Collision Movement

    private void MoveAndSlide(Vector3 movement)
    {
        for (int i = 0; i < maxSlideIterations; i++)
        {
            if (movement.sqrMagnitude <= 0.000001f)
                break;

            GetCapsulePoints(out Vector3 top, out Vector3 bottom);

            Vector3 direction = movement.normalized;
            float distance = movement.magnitude;

            float castRadius = GetCastRadius();

            bool hitSomething = Physics.CapsuleCast(
                top,
                bottom,
                castRadius,
                direction,
                out RaycastHit hit,
                distance + skinWidth,
                collisionMask,
                QueryTriggerInteraction.Ignore
            );

            if (!hitSomething)
            {
                transform.position += movement;
                break;
            }

            float moveDistance = Mathf.Max(hit.distance - skinWidth, 0f);

            transform.position += direction * moveDistance;

            Vector3 remainingMovement = movement - direction * moveDistance;

            // Slide along the surface instead of stopping completely
            movement = Vector3.ProjectOnPlane(remainingMovement, hit.normal);

            // Remove velocity going into the surface
            if (Vector3.Dot(_acceleration, hit.normal) < 0f)
            {
                _acceleration = Vector3.ProjectOnPlane(_acceleration, hit.normal);
            }
        }
    }

    private void GetCapsulePoints(out Vector3 top, out Vector3 bottom)
    {
        Vector3 center = transform.position + capsuleCenterOffset;

        float halfHeight = Mathf.Max(capsuleHeight * 0.5f - capsuleRadius, 0f);

        top = center + Vector3.up * halfHeight;
        bottom = center - Vector3.up * halfHeight;
    }

    private float GetCastRadius()
    {
        return Mathf.Max(capsuleRadius - skinWidth, 0.01f);
    }

    #endregion

    #region Ground And Wall Checks

    private void CheckGround()
    {
        _wallDown = new RaycastHit();
        _isGrounded = false;

        GetCapsulePoints(out Vector3 top, out Vector3 bottom);

        float castRadius = GetCastRadius();
        float castDistance = groundCheckDistance + skinWidth;

        bool hitGround = Physics.SphereCast(
            bottom,
            castRadius,
            Vector3.down,
            out RaycastHit hit,
            castDistance,
            collisionMask,
            QueryTriggerInteraction.Ignore
        );

        if (!hitGround)
            return;

        float groundAngle = Vector3.Angle(hit.normal, Vector3.up);

        if (groundAngle > maxGroundAngle)
            return;

        _isGrounded = true;
        _wallDown = hit;
    }

    private void CheckWalls()
    {
        _wallLeft = new RaycastHit();
        _wallRight = new RaycastHit();
        _wallFront = new RaycastHit();
        _wallBack = new RaycastHit();
        _wallUp = new RaycastHit();

        if (_meshModel == null)
            return;

        Transform meshTransform = _meshModel.transform;

        CastWall(meshTransform.right, out _wallRight);
        CastWall(-meshTransform.right, out _wallLeft);
        CastWall(meshTransform.forward, out _wallFront);
        CastWall(-meshTransform.forward, out _wallBack);

        CheckCeiling(meshTransform.up);
    }

    private bool CastWall(Vector3 direction, out RaycastHit wallHit)
    {
        wallHit = new RaycastHit();

        GetCapsulePoints(out Vector3 top, out Vector3 bottom);

        float castRadius = GetCastRadius();
        float castDistance = wallCheckDistance + skinWidth;

        bool hitSomething = Physics.CapsuleCast(
            top,
            bottom,
            castRadius,
            direction.normalized,
            out RaycastHit hit,
            castDistance,
            collisionMask,
            QueryTriggerInteraction.Ignore
        );

        if (!hitSomething)
            return false;

        if (Mathf.Abs(hit.normal.y) > maxWallNormalY)
            return false;

        wallHit = hit;
        return true;
    }

    private void CheckCeiling(Vector3 upDirection)
    {
        GetCapsulePoints(out Vector3 top, out Vector3 bottom);

        float castRadius = GetCastRadius();
        float castDistance = wallCheckDistance + skinWidth;

        Physics.SphereCast(
            top,
            castRadius,
            upDirection.normalized,
            out _wallUp,
            castDistance,
            collisionMask,
            QueryTriggerInteraction.Ignore
        );
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (_meshModel == null)
            _meshModel = GetComponentInChildren<MeshModelController>();

        DrawCapsuleGizmos();
        DrawCollisionCheckGizmos();
    }

    private void DrawCapsuleGizmos()
    {
        GetCapsulePoints(out Vector3 top, out Vector3 bottom);

        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(top, capsuleRadius);
        Gizmos.DrawWireSphere(bottom, capsuleRadius);

        Gizmos.DrawLine(
            top + Vector3.forward * capsuleRadius,
            bottom + Vector3.forward * capsuleRadius
        );

        Gizmos.DrawLine(
            top - Vector3.forward * capsuleRadius,
            bottom - Vector3.forward * capsuleRadius
        );

        Gizmos.DrawLine(
            top + Vector3.right * capsuleRadius,
            bottom + Vector3.right * capsuleRadius
        );

        Gizmos.DrawLine(
            top - Vector3.right * capsuleRadius,
            bottom - Vector3.right * capsuleRadius
        );
    }

    private void DrawCollisionCheckGizmos()
    {
        if (_meshModel == null)
            return;

        GetCapsulePoints(out Vector3 top, out Vector3 bottom);

        Transform meshTransform = _meshModel.transform;

        float castRadius = GetCastRadius();
        float wallCastDistance = wallCheckDistance + skinWidth;
        float groundCastDistance = groundCheckDistance + skinWidth;

        // Ground
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(bottom, bottom + Vector3.down * groundCastDistance);
        Gizmos.DrawWireSphere(bottom + Vector3.down * groundCastDistance, castRadius);

        // Right wall
        Gizmos.color = Color.red;
        DrawCapsuleCastGizmo(top, bottom, castRadius, meshTransform.right, wallCastDistance);

        // Left wall
        Gizmos.color = Color.magenta;
        DrawCapsuleCastGizmo(top, bottom, castRadius, -meshTransform.right, wallCastDistance);

        // Front wall
        Gizmos.color = Color.blue;
        DrawCapsuleCastGizmo(top, bottom, castRadius, meshTransform.forward, wallCastDistance);

        // Back wall
        Gizmos.color = Color.yellow;
        DrawCapsuleCastGizmo(top, bottom, castRadius, -meshTransform.forward, wallCastDistance);

        // Ceiling
        Gizmos.color = Color.green;
        Gizmos.DrawLine(top, top + meshTransform.up * wallCastDistance);
        Gizmos.DrawWireSphere(top + meshTransform.up * wallCastDistance, castRadius);
    }

    private void DrawCapsuleCastGizmo(
        Vector3 top,
        Vector3 bottom,
        float radius,
        Vector3 direction,
        float distance
    )
    {
        Vector3 offset = direction.normalized * distance;

        Gizmos.DrawLine(top, top + offset);
        Gizmos.DrawLine(bottom, bottom + offset);

        Gizmos.DrawWireSphere(top + offset, radius);
        Gizmos.DrawWireSphere(bottom + offset, radius);
    }

    #endregion
}