using System.Collections;
using UnityEngine;

public class AirLockState : State
{
    [SerializeField] private int nbFrames;
    private Coroutine coroutine;
    private const EPlayerState ENUMTYPE = EPlayerState.AIRLOCK;

    public override void EnterState()
    {
        coroutine = StartCoroutine(LockWindow());
    }

    public override void ExitState()
    {
        StopCoroutine(coroutine);
        coroutine = null;
    }

    public override void UpdateState()
    {
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
    }

    IEnumerator LockWindow()
    {
        int framesToWait = nbFrames;
        while (framesToWait > 0)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            framesToWait--;
        }

        _stateMachine.Transition(EPlayerState.AIR);
    }
}