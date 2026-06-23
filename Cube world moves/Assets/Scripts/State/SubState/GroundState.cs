using UnityEngine;

public class GroundState : MovementState
{
    [SerializeField] private float jumpForce = 3;
    [SerializeField] private bool canJump;
    private const EPlayerState ENUMTYPE = EPlayerState.GROUND;

    public override void EnterState()
    {
        canJump = true;
        print("Entering Ground State");
    }

    public override void ExitState()
    {
        base.ExitState();
        canJump = false;
        print("Exiting Ground State");
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if(acceleration.magnitude > 0.1f) meshModel.UpdateModelRotation(pivotController.transform.eulerAngles.y);

        if (_controller.JumpInput && canJump)
        {
            _playerPhysics.AddAcceleration(jumpForce * Vector3.up);
            canJump = false;
        }
        if (!_playerPhysics.isWallDown) _stateMachine.Transition(EPlayerState.AIR);
        
    }
    

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
}
