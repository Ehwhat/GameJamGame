using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnemyMessages;

public abstract class EnemyBase : MonoBehaviour {

    public enum EnemyState
    {
        Patrolling,
        Alert,
        Attacking,
        Dead
    }

    public delegate void MessageHandlerFunction<T>(T message);

    private delegate void MessageInternalFunction(object message);
    private delegate void MessageHandler(object message);

    private Dictionary<System.Type, MessageHandler> _messageHandlers = new Dictionary<System.Type, MessageHandler>();

    protected EnemySquadManager _squadManager;
    protected EnemyState _enemyState = EnemyState.Patrolling;

    public void InitaliseEnemy(EnemySquadManager squad)
    {
        _squadManager = squad;
        OnSquadSpawn();
    }

    public abstract void OnSquadSpawn();


    public void OnRecieveSquadMessage<T>(T message)
    {
        HandleMessage<T>(message);
    }

    void HandleMessage<T>(T message)
    {
        System.Type type = typeof(T);
        if (_messageHandlers.ContainsKey(type))
        {
            _messageHandlers[type].Invoke(message);
        }
    }

    public void AddMessageListener<T>(MessageHandlerFunction<T> handler) {
        System.Type type = typeof(T);
        MessageInternalFunction func = new MessageInternalFunction((m) => { handler(CastObjectToType<T>(m)); });
        _messageHandlers[type] = ((m) => { func.Invoke(m); });
    }

    public void SendMessageToSquad<T>(T message)
    {
        _squadManager.RelayMessage<T>(message, this);
    }

    T CastObjectToType<T>(object obj)
    {
        return (T)obj;
    }

    void MovementStep()
    {

    }
    

}
