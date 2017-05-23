﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour {

    [System.Serializable]
    public struct ObjectivePrefab
    {
        public LevelPopulation.PrefabSize size;
        public ObjectiveAbstract objective;
    }

    public enum GameObjectiveState
    {
        NotComplete,
        TimeToGo,
        Done
    }

    public delegate void OnObjectiveComplete (ObjectiveAbstract objective);

    public OnObjectiveComplete onObjectiveSuccess;
    public OnObjectiveComplete onObjectiveFailure;

    public ObjectivePrefab[] objectivePrefabs;
    public ObjectiveExit exitPrefab;
    public int amountOfObjectives;

    public List<ObjectiveAbstract> objectives;

    private GameObjectiveState objectiveState = GameObjectiveState.NotComplete;
    private static Vector3[] objectivePositions;

    private void Update()
    {
        if (CheckAllObjectivesDone() && objectiveState == GameObjectiveState.NotComplete)
        {
            Debug.Log("All objectives done");
            objectiveState = GameObjectiveState.TimeToGo;
            SpawnExit();
        }
    }

    public void SpawnExit()
    {
        exitPrefab.gameObject.SetActive(true);
        exitPrefab.SetCallback(OnExit);
    }

    public void OnExit()
    {
        Debug.Log("Exit Game");
        GameManager.EndGame();
    }

    public List<ObjectivePrefab> DecideObjectives(int amount, out int bigObjective, out int midObjectives)
    {
        midObjectives = 0;
        bigObjective = 0;
        List<ObjectivePrefab> decidedList = new List<ObjectivePrefab>();
        for (int i = 0; i < amount; i++)
        {
            ObjectivePrefab randomObjective = objectivePrefabs[Random.Range(0, objectivePrefabs.Length)];
            if(randomObjective.size == LevelPopulation.PrefabSize.Medium)
            {
                midObjectives++;
            }else if (randomObjective.size == LevelPopulation.PrefabSize.Large)
            {
                bigObjective++;
            }
            decidedList.Add(randomObjective);
            midObjectives++; // for objective exit
        }
        return decidedList;
        
    }

    public void PopulateObjectives(List<ObjectivePrefab> decidedObjs,List<Vector3>mediumSpots, List<Vector3>bigSpots)
    {
        objectivePositions = new Vector3[decidedObjs.Count];
        for (int i = 0; i < decidedObjs.Count; i++)
        {
            ObjectivePrefab objective = decidedObjs[i];
            Vector3 position = Vector3.zero;
            if (objective.size == LevelPopulation.PrefabSize.Medium)
            {
                int rand = Random.Range(0, mediumSpots.Count);
                position = mediumSpots[rand];
                mediumSpots.RemoveAt(rand);
            }else if(objective.size == LevelPopulation.PrefabSize.Large)
            {
                int rand = Random.Range(0, bigSpots.Count);
                position = bigSpots[rand];
                mediumSpots.RemoveAt(rand);
            }
            if (objective.objective != null)
            {
                ObjectiveAbstract obj = Instantiate<ObjectiveAbstract>(objective.objective);
                obj.transform.position = position;
                objectivePositions[i] = position;
                RegisterObjective(obj);
            }
        }
        exitPrefab = Instantiate<ObjectiveExit>(exitPrefab);
        exitPrefab.transform.position = mediumSpots[0];
        exitPrefab.gameObject.SetActive(false);
    }

    public void RegisterObjective(ObjectiveAbstract obj)
    {
        objectives.Add(obj);
        obj.AddObjectiveCallbacks(AddSuccessScore, AddFailureScore);
    }

    public void AddSuccessScore(ObjectiveAbstract obj)
    {
        onObjectiveSuccess(obj);
        GameManager.AddScore(obj._objectiveSuccessScore);
    }

    public void AddFailureScore(ObjectiveAbstract obj)
    {
        onObjectiveFailure(obj);
        GameManager.AddScore(obj._objectiveFailureScore);
    }

    public bool CheckAllObjectivesDone()
    {
        if(objectives.Count > 0)
        {
            foreach(ObjectiveAbstract obj in objectives)
            {
                if (obj != null && obj._objectiveActive)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public ObjectiveAbstract findClosestObjectiveToPlayers()
    {
        if(objectiveState == GameObjectiveState.TimeToGo)
        {
            return exitPrefab;
        }
        ObjectiveAbstract bestObjective = null;
        if (objectives.Count > 0) {
            Vector3 playerCentre = GameManager.GetPlayersCentre();
            float bestDistance = Mathf.Infinity;
            if (objectives[0]._objectiveActive)
            {
                bestObjective = objectives[0];
                bestDistance = Vector3.Distance(objectives[0].transform.position, playerCentre);
            }
            foreach (ObjectiveAbstract obj in objectives)
            {
                if (obj._objectiveActive)
                {
                    float distance = Vector3.Distance(obj.transform.position, playerCentre);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestObjective = obj;
                    }
                }
            }
        }
        return bestObjective;
    }

    public static Vector3[] getObjectivePositions()
    {
        return objectivePositions;
    }

}
