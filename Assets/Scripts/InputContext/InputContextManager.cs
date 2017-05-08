using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputContextManager : MonoBehaviour {

    [System.Serializable]
    public class InputContextManagerEvent : UnityEvent<PlayerManager> { }

    [System.Serializable]
    public struct InputContextPattern
    {
        [SerializeField]
        public InputContextManagerEvent _successEvent;
        public List<InputContext.ThumbstickPositions> _command;
    }

    public Transform _inputContextParent;
    private static Transform _inputContextParentStatic;
    private static InputContext _prefabStatic;
    private static InputIndicator _indicatorPrefabStatic;
    public static List<InputContextPattern> _inputContextPatternsStatic;
    public static Dictionary<Transform, InputIndicator> _inputIndicatorDict = new Dictionary<Transform, InputIndicator>();

    public InputIndicator _indicatorPrefab;
    public InputContext _prefab;
    public List<InputContextPattern> _inputContextPatterns;

    void Awake()
    {
        _inputContextParentStatic = _inputContextParent;
        _inputContextPatternsStatic = _inputContextPatterns;
        _prefabStatic = _prefab;
        _indicatorPrefabStatic = _indicatorPrefab;
    }

    public static InputContext CreateNewInputContext(InputContext context, Transform track, Camera camera, PlayerControlManager.PlayerController listenController, PlayerManager player, UnityAction successEvent = null, UnityAction failureEvent = null)
    {
        context = ((InputContext)Instantiate(context, _inputContextParentStatic)).Init(listenController, player);
        //context.transform.SetParent(_inputContextParentStatic, false);
        if (successEvent != null)
        {
            context._successEvent.AddListener(successEvent);
        }
        if (failureEvent != null)
        {
            context._failureEvent.AddListener(failureEvent);
        }
        context._trackingTransform = track;
        context._cameraToTrackFrom = camera;
        player.EnterContext(context);
        return context;
    }

    public static InputContext CreateNewRandomInputContext(int length, bool allowResets, Transform track, Camera camera, PlayerControlManager.PlayerController listenController, PlayerManager player, UnityAction successEvent = null, UnityAction failureEvent = null)
    {

        InputContext context = ((InputContext)Instantiate(_prefabStatic, _inputContextParentStatic)).InitRandom(listenController, player, length, allowResets);

        //context.transform.SetParent(_inputContextParentStatic, false);
        if (successEvent != null)
        {
            context._successEvent.AddListener(successEvent);
        }
        if (failureEvent != null)
        {
            context._failureEvent.AddListener(failureEvent);
        }
        context._trackingTransform = track;
        context._cameraToTrackFrom = camera;
        player.EnterContext(context);
        return context;
    }

    public static InputContext CreateNewManagerInputContext(Transform track, Camera camera, PlayerControlManager.PlayerController listenController, PlayerManager player)
    {

        InputContext context = ((InputContext)Instantiate(_prefabStatic, _inputContextParentStatic)).InitCompareToManager(listenController, player);
        //context.transform.SetParent(_inputContextParentStatic, false);
        context._trackingTransform = track;
        context._cameraToTrackFrom = camera;
        player.EnterContext(context);
        return context;
    }

    public static void PlaceIndicator(Transform t)
    {
        if (!_inputIndicatorDict.ContainsKey(t))
        {
            InputIndicator ind = (InputIndicator)Instantiate(_indicatorPrefabStatic, _inputContextParentStatic);
            ind._trackingTransform = t;
            ind._cameraToTrackFrom = GameManager.GetCamera().camera;
            _inputIndicatorDict[t] = ind;
        }
    }

    public static void RemoveIndicator(Transform t)
    {
        if (_inputIndicatorDict.ContainsKey(t))
        {
            Destroy(_inputIndicatorDict[t].gameObject);
            _inputIndicatorDict.Remove(t);
        }
    }

    public static bool ComparePatternToManagerPattern(List<InputContext.ThumbstickPositions> pattern, PlayerManager player)
    {
        foreach(InputContextPattern pat in _inputContextPatternsStatic)
        {
            if (pat._command.Count == pattern.Count)
            {
                bool fail = false;
                for (int i = 0; i < pat._command.Count; i++)
                {
                    if (pattern[i] != pat._command[i])
                    {
                        fail = true;
                        break;
                    }
                }
                if (fail)
                {
                    continue;
                }
                else if (pat._successEvent != null)
                {
                    pat._successEvent.Invoke(player);
                }
                return true;

            }
        }
        return false;
    }

}
