﻿using UnityEngine;
using System.Collections;

namespace FSM
{
    public abstract class State<T>
    {

        public string stateKey;
        public FSM<T> ownerMachine;
        public T instance;

        public State (string key) {
            stateKey = key;
        }

        public void InitState(T inst, FSM<T> owner) {
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