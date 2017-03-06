using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FSM;

public class FSM<T> {

    public State<T> currentState;

    private T machineOwner;
    private Dictionary<int, State<T>> stateDict = new Dictionary<int, State<T>>();
    
    public FSM (string startState, T owner, params State<T>[] initalStates){
        Initialise(startState, owner, initalStates);
    }

    public void Initialise(string startState, T owner, params State<T>[] initalStates) {
        RegisterStates(initalStates);
        ChangeCurrentState(startState);
        machineOwner = owner;
    }

    public void UpdateCurrentState() {
        currentState.UpdateState();
    }

    public void ChangeCurrentState(string stateKey) {
        State<T> state = GetStateFromKey(stateKey);
        if (state != null)
        {
            currentState = state;
            currentState.InitState(machineOwner, this);
        }
    }

    private State<T> GetStateFromKey(string stateKey) {
        int hash = stateKey.GetHashCode();
        if (stateDict.ContainsKey(hash))
            return stateDict[hash];
        return null;
    }

    private void RegisterStates(State<T>[] states) {
        foreach(State<T> s in states) {
            RegisterState(s);
        }
    }

    private void RegisterState(State<T> s) {
        stateDict.Add(s.stateKey.GetHashCode(), s);
    }
	
}
