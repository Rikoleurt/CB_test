using UnityEngine;

public class ClimbState : MovementState
{
    [SerializeField] private float jumpForce;
    private const EPlayerState ENUMTYPE = EPlayerState.CLIMB;
    public override void EnterState()
    {
        meshModel.IsRotating = false;
        _playerPhysics.SetAcceleration(Vector3.zero);
        _playerPhysics.SetGravity(0f); // Climbing gravity
        print("Entering Climb State");
    }

    public override void ExitState()
    {
        meshModel.IsRotating = true;
        _playerPhysics.SetGravity(1f); // Base gravity
        print("Exiting Climb State");
    }
    
    
    public override void UpdateState()
    {
        Vector3 acceleration = _controller.VerticalInput * moveSpeed * transform.up + _controller.HorizontalInput * moveSpeed * transform.right;
        if (_controller.JumpInput)
        {
            if(acceleration.y >= 0) _playerPhysics.SetAcceleration(jumpForce * new Vector3(acceleration.x, acceleration.y, acceleration.z));
            else _playerPhysics.SetAcceleration(-transform.forward * jumpForce + transform.up );
        }
        else _playerPhysics.SetAcceleration(acceleration);    
        if(!_controller.ClimbInput) _stateMachine.Transition(EPlayerState.AIR);
        if(!_playerPhysics.CanClimb) _stateMachine.Transition(EPlayerState.AIR);
        
    }
    

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
}
