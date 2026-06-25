using UnityEngine;
using UnityEngine.Serialization;

public class MovementState : State
{
    [SerializeField] protected float maxSpeed = 10;
    [SerializeField] protected float _accelerationRate = 2;

    protected Vector3 acceleration;
    protected PivotController pivotController;
    protected MeshModelController _meshModel;
    protected Camera _playerCam;

    private const EPlayerState ENUMTYPE = EPlayerState.MOVEMENT;
    
    public override void InitState()
    {
        base.InitState();
        pivotController = GetComponentInChildren<PivotController>();
        _meshModel = GetComponentInChildren<MeshModelController>();
        _playerCam = Camera.main;
    }
    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        acceleration = _playerPhysics.Acceleration;
        
        Vector3 moveDirection = ComputeMoveDirection();
        
        Vector3 groundAcceleration = new Vector3(acceleration.x, 0f, acceleration.z);
        groundAcceleration += moveDirection * _accelerationRate;
        groundAcceleration = Vector3.ClampMagnitude(
            groundAcceleration,
            maxSpeed
        );

        acceleration.x = groundAcceleration.x;
        acceleration.z = groundAcceleration.z;

        _meshModel.transform.eulerAngles = new Vector3(0, pivotController.transform.eulerAngles.y, 0);
        _playerPhysics.SetAcceleration(acceleration);
    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }

    private Vector3 ComputeMoveDirection()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(_playerCam.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(_playerCam.transform.right, Vector3.up).normalized;
        
        Vector2 input = new Vector2(
            _controller.HorizontalInput,
            _controller.VerticalInput
        );

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 moveDirection = camForward * input.y + camRight * input.x;

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }
        
        return moveDirection;
    }

    public override bool TryMakeTransition()
    {
        throw new System.NotImplementedException();
    }
}
