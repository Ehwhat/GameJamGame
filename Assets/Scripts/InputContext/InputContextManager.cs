using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputContextManager : MonoBehaviour {

    [System.Serializable]
    public struct InputContextPattern
    {
        [SerializeField]
        public InputContextEvent _successEvent;
        public List<InputContext.ThumbstickPositions> _command;
    }

    public Transform _inputContextParent;
    private static Transform _inputContextParentStatic;
    private static InputContext _prefabStatic;
    public static List<InputContextPattern> _inputContextPatternsStatic;

    public InputContext _prefab;
    public List<InputContextPattern> _inputContextPatterns;

    void Awake()
    {
        _inputContextParent = transform;
        _inputContextParentStatic = _inputContextParent;
        _inputContextPatternsStatic = _inputContextPatterns;
        _prefabStatic = _prefab;
    }

    public static InputContext CreateNewInputContext(InputContext context, Transform track, Camera camera, PlayerControlManager.PlayerController listenController, UnityAction successEvent = null, UnityAction failureEvent = null)
    {
        context = ((InputContext)Instantiate(context, _inputContextParentStatic)).Init(listenController);
        //context.transform.SetParent(_inputContextParentStatic, false);
        context._successEvent.AddListener(successEvent);
        context._failureEvent.AddListener(failureEvent);
        context._trackingTransform = track;
        context._cameraToTrackFrom = camera;
        return context;
    }

    public static InputContext CreateNewRandomInputContext(int length, bool allowResets, Transform track, Camera camera, PlayerControlManager.PlayerController listenController, UnityAction successEvent = null, UnityAction failureEvent = null)
    {

        InputContext context = ((InputContext)Instantiate(_prefabStatic, _inputContextParentStatic)).InitRandom(listenController, length, allowResets);

        //context.transform.SetParent(_inputContextParentStatic, false);
        context._successEvent.AddListener(successEvent);
        context._failureEvent.AddListener(failureEvent);
        context._trackingTransform = track;
        context._cameraToTrackFrom = camera;
        return context;
    }

    public static InputContext CreateNewManagerInputContext(Transform track, Camera camera, PlayerControlManager.PlayerController listenController)
    {

        InputContext context = ((InputContext)Instantiate(_prefabStatic, _inputContextParentStatic)).InitCompareToManager(listenController);
        //context.transform.SetParent(_inputContextParentStatic, false);
        context._trackingTransform = track;
        context._cameraToTrackFrom = camera;
        return context;
    }

    public static bool ComparePatternToManagerPattern(List<InputContext.ThumbstickPositions> pattern)
    {
        foreach(InputContextPattern pat in _inputContextPatternsStatic)
        {
            if (pat._command.Count == pattern.Count)
            {
                for (int i = 0; i < pat._command.Count; i++)
                {
                    if (pattern[i] != pat._command[i])
                    {
                        return false;
                    }
                }
                pat._successEvent.Invoke();
                return true;

            }
        }
        return false;
    }

}
