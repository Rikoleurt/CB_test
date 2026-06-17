using UnityEngine;

public class AirState : MovementState
{
    private const EPlayerState ENUMTYPE = EPlayerState.AIR;

    public override void EnterState()
    {
        print("Entering Air State");
    }

    public override void ExitState()
    {
        print("Exiting Air State");
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_playerPhysics.IsGrounded) _stateMachine.Transition(EPlayerState.GROUND);
    }
    
    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
}
