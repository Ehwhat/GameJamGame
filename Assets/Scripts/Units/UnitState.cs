using UnityEngine;
using System.Collections;
using FSM;
using System;

public abstract class UnitState<T> : State<T, UnitState<T>> {

    public UnitState(string key) : base(key)
    {
    }

    public abstract override void EndState();
    public abstract override void StartState();
    public abstract override void UpdateState();
    public abstract void FixedUpdateState();

    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerExit(Collider other);

    public abstract void OnDrawGizmos();
    
}
