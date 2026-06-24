using UnityEngine;

public class AirState : MovementState
{
    private const EPlayerState ENUMTYPE = EPlayerState.AIR;

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        base.UpdateState();
        MakeTransition();
    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }

    public override void MakeTransition()
    {
        if (_playerPhysics.isWallDown)
        {
            _stateMachine.Transition(EPlayerState.GROUND);
            return;
        }
        if (_playerPhysics.isWallSide)
        {
            _stateMachine.Transition(EPlayerState.WALLRUN);
            return;
        }

        if (_playerPhysics.isWallFront && _controller.ClimbInput)
        {
            _stateMachine.Transition(EPlayerState.CLIMB);
            return;
        }
    }
}