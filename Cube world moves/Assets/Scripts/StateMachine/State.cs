using UnityEngine;

public abstract class State : MonoBehaviour
{
    public bool isInit { get; private set; } = false;
    protected StateMachine _stateMachine;
    protected Controller _controller;
    protected PlayerPhysics _playerPhysics;
    
    /**
     * Called once when we enter the new state.
     */
    public abstract void EnterState();
    /**
     * Called once on exit.
     */
    public abstract void ExitState();
    /**
     * Called each frame.
     */
    public abstract void UpdateState();
    
    public virtual void InitState()
    {
        _stateMachine ??= GetComponent<StateMachine>();
        _controller ??= GetComponent<Controller>();
        _playerPhysics ??= GetComponent<PlayerPhysics>();
        isInit = true;
    }
    
    public abstract EPlayerState GetEnumType();
}
