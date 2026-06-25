using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class WallSlideState : MovementState
{
    
    [SerializeField] private float _wallFriction;
    [SerializeField] private float _wallJumpSideForce;
    [SerializeField] private float _jumpUpForce;
    [SerializeField] private float _gravityFactorWithWall = 0.1f;
    
    private EPlayerState ENUMTYPE = EPlayerState.WALLSLIDE;
    private Vector3 _oldWallHitNormal;
    
    public override void EnterState()
    {
        _playerPhysics.SetFactorGravity(_gravityFactorWithWall);
        _playerPhysics.SetAcceleration(0.1f * _playerPhysics.Acceleration); // To stop previous acceleration
    }

    public override void ExitState()
    {
        _playerPhysics.SetFactorGravity(1);
    }

    public override void UpdateState()
    {
        if(TryMakeTransition()) return;
        
        //_playerPhysics.SetAcceleration(new Vector3(_wallFriction * acceleration.x, acceleration.y, _wallFriction * acceleration.z));
        if (_controller.JumpInput)
        {
            WallJump();
        }
    }

    public override bool TryMakeTransition()
    {
        if (!_playerPhysics.isWallSide)
        {
            _stateMachine.Transition(EPlayerState.AIRLOCK);
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
        RaycastHit wallHit = _playerPhysics.WallSide;

        if (Vector3.Dot(_oldWallHitNormal, wallHit.normal.normalized) > 0.5)
        {
            return;
        }
        
        Vector3 jumpDirection = Vector3.up * _jumpUpForce + wallHit.normal * _wallJumpSideForce;
        
        acceleration = jumpDirection;
        _playerPhysics.SetAcceleration(acceleration);
        _oldWallHitNormal = wallHit.normal.normalized;
        
        _stateMachine.Transition(EPlayerState.AIRLOCK);
        return;
    }
}