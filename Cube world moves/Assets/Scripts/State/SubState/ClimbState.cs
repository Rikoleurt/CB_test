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
        print("Entering Climb State");
    }

    public override void ExitState()
    {
        _playerPhysics.SetGravity(1f); // Base gravity
        print("Exiting Climb State");
    }
    
    
    public override void UpdateState()
    {
        MakeTransition();

        RaycastHit wallHit = _playerPhysics.WallFront;
        acceleration = 
              _controller.VerticalInput * _moveSpeed * meshModel.transform.up 
            + _controller.HorizontalInput * _moveSpeed * meshModel.transform.right
            + -wallHit.normal.normalized * _wallStickForce
              ;
        
        if (_controller.JumpInput)
        {
            if (acceleration.y >= 0) acceleration *= _jumpForce;
            else acceleration = -wallHit.normal * _jumpForce + Vector3.up;
            
            acceleration += wallHit.normal.normalized * _wallStickForce;
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

    public override void MakeTransition()
    {
        if(!_playerPhysics.isWallFront) _stateMachine.Transition(EPlayerState.AIR);

    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
}
