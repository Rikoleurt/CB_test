using System;
using UnityEngine;

public class WallRunState : MovementState
{
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _wallJumpSideForce = 8f;
    [SerializeField] private float _gravityWhileWallRunning = 0.2f;
    [SerializeField] private float _wallStickForce = 3f;

    private const EPlayerState ENUMTYPE = EPlayerState.WALLRUN;

    public override void EnterState()
    {
        _playerPhysics.SetGravity(_gravityWhileWallRunning);
        print("Entering WallRun State");
    }

    public override void ExitState()
    {
        _playerPhysics.SetGravity(1f);
        print("Exiting WallRun State");
    }

    public override void UpdateState()
    {
        MakeTransition();
        RaycastHit wallHit = _playerPhysics.isWallRight ? _playerPhysics.WallRight : _playerPhysics.WallLeft;
        SnapModel(wallHit);
        HandleJump(wallHit);
        _playerPhysics.SetAcceleration(acceleration);
    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }

    private void SnapModel(RaycastHit wallHit)
    {
        // Direction
        Vector3 wallRunDirection = Vector3.Cross(wallHit.normal, Vector3.up).normalized;
        if (Vector3.Dot(wallRunDirection, meshModel.transform.forward) < 0f) wallRunDirection = -wallRunDirection;
        
        // Make it look the wall run direction
        meshModel.transform.rotation = Quaternion.LookRotation(wallRunDirection, Vector3.up);
        // Accelerate the player along the wall
        acceleration = wallRunDirection * (_controller.VerticalInput * _moveSpeed) + -wallHit.normal.normalized * _wallStickForce;
    }

    private void HandleJump(RaycastHit wallHit)
    {
        if (_controller.JumpInput)
        {
            acceleration += wallHit.normal.normalized * _wallStickForce;

            Vector3 jumpDirection =
                Vector3.up * _jumpForce +
                wallHit.normal * _wallJumpSideForce;
            
            acceleration += jumpDirection;
            
            _playerPhysics.SetAcceleration(acceleration);
            _stateMachine.Transition(EPlayerState.AIRLOCK);
            return;
        }
    }
    public override void MakeTransition()
    {
        if (_playerPhysics.isWallDown)
        {
            _stateMachine.Transition(EPlayerState.GROUND);
            return;
        }

        if (!_playerPhysics.isWallLeft && !_playerPhysics.isWallRight)
        {
            _stateMachine.Transition(EPlayerState.AIR);
            return;
        } 
    }
}