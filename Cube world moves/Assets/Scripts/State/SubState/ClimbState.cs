using TMPro;
using UnityEngine;

public class ClimbState : MovementState
{
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _wallStickForce = 3f;
    private const EPlayerState ENUMTYPE = EPlayerState.CLIMB;
    public override void EnterState()
    {
        _playerPhysics.SetAcceleration(Vector3.zero);
        _playerPhysics.SetGravity(0f); // Climbing gravity
    }

    public override void ExitState()
    {
        _playerPhysics.SetGravity(1f); // Base gravity
    }
    
    
    public override void UpdateState()
    {
        if(TryMakeTransition()) return;

        RaycastHit wallHit = _playerPhysics.WallFront;
        acceleration = 
              _controller.VerticalInput * _moveSpeed * meshModel.transform.up 
            + _controller.HorizontalInput * _moveSpeed * meshModel.transform.right
            + -wallHit.normal.normalized * _wallStickForce
              ;
        
        if (_controller.JumpInput)
        {
            acceleration += wallHit.normal.normalized * _wallStickForce; // Suppress wall hit force  before jumping
            if (acceleration.y >= 0) acceleration *= _jumpForce;
            else acceleration = wallHit.normal * _jumpForce + Vector3.up;
            
            _playerPhysics.SetAcceleration(acceleration);
            _stateMachine.Transition(EPlayerState.AIRLOCK);
            return;
        }
        
  
        if (Vector3.Angle(wallHit.normal, Vector3.up) < 35)
        {
            _stateMachine.Transition(EPlayerState.GROUND);
        }
        
        _playerPhysics.SetAcceleration(acceleration);

        meshModel.transform.rotation = Quaternion.LookRotation(-wallHit.normal);
    }

    public override bool TryMakeTransition()
    {
        if (!_playerPhysics.isWallFront)
        {
            _stateMachine.Transition(EPlayerState.AIR);
            return true;
        } 
        
        return false;
    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
}
