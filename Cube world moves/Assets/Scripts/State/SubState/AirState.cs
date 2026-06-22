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
        if(_playerPhysics.IsGrounded) _stateMachine.Transition(EPlayerState.GROUND);
        if(_playerPhysics.CanRunWallRight) _stateMachine.Transition(EPlayerState.WALLRUN);
        if(_playerPhysics.CanRunWallLeft) _stateMachine.Transition(EPlayerState.WALLRUN);
    }
    
    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }
}
