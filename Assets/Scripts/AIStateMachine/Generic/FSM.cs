using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FSM;

public class FSM<T,StateType> where StateType : State<T, StateType> {

    public StateType currentState;

    private T machineOwner;
    private Dictionary<int, StateType> stateDict = new Dictionary<int, StateType>();
    
    public FSM (string startState, T owner, params StateType[] initalStates){
        Initialise(startState, owner, initalStates);
    }

    public void Initialise(string startState, T owner, params StateType[] initalStates) {
        RegisterStates(initalStates);
        ChangeCurrentState(startState);
        machineOwner = owner;
    }

    public void UpdateCurrentState() {
        currentState.UpdateState();
    }

    public void ChangeCurrentState(string stateKey) {
        StateType state = GetStateFromKey(stateKey);
        if (state != null)
        {
            currentState = state;
            currentState.InitState(machineOwner, this);
        }
    }

    /*public void InvokeOnState<R>(string state, string methodName,object arguement) where R : State<T>
    {
        R 
        MethodInfo method = typeof(R).GetMethod(methodName);
        method.Invoke(, new object[] { arguement} );
    }

    private R<T> GetStateFromKey<R>(string stateKey)
    {
        int hash = stateKey.GetHashCode();
        if (stateDict.ContainsKey(hash))
            return stateDict[hash];
        return null;
    }*/

    private StateType GetStateFromKey(string stateKey) {
        int hash = stateKey.GetHashCode();
        if (stateDict.ContainsKey(hash))
            return stateDict[hash];
        return null;
    }

    private void RegisterStates(StateType[] states) {
        foreach(StateType s in states) {
            RegisterState(s);
        }
    }

    private void RegisterState(StateType s) {
        stateDict.Add(s.stateKey.GetHashCode(), s);
    }
	
}
