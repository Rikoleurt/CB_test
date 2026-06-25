using NaughtyAttributes;
using UnityEngine;
public class WallSlideState : MovementState
{
    private EPlayerState ENUMTYPE = EPlayerState.WALLSLIDE;
    private Vector3 _oldWallHitNormal;
    private bool _hasWallJumped;
    [SerializeField] private float _wallFriction;
    [SerializeField] private float _wallJumpSideForce;
    [SerializeField] private float _jumpForce;
    
    public override void EnterState()
    {
        _playerPhysics.SetGravity(0.1f);
        _playerPhysics.SetAcceleration(0.05f * acceleration); // To stop previous acceleration
       
    }

    public override void ExitState()
    {
        _playerPhysics.SetGravity(1);
        _hasWallJumped = false;
        
    }

    public override void UpdateState()
    {
        base.UpdateState();
        _playerPhysics.SetAcceleration(new Vector3(_wallFriction * acceleration.x, acceleration.y, _wallFriction * acceleration.z));
        if (_controller.JumpInput)
        {
            WallJump();
        }
        if(TryMakeTransition()) return;
    }

    public override bool TryMakeTransition()
    {
        if (!_playerPhysics.isWallSide)
        {
            _stateMachine.Transition(EPlayerState.AIR);
            return true;
        }

        if (_playerPhysics.isWallDown)
        {
            _stateMachine.Transition(EPlayerState.GROUND);
            return true;
        }

        if (_controller.WallRunInput)
        {
            _stateMachine.Transition(EPlayerState.WALLRUN);
            return true;
        }
        return false;
    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
    
    private void WallJump()
    {
        RaycastHit wallHit = _playerPhysics.isWallRight ? _playerPhysics.WallRight : _playerPhysics.WallLeft;
        if (_hasWallJumped && Vector3.Dot(_oldWallHitNormal, wallHit.normal.normalized) > 0.5)
        {
            return;
        }
        
        Vector3 jumpDirection = Vector3.up * _jumpForce + wallHit.normal * _wallJumpSideForce;
        
        acceleration += jumpDirection;
        _playerPhysics.SetAcceleration(acceleration);
        _hasWallJumped = true;
        _oldWallHitNormal = wallHit.normal.normalized;
        
    }
}