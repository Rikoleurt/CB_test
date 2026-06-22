using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    // private State initialState;
    [SerializeField] private State currentState;
    [SerializeField] private Dictionary<EPlayerState, State> PossibleStates = new Dictionary<EPlayerState, State>();
    
    public virtual void Awake()
    {
        var xxx = GetComponents<State>();
        foreach (State s in xxx)
        {
            PossibleStates.Add(s.GetEnumType(), s);
        }
        
        InitAllStates();
        
        currentState ??= PossibleStates[0];
        if(!currentState.isInit) currentState.InitState();
        currentState.EnterState();
    }
    
    /**
     * Make a transition with a new state.
     */
    public void Transition(EPlayerState newState)
    {
        currentState.ExitState();
        currentState.enabled = false;
        currentState = PossibleStates[newState];
        currentState.enabled = true;
        currentState.EnterState();
    }
    
    
    /**
     * Each frame, call the current state's update method.
     */
    void Update()
    {
        currentState.UpdateState();
    }

    /**
     * Set all state to "Init"
     */
    private void InitAllStates()
    {
        foreach (State state in PossibleStates.Values)
        {
            state.InitState();
            state.enabled = false;
        }
    }
}