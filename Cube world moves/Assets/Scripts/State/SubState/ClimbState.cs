using UnityEngine;

public class ClimbState : MovementState
{
    [SerializeField] private float jumpForce;
    private const EPlayerState ENUMTYPE = EPlayerState.CLIMB;
    public override void EnterState()
    {
        meshModel.CanRotating = false;
        _playerPhysics.SetAcceleration(Vector3.zero);
        _playerPhysics.SetGravity(0f); // Climbing gravity
        print("Entering Climb State");
    }

    public override void ExitState()
    {
        meshModel.CanRotating = true;
        _playerPhysics.SetGravity(1f); // Base gravity
        print("Exiting Climb State");
    }
    
    
    public override void UpdateState()
    {
    }
    

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
    
}
