using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class AIUnitManager<T> : UnitManager {

    public FiniteStateMachine<T> aiStateMachine;


}
