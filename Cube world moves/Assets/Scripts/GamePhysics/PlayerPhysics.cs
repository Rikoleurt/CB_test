using NaughtyAttributes;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    
    [SerializeField] Vector3 _acceleration;
    [SerializeField] private float _gravity;
    [SerializeField] private float groundFriction; // Between 0 and 1
    [SerializeField] private float stopThreshold;
    
    MeshModelController _meshModel;
    
    private RaycastHit _wallLeft;
    private RaycastHit _wallRight;
    private RaycastHit _wallUp;
    private RaycastHit _wallDown;
    private RaycastHit _wallFront;
    private RaycastHit _wallBack;
    
    public Vector3 Acceleration => _acceleration;
    public float Gravity => _gravity;

    public RaycastHit WallLeft => _wallLeft;
    public RaycastHit WallRight => _wallRight;
    public RaycastHit WallUp => _wallUp;
    public RaycastHit WallDown => _wallDown;
    public RaycastHit WallFront => _wallFront;
    public RaycastHit WallBack => _wallBack;
    
    public bool isWallDown => _wallDown.collider;
    public bool isWallFront => _wallFront.collider;
    public bool isWallBack => _wallBack.collider;
    public bool isWallLeft => _wallLeft.collider;
    public bool isWallRight => _wallRight.collider;
    public bool isWallUp => _wallUp.collider;

    public bool isWallSide => isWallLeft || isWallRight;
    
    public RaycastHit WallSide => isWallRight ? _wallRight : _wallLeft;
    private void Start()
    {
        _meshModel = GetComponentInChildren<MeshModelController>();
    }

    void FixedUpdate()
    { 
        CheckWalls();
        HandleVelocity();
    }
    
    #region acceleration
    void HandleVelocity()
    {
        // Handle x and z movements (with friction)
        if (_wallDown.collider)
        {
            if (_acceleration.x != 0) _acceleration.x += groundFriction * -_acceleration.x;
            if (_acceleration.z != 0) _acceleration.z += groundFriction * -_acceleration.z;
            // Passing a certain threshold, we stop the player
            if (_acceleration.magnitude < stopThreshold) _acceleration = Vector3.zero;
            
            if(_acceleration.y < 0) _acceleration.y = 0;
        }
        else
        {
            _acceleration.y -= _gravity;
        }
        
        transform.position += _acceleration * Time.fixedDeltaTime;
    }
    public void SetAcceleration(Vector3 newAcceleration)
    {
        _acceleration = newAcceleration;
    }
    public void AddAcceleration(Vector3 accelerationVector)
    {
        _acceleration += accelerationVector;
    }

    public void SetGravity(float newGravity)
    {
        _gravity = newGravity;
    }
    #endregion
    #region WallChecker
    private void CheckWalls()
    {
        Vector3 origin = _meshModel.transform.position;

        Physics.Raycast(origin, Vector3.down, out _wallDown, 1);
        Physics.Raycast(origin, _meshModel.transform.up, out _wallUp, 1);
        Physics.Raycast(origin, _meshModel.transform.right, out _wallRight, 1);
        Physics.Raycast(origin, -_meshModel.transform.right, out _wallLeft, 1);
        Physics.Raycast(origin, _meshModel.transform.forward, out _wallFront, 1);
        Physics.Raycast(origin, -_meshModel.transform.forward, out _wallBack, 1);
    }
    
    private void OnDrawGizmos()
    {
        if (_meshModel == null)
            return;

        Vector3 origin = _meshModel.transform.position;
        float distance = 1f;

        Transform meshTransform = _meshModel.transform;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + Vector3.down * distance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + meshTransform.up * distance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + meshTransform.right * distance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(origin, origin - meshTransform.right * distance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, origin + meshTransform.forward * distance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin - meshTransform.forward * distance);
    }
    #endregion
}
