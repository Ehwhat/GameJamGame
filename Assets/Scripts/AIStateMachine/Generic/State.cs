using UnityEngine;
using System.Collections;

namespace FSM
{
    public abstract class State<T,StateType> where StateType : State<T,StateType>
    {

        public string stateKey;
        public FSM<T, StateType> ownerMachine;
        public T instance;

        public State (string key) {
            stateKey = key;
        }

        public void InitState(T inst, FSM<T, StateType> owner) {
            instance = inst;
            ownerMachine = owner;
            StartState();
        }

        public void RequestStateChange(string state) {
            EndState();
            ownerMachine.ChangeCurrentState(state);
        }
        
        public abstract void StartState();
        public abstract void UpdateState();
        public abstract void EndState();
    }
}
