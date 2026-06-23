using System;
using UnityEngine;

public class WallRunState : MovementState
{
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float wallJumpSideForce = 8f;
    [SerializeField] private float gravityWhileWallRunning = 0.2f;

    private const EPlayerState ENUMTYPE = EPlayerState.WALLRUN;

    public override void EnterState()
    {
        _playerPhysics.SetGravity(gravityWhileWallRunning);
        print("Entering WallRun State");
    }

    public override void ExitState()
    {
        _playerPhysics.SetGravity(1f);
        print("Exiting WallRun State");
    }

    public override void UpdateState()
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
        
        RaycastHit wallHit = _playerPhysics.isWallRight ? _playerPhysics.WallRight : _playerPhysics.WallLeft;
        
        Vector3 wallNormal = wallHit.normal;
        Vector3 wallRunDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;

        if (Vector3.Dot(wallRunDirection, meshModel.transform.forward) < 0f) wallRunDirection = -wallRunDirection;
        
        meshModel.transform.rotation = Quaternion.LookRotation(wallRunDirection, Vector3.up);
        acceleration = wallRunDirection * (_controller.VerticalInput * moveSpeed);
        
        if (_controller.JumpInput)
        {
            Vector3 jumpDirection =
                Vector3.up * jumpForce +
                wallNormal * wallJumpSideForce;

            _playerPhysics.SetAcceleration(jumpDirection+ acceleration);
            _stateMachine.Transition(EPlayerState.AIRLOCK);
            return;
        }


        _playerPhysics.SetAcceleration(acceleration);
    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }

    private void OnDrawGizmos()
    {
        
    }
}