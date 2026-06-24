using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class WallRunState : MovementState
{
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _wallJumpSideForce = 8f;
    [SerializeField] private float _gravityWhileWallRunning = 0.2f;
    [SerializeField] private float _wallStickForce = 3f;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private CameraLook _camLook;

    [Space(15)]
    [Header("CameraPosition Modifiers")]
    [SerializeField] private float forwardPosCoef;
    [SerializeField] private float wallNormalPosCoef;
    [SerializeField] private float heightPosCoef;
    
    [Space(5)]
    [Header("CameraLook Position Modifiers")]
    [SerializeField] private float forwardLookCoef;
    [SerializeField] private float wallNormalLookCoef;
    [SerializeField] private float heightLookCoef;
    

    private const EPlayerState ENUMTYPE = EPlayerState.WALLRUN;
    
    public override void EnterState()
    {
        _playerPhysics.SetGravity(_gravityWhileWallRunning);
        _cameraController.SetIsWallRunning(true);
    }

    public override void ExitState()
    {
        _playerPhysics.SetGravity(1f);
        _cameraController.SetIsWallRunning(false);
        print("Exiting wall run state");
        Debug.Break();
    }

    public override void UpdateState()
    {
        MakeTransition();
        RaycastHit wallHit = _playerPhysics.isWallRight ? _playerPhysics.WallRight : _playerPhysics.WallLeft;
        SnapModel(wallHit);
        HandleJump(wallHit);

        Vector3 newCameraLook =
                wallHit.normal * wallNormalLookCoef
                + meshModel.transform.forward * forwardLookCoef
                + meshModel.transform.up * heightLookCoef
                + _playerPhysics.transform.position
            ;
        
        Vector3 newCameraPosition = 
                wallHit.normal * wallNormalPosCoef 
                + -meshModel.transform.forward * forwardPosCoef 
                + meshModel.transform.up * heightPosCoef
                + _playerPhysics.transform.position
            ;
        _cameraController.UpdateWallRunCamera(newCameraPosition, newCameraLook);
        _playerPhysics.SetAcceleration(acceleration);
        print("Fixed Update of Wall Run State : " + _camLook.transform.localPosition);

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
        print("Making transition");
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
        print("Finished transitionning");
    }
}