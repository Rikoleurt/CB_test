using UnityEngine;

public class ClimbState : MovementState
{
    private const EPlayerState ENUMTYPE = EPlayerState.CLIMB;
    
    public override void EnterState()
    {
        _playerPhysics.SetAcceleration(Vector3.zero);
        print("Entering Climb State");
    }

    public override void ExitState()
    {
        print("Exiting Climb State");
    }
    
    
    public override void UpdateState()
    {
        Vector3 acceleration = _playerPhysics.Acceleration;
        acceleration += _controller.VerticalInput * moveSpeed * transform.up + _controller.HorizontalInput * moveSpeed * transform.right;
        
        acceleration.y = Mathf.Clamp(acceleration.y, -maxSpeed, maxSpeed);
        acceleration.x = Mathf.Clamp(acceleration.x, -maxSpeed, maxSpeed);
        
        _playerPhysics.SetAcceleration(acceleration);
        if(!_controller.ClimbInput) _stateMachine.Transition(EPlayerState.AIR);
        if(!_playerPhysics.CanClimb) _stateMachine.Transition(EPlayerState.AIR);
    }
    

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
}
