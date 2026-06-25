using NaughtyAttributes;
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
        if(TryMakeTransition()) return;
    }

    public override EPlayerState GetEnumType()
    {
        return ENUMTYPE;
    }

    public override bool TryMakeTransition()
    {
        if (_playerPhysics.isWallDown)
        {
            _stateMachine.Transition(EPlayerState.GROUND);
            return true;
        }
        if (_playerPhysics.isWallSide && _controller.WallRunInput)
        {
            _stateMachine.Transition(EPlayerState.WALLRUN);
            return true;
        }

        if (_playerPhysics.isWallSide)
        {
            _stateMachine.Transition(EPlayerState.WALLSLIDE);
            return true;
        }

        if (_playerPhysics.isWallFront && _controller.ClimbInput)
        {
            _stateMachine.Transition(EPlayerState.CLIMB);
            return true;
        }
        return false;
    }
}