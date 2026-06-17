using UnityEngine;
public class MovementState : State
{
    
    [SerializeField] protected float maxSpeed = 10;
    [SerializeField] protected float moveSpeed = 2;
    
    private const EPlayerState ENUMTYPE = EPlayerState.MOVEMENT;
    
    public override void EnterState()
    {
        print("Entering Movement State");
    }

    public override void ExitState()
    {
      
        print("Exiting Movement State");
    }

    public override void UpdateState()
    {
        Vector3 acceleration = _playerPhysics.Acceleration;
        acceleration += _controller.VerticalInput * moveSpeed * transform.forward + _controller.HorizontalInput * moveSpeed * transform.right;
        
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
