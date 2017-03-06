using UnityEngine;
using System.Collections;
using FSM;
using System;

public abstract class TriggerState<T> : State<T, TriggerState<T>> {
    public TriggerState(string key) : base(key)
    {
    }

    public override abstract void EndState();

    public override abstract void StartState();

    public override abstract void UpdateState();

    public abstract void OnTriggerEnter(Collider other);

}
