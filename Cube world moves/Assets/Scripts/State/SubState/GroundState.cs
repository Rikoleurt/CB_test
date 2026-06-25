using UnityEngine;

public class GroundState : MovementState
{
    [SerializeField] private float jumpForce = 3;
    [SerializeField] private bool canJump;
    
    private const EPlayerState ENUMTYPE = EPlayerState.GROUND;

    public override void EnterState()
    {
        canJump = true;
    }

    public override void ExitState()
    {
        base.ExitState();
        canJump = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        HandleJump();
        if(TryMakeTransition()) return;
    }
    

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }

    public override bool TryMakeTransition()
    {
        if (!_playerPhysics.isWallDown)
        {
            _stateMachine.Transition(EPlayerState.AIR);
            return true;
        }

        if (_playerPhysics.isWallFront && _controller.ClimbInput)
        {
            _stateMachine.Transition(EPlayerState.CLIMB);
            return true;
        }
        return false;
    }

    private void HandleJump()
    {
        if (_controller.JumpInput && canJump)
        {
            _playerPhysics.AddAcceleration(jumpForce * Vector3.up);
            canJump = false;
        }

    }
}
