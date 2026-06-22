using UnityEngine;
public class LeftWallRunState : MovementState
{
    [SerializeField] private float jumpForce;
    private const EPlayerState ENUMTYPE = EPlayerState.LEFTRUNWALL;
    
    public override void EnterState()
    {
        _playerPhysics.SetGravity(0);
    }

    public override void ExitState()
    {
        _playerPhysics.SetGravity(1);
    }

    public override void UpdateState()
    {
        Vector3 acceleration = _controller.VerticalInput * moveSpeed * transform.right;
        if (_controller.JumpInput) _playerPhysics.SetAcceleration(new Vector3(acceleration.x, acceleration.y, acceleration.z) + transform.up * jumpForce + -transform.forward * jumpForce);
        else _playerPhysics.SetAcceleration(acceleration);
        if(_playerPhysics.IsGrounded) _stateMachine.Transition(EPlayerState.GROUND);
        if(!_playerPhysics.CanRunWallLeft) _stateMachine.Transition(EPlayerState.AIR);
    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
}
