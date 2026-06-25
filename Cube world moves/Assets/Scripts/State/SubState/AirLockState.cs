using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class AirLockState : State
{
    [ShowNonSerializedField] private float _secondsWaited;
    
    [SerializeField] private float _secondsUntilClimb;
    [SerializeField] private float _secondsUntilAir;
    
    private Coroutine coroutine;
    private const EPlayerState ENUMTYPE = EPlayerState.AIRLOCK;
    private bool _isClimbLocked = true;
    private bool _isAirLocked = true;
    
    public override void EnterState()
    {
        ResetState();
    }

    public override void ExitState()
    {
        ResetState();
    }

    public override void UpdateState()
    {
        HandleLockWindow();
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

        if (!_isClimbLocked && _controller.ClimbInput && _playerPhysics.isWallFront)
        {
            _stateMachine.Transition(EPlayerState.CLIMB);
            return true;
        }
        if (!_isAirLocked)
        {
            _stateMachine.Transition(EPlayerState.AIR);
            return true;
        }

        return false;
    }
    

    void HandleLockWindow()
    {
        _secondsWaited += Time.fixedDeltaTime;
        if (_secondsWaited > _secondsUntilClimb)
        {
           _isClimbLocked = false;
        }
        if (_secondsWaited > _secondsUntilAir)
        {
            _isAirLocked = false;
        }
    }

    void ResetState()
    {
        _isClimbLocked = true;
        _isAirLocked = true;
        _secondsWaited = 0;
    }
}