using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    [SerializeField] private State initialState;

    public Dictionary<string, State> States = new();

    public State currentState { get; private set; }

    public State CreateState(string name)
    {
        var state = new State();
        state.name = name;
        if (States.Count == 0) initialState = state;

        States[name] = state;
        return state;
    }

    public void Update()
    {
        if (States.Count == 0 || initialState == null)
        {
            Debug.LogError("No states defined");
            return;
        }

        if (currentState == null) TransitionTo(initialState);
        if (currentState != null && currentState.OnFrame != null) currentState.OnFrame();
    }

    public void TransitionTo(State state)
    {
        if (state == null)
        {
            Debug.LogError("Null state");
            return;
        }
        
        if (state == currentState)
        {
            return;
        }

        if (currentState != null && currentState.OnExit != null) currentState.OnExit();
        Debug.Log($"Transitioning from {currentState} to {state}");
        currentState = state;
        currentState.OnEnter?.Invoke();
    }

    public class State
    {
        public string name;
        public Action OnEnter;
        public Action OnExit;
        public Action OnFrame;

        public override string ToString()
        {
            return name;
        }
    }
}
