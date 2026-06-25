using UnityEngine;
public class WallRunState : MovementState
{
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _wallJumpSideForce = 8f;
    [SerializeField] private float _gravityWhileWallRunning = 0.2f;
    [SerializeField] private float _wallStickForce = 3f;
    [SerializeField] private CameraController _cameraController;

    [Space(15)]
    [Header("Custom CameraProfile")]
    [SerializeField] CameraProfile _wallRunProfile;

    private const EPlayerState ENUMTYPE = EPlayerState.WALLRUN;
    
    public override void EnterState()
    {
        _playerPhysics.SetFactorGravity(_gravityWhileWallRunning);
        _cameraController.SetCameraProfile(_wallRunProfile);
    }

    public override void ExitState()
    {
        _playerPhysics.SetFactorGravity(1f);
        _cameraController.SetBaseCameraProfile();

    }

    public override void UpdateState()
    {
        if(TryMakeTransition()) return;
        RaycastHit wallHit = _playerPhysics.isWallRight ? _playerPhysics.WallRight : _playerPhysics.WallLeft;
        SnapModel(wallHit);
        HandleJump(wallHit);

        
        _cameraController.UpdateWallRunCamera(wallHit.normal,meshModel.transform.forward,meshModel.transform.up);
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
    public override bool TryMakeTransition()
    {
        if (_playerPhysics.isWallDown)
        {
            _stateMachine.Transition(EPlayerState.GROUND);
            return true;
        }

        if (!_playerPhysics.isWallLeft && !_playerPhysics.isWallRight)
        {
            _stateMachine.Transition(EPlayerState.AIR);
            return true;
        } 
        return false;
    }
}