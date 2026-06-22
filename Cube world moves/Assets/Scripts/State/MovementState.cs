using UnityEngine;
public class MovementState : State
{
    
    [SerializeField] protected float maxSpeed = 10;
    [SerializeField] protected float moveSpeed = 2;
    protected PivotController pivotController;
    protected MeshModelController meshModel;
    private const EPlayerState ENUMTYPE = EPlayerState.MOVEMENT;
    
    public override void InitState()
    {
        base.InitState();
        pivotController = GetComponentInChildren<PivotController>();
        meshModel = GetComponentInChildren<MeshModelController>();
    }
    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        Vector3 acceleration = _playerPhysics.Acceleration;
        acceleration += _controller.VerticalInput * moveSpeed * pivotController.transform.forward + _controller.HorizontalInput * moveSpeed * pivotController.transform.right;
        
        acceleration.z = Mathf.Clamp(acceleration.z, -maxSpeed, maxSpeed);
        acceleration.x = Mathf.Clamp(acceleration.x, -maxSpeed, maxSpeed);
        
        _playerPhysics.SetAcceleration(acceleration);
        if(_playerPhysics.CanClimb && _controller.ClimbInput) _stateMachine.Transition(EPlayerState.CLIMB);
    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
}
