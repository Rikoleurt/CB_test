using UnityEngine;
public class MovementState : State
{
    
    [SerializeField] protected float maxSpeed = 10;
    [SerializeField] protected float _moveSpeed = 2;

    protected Vector3 acceleration;

    protected PivotController pivotController;
    protected MeshModelController meshModel;
    protected Camera playerCam;

    private const EPlayerState ENUMTYPE = EPlayerState.MOVEMENT;
    
    public override void InitState()
    {
        base.InitState();
        pivotController = GetComponentInChildren<PivotController>();
        meshModel = GetComponentInChildren<MeshModelController>();
        playerCam = Camera.main;
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
        
        groundAcceleration += moveDirection * moveSpeed;
        
        groundAcceleration = Vector3.ClampMagnitude(
            groundAcceleration,
            maxSpeed
        );

        acceleration.x = groundAcceleration.x;
        acceleration.z = groundAcceleration.z;

        _playerPhysics.SetAcceleration(acceleration);
        MakeTransition();
    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }

    private Vector3 ComputeMoveDirection()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(playerCam.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(playerCam.transform.right, Vector3.up).normalized;
        
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
    
    public override void MakeTransition()
    {
        if (_playerPhysics.isWallFront && _controller.ClimbInput)
        {
            _stateMachine.Transition(EPlayerState.CLIMB);
            return;
        }
    }
}
