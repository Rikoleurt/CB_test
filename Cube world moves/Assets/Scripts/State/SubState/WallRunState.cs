using UnityEngine;
public class WallRunState : MovementState
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityWhileWallRunning;
    private const EPlayerState ENUMTYPE = EPlayerState.WALLRUN;
    
    public override void EnterState()
    {
        _playerPhysics.SetGravity(gravityWhileWallRunning);
    }

    public override void ExitState()
    {
        _playerPhysics.SetGravity(1);
    }

    public override void UpdateState()
    {
        Vector3 localRight = _playerPhysics.CanRunWallRight ? -transform.right : transform.right;
        acceleration = _controller.VerticalInput * moveSpeed * localRight;

        if (_controller.JumpInput)
        {
            _playerPhysics.SetAcceleration(new Vector3(acceleration.x, acceleration.y, acceleration.z) + transform.up * jumpForce + -transform.forward * jumpForce);
            _stateMachine.Transition(EPlayerState.AIRLOCK);
            return;
        }
        
        #region StateMachineTransition
        if (_playerPhysics.IsGrounded)
        {
            _stateMachine.Transition(EPlayerState.GROUND);
            return;
        }

        if (!_playerPhysics.CanRunWallLeft)
        {
            _stateMachine.Transition(EPlayerState.AIR);
            return;
        }
        #endregion
        _playerPhysics.SetAcceleration(acceleration);
    }
    
    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
}
