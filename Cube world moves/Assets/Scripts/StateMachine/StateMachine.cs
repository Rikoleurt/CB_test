using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    // private State initialState;
    [SerializeField] private State currentState;
    [SerializeField] private Dictionary<EPlayerState, State> PossibleStates = new Dictionary<EPlayerState, State>();
    
    public virtual void Awake()
    {
        var components = GetComponents<State>();
        foreach (State s in components)
        {
            PossibleStates.Add(s.GetEnumType(), s);
            print(s.name);
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
        if (currentState != null && currentState.GetEnumType() == newState)
            return;
        
        currentState.ExitState();
        currentState.enabled = false;
        currentState = PossibleStates[newState];
        currentState.enabled = true;
        currentState.EnterState();
    }
    
    
    /**
     * Each frame, call the current state's update method.
     */
    void FixedUpdate()
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

    public State GetCurrentState()
    {
        return currentState;
    }

    void OnGUI()
    {
        GUI.Label(
            new Rect(10, 10, 300, 30),
            currentState != null ? currentState.ToString() : "No State"
        );
    }
}