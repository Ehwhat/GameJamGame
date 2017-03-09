using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class AIUnitManager<T> : UnitManager {

    protected FSM<T, UnitState<T>> stateMachine;




}
