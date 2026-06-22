using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    [SerializeField] bool _isGrounded;
    [SerializeField] bool _canClimb;
    [SerializeField] bool _canRunWallRight;
    [SerializeField] bool _canRunWallLeft;
    [SerializeField] Vector3 _acceleration;
    [SerializeField] private float _gravity;
    [SerializeField] private float groundFriction; // Between 0 and 1
    [SerializeField] private float stopThreshold;
    MeshModelController _meshModel;
    
    public Vector3 Acceleration => _acceleration;
    public bool IsGrounded => _isGrounded;
    public bool CanClimb => _canClimb;
    public float Gravity => _gravity;
    public bool CanRunWallRight => _canRunWallRight;

    public bool CanRunWallLeft => _canRunWallLeft;

    private void Start()
    {
        _meshModel = GetComponentInChildren<MeshModelController>();
    }

    void FixedUpdate()
    { 
        CheckGround();
        CheckClimbableWall();
        CheckRightRunnableWall();
        CheckLeftRunnableWall();
        HandleVelocity();
    }
    
    #region voids
    void HandleVelocity()
    {
        // Handle x and z movements (with friction)
        if (_isGrounded)
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
    #endregion
    
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
    
    #region Checker
    void CheckGround()
    {
        RaycastHit hit = new();
        Physics.Raycast(transform.position, Vector3.down, out hit, 0.55f);
        _isGrounded = hit.collider && hit.collider.CompareTag("Ground");
    }
    
    void CheckClimbableWall()
    {
        RaycastHit hit = new();
        Physics.Raycast(transform.position, _meshModel.transform.forward, out hit, 0.7f);
        _canClimb = hit.collider && hit.collider.CompareTag("Wall");
    }

    void CheckRightRunnableWall()
    {
        RaycastHit hit = new();
        Physics.Raycast(transform.position, _meshModel.transform.right, out hit, 0.7f);
        _canRunWallRight = hit.collider && hit.collider.CompareTag("Wall");
    }

    void CheckLeftRunnableWall()
    {
        RaycastHit hit = new();
        Physics.Raycast(transform.position, -_meshModel.transform.right, out hit, 0.7f);
        _canRunWallLeft = hit.collider && hit.collider.CompareTag("Wall");
    }
    #endregion
}
